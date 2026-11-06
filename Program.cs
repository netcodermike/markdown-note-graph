using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
record Note(string Name,string Path,string Text,string[] Links);
class Vault {
 public static string[] Links(string text)=>Regex.Matches(text,@"\[\[([^\]\r\n|]+)(?:\|[^\]\r\n]*)?\]\]").Select(m=>m.Groups[1].Value.Trim()).Where(s=>s.Length>0).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
 public static List<Note> Read(string folder){
  if(!Directory.Exists(folder))throw new IOException("Folder does not exist");
  var notes=new List<Note>();foreach(var path in Directory.EnumerateFiles(folder,"*.md",SearchOption.TopDirectoryOnly).Take(501)){
   if(notes.Count==500)throw new IOException("Vault exceeds the 500-note limit");if(new FileInfo(path).Length>1024*1024)throw new IOException("Note exceeds 1 MiB: "+path);
   var text=File.ReadAllText(path);notes.Add(new(System.IO.Path.GetFileNameWithoutExtension(path),path,text,Links(text)));
  }return notes.OrderBy(n=>n.Name,StringComparer.OrdinalIgnoreCase).ToList();
 }
 public static void Save(Note note,string text){var tmp=note.Path+".notegraph.tmp";File.WriteAllText(tmp,text);File.Move(tmp,note.Path,true);}
}
class Program {
 [STAThread]static int Main(string[] args){
  if(args.Contains("--self-test")){
   if(!Vault.Links("[[Alpha]] [[Beta|label]] [[alpha]]").SequenceEqual(new[]{"Alpha","Beta"}))return 1;
   var dir=System.IO.Path.Combine(System.IO.Path.GetTempPath(),Guid.NewGuid().ToString());Directory.CreateDirectory(dir);
   try{File.WriteAllText(System.IO.Path.Combine(dir,"Alpha.md"),"[[Beta]]");File.WriteAllText(System.IO.Path.Combine(dir,"Beta.md"),"hello");var sample=Vault.Read(dir);if(sample.Count!=2||sample[0].Links[0]!="Beta")return 1;Vault.Save(sample[1],"updated");if(Vault.Read(dir)[1].Text!="updated")return 1;return 0;}finally{Directory.Delete(dir,true);}
  }
  var app=new Application();var window=new Window{Title="Markdown Note Graph",Width=1150,Height=760,MinWidth=900,MinHeight=600};
  var root=new DockPanel{Margin=new Thickness(18)};window.Content=root;var toolbar=new StackPanel{Orientation=Orientation.Horizontal};DockPanel.SetDock(toolbar,Dock.Top);root.Children.Add(toolbar);
  var folder=new TextBox{Text=args.FirstOrDefault()??Environment.CurrentDirectory,Width=500,Margin=new Thickness(4)};toolbar.Children.Add(folder);
  var search=new TextBox{Width=180,Margin=new Thickness(4),ToolTip="Search note names/text"};toolbar.Children.Add(search);
  var columns=new Grid();columns.ColumnDefinitions.Add(new(){Width=new GridLength(210)});columns.ColumnDefinitions.Add(new(){Width=new GridLength(1,GridUnitType.Star)});columns.ColumnDefinitions.Add(new(){Width=new GridLength(330)});root.Children.Add(columns);
  var list=new ListBox{Margin=new Thickness(4)};columns.Children.Add(list);var editor=new TextBox{AcceptsReturn=true,AcceptsTab=true,VerticalScrollBarVisibility=ScrollBarVisibility.Auto,FontFamily=new FontFamily("Consolas"),TextWrapping=TextWrapping.Wrap,Margin=new Thickness(8)};Grid.SetColumn(editor,1);columns.Children.Add(editor);
  var side=new StackPanel();Grid.SetColumn(side,2);columns.Children.Add(side);var info=new TextBlock{TextWrapping=TextWrapping.Wrap,Margin=new Thickness(8)};side.Children.Add(info);var graph=new Canvas{Width=310,Height=310,Background=Brushes.WhiteSmoke};side.Children.Add(graph);var related=new ListBox{Height=210,Margin=new Thickness(8)};side.Children.Add(related);
  var notes=new List<Note>();Note? active=null;bool dirty=false,loading=false;
  bool Discard()=>!dirty||MessageBox.Show(window,"Discard unsaved edits?","Unsaved note",MessageBoxButton.YesNo)==MessageBoxResult.Yes;
  void Draw(){
   graph.Children.Clear();related.Items.Clear();if(active is null){info.Text="Open a folder of .md notes.";return;}
   var incoming=notes.Where(n=>n.Links.Contains(active.Name,StringComparer.OrdinalIgnoreCase)).Select(n=>n.Name);
   var names=active.Links.Concat(incoming).Distinct(StringComparer.OrdinalIgnoreCase).Take(10).ToArray();info.Text=$"{active.Name}\nOutgoing: {active.Links.Length} · incoming: {incoming.Count()}\nGraph shows up to 10 neighbors.";
   void Node(string name,double x,double y){var b=new Button{Content=name.Length>16?name[..16]+"…":name,Width=100,Height=30,ToolTip=name};Canvas.SetLeft(b,x-50);Canvas.SetTop(b,y-15);b.Click+=(_,_)=>list.SelectedItem=notes.FirstOrDefault(n=>n.Name.Equals(name,StringComparison.OrdinalIgnoreCase))?.Name;graph.Children.Add(b);}
   for(int i=0;i<names.Length;i++){double a=i*2*Math.PI/names.Length;double x=155+100*Math.Cos(a),y=155+100*Math.Sin(a);graph.Children.Add(new Line{X1=155,Y1=155,X2=x,Y2=y,Stroke=Brushes.SlateGray});}
   Node(active.Name,155,155);for(int i=0;i<names.Length;i++){double a=i*2*Math.PI/names.Length;Node(names[i],155+100*Math.Cos(a),155+100*Math.Sin(a));}
   foreach(var name in active.Links)related.Items.Add((notes.Any(n=>n.Name.Equals(name,StringComparison.OrdinalIgnoreCase))?"→ ":"Missing: ")+name);
   foreach(var name in incoming)related.Items.Add("← "+name);
  }
  void Filter(){var selected=active?.Name;list.Items.Clear();foreach(var n in notes.Where(n=>(n.Name+" "+n.Text).Contains(search.Text,StringComparison.OrdinalIgnoreCase)))list.Items.Add(n.Name);if(selected!=null)list.SelectedItem=selected;}
  void Open(){if(!Discard())return;try{var next=Vault.Read(folder.Text);loading=true;notes=next;active=null;dirty=false;editor.Text="";Filter();loading=false;Draw();}catch(Exception e)when(e is IOException or UnauthorizedAccessException){loading=false;MessageBox.Show(window,e.Message);}}
  void Button(string label,Action action){var b=new Button{Content=label,Margin=new Thickness(4),Padding=new Thickness(8)};b.Click+=(_,_)=>action();toolbar.Children.Add(b);}
  Button("Open / refresh",Open);Button("Save note",()=>{if(active is null)return;try{if(File.ReadAllText(active.Path)!=active.Text){MessageBox.Show(window,"This note changed on disk. Refresh before saving.");return;}Vault.Save(active,editor.Text);active=active with{Text=editor.Text,Links=Vault.Links(editor.Text)};notes[notes.FindIndex(n=>n.Name==active.Name)]=active;dirty=false;Draw();}catch(Exception e)when(e is IOException or UnauthorizedAccessException){MessageBox.Show(window,e.Message);}});
  list.SelectionChanged+=(_,_)=>{if(loading||list.SelectedItem is not string name||active?.Name==name)return;if(!Discard()){loading=true;list.SelectedItem=active?.Name;loading=false;return;}active=notes.First(n=>n.Name==name);loading=true;editor.Text=active.Text;loading=false;dirty=false;Draw();};
  editor.TextChanged+=(_,_)=>{if(!loading&&active!=null)dirty=true;};search.TextChanged+=(_,_)=>{loading=true;Filter();loading=false;};window.Closing+=(_,e)=>{if(!Discard())e.Cancel=true;};Open();app.Run(window);return 0;
 }
}
