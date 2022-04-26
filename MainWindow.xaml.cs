using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace TreeDirectory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DirectoryItem current;
        DirectoryItem clipboard;

        public MainWindow()
        {
            InitializeComponent();
            DirectoryItem outer = new DirectoryItem(@"E:\Users\Bill\Documents");
            directory_view.Items.Add(outer);
            DirectorySelected(outer, new RoutedEventArgs());
        }

        private void DirectorySelected(object sender, RoutedEventArgs e)
        {
            bool fromList = sender.GetType().Equals(typeof(ListViewItem));
            DirectoryItem caller = fromList ? (DirectoryItem)(((DirectoryItemBar)(((ListViewItem)sender).Content)).parent) : (DirectoryItem)sender;
            if (caller._Type.Equals("Directory"))
            {
                current = caller;
                if (!fromList)
                {
                    directory_list.Items.Clear();
                    foreach (DirectoryItem d in caller.Items)
                        directory_list.Items.Add(new DirectoryItemBar(d));
                    updateLine(caller._Address);
                    PreviewPanel.Text = "Directory";
                }
                else
                    PreviewPanel.Text = "Directory";
            }
            else
            {
                PreviewPanel.Text = "";
                int counter = 0;
                foreach (string line in System.IO.File.ReadLines(caller._Address))
                {
                    if (counter++ >= 128)
                        break;
                    PreviewPanel.Text += line + '\n';
                }
                if (counter >= 128)
                    PreviewPanel.Text += "...";
            }
        }
        private void updateLine(string addr)
        {
            File_Line.Children.Clear();
            foreach (string s in addr.Split('\\', StringSplitOptions.RemoveEmptyEntries))
            { 
                Button b = new Button();
                TextBlock t = new TextBlock();
                t.Text = s;
                b.Content = t;
                File_Line.Children.Add(b);
            }
        }
        private void lineClick(object sender, RoutedEventArgs e)
        {
            List<Button> buttons = new List<Button>();
            string addr = "";
            foreach (Button b in File_Line.Children)
            {
                buttons.Add(b);
                addr += ((TextBlock)(b.Content)).Text + @"\";
                if (b == (Button)sender)
                    break;
            }
            File_Line.Children.Clear();
            foreach (Button b in buttons)
                File_Line.Children.Add(b);
            DirectorySelected(new DirectoryItem(addr), new RoutedEventArgs());
        }
        private void DirectoryExpanded(object sender, RoutedEventArgs e)
        {
        }
        private void DirectoryCollapsed(object sender, RoutedEventArgs e)
        {
        }
        private void OpenDirectory(object sender, RoutedEventArgs e)
        {
            DirectoryItemBar dib = (DirectoryItemBar)(((ListViewItem)sender).Content);
            if (dib._Type.Equals("Directory"))
            {
                dib.parent.Fill();
                DirectorySelected(dib.parent, new RoutedEventArgs());
            }
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            Copy(sender, e);
            Delete(sender, e);
        }
        private void Copy(object sender, RoutedEventArgs e)
        {
            clipboard = current;
        }
        private void Paste(object sender, RoutedEventArgs e)
        {

        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            if (((DirectoryItem)sender)._Type.Equals("Directory"))
                (new DirectoryInfo(((DirectoryItem)sender)._Address)).Delete();
            else
                (new FileInfo(((DirectoryItem)sender)._Address)).Delete();
            DirectorySelected(current, new RoutedEventArgs());
        }
        private void Duplicate(object sender, RoutedEventArgs e)
        {
            Copy(sender, e);
            Paste(sender, e);
        }
        private void NewFolder(object sender, RoutedEventArgs e)
        {

        }
        private void Rename(object sender, RoutedEventArgs e)
        {

        }
        private void Zip(object sender, RoutedEventArgs e)
        {

        }
        private void Print(object sender, RoutedEventArgs e)
        {

        }

    }
}
