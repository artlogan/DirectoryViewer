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
    public class DirectoryItem : TreeViewItem
    {
        public static readonly RoutedEvent DirectSelection = EventManager.RegisterRoutedEvent(
            name: "DirectSelected",
            routingStrategy: RoutingStrategy.Direct,
            ownerType: typeof(TreeViewItem),
            handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler DirectSelected
        {
            add { AddHandler(DirectSelection, value); }
            remove { RemoveHandler(DirectSelection, value); }
        }
        public static readonly RoutedEvent DirectExpansion = EventManager.RegisterRoutedEvent(
           name: "DirectExpanded",
           routingStrategy: RoutingStrategy.Direct,
           ownerType: typeof(TreeViewItem),
           handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler DirectExpanded
        {
            add { AddHandler(DirectExpansion, value); }
            remove { RemoveHandler(DirectExpansion, value); }
        }
        public static readonly RoutedEvent DirectCollapse = EventManager.RegisterRoutedEvent(
           name: "DirectCollapsed",
           routingStrategy: RoutingStrategy.Direct,
           ownerType: typeof(TreeViewItem),
           handlerType: typeof(RoutedEventHandler));
        public event RoutedEventHandler DirectCollapsed
        {
            add { AddHandler(DirectCollapse, value); }
            remove { RemoveHandler(DirectCollapse, value); }
        }
        protected override void OnSelected(RoutedEventArgs e)
        {
            RaiseEvent(new(routedEvent: DirectSelection));
        }
        protected override void OnExpanded(RoutedEventArgs e)
        {
            FillChildren();
            RaiseEvent(new(routedEvent: DirectExpansion));
        }
        protected override void OnCollapsed(RoutedEventArgs e)
        {
            ClearChildren();
            RaiseEvent(new(routedEvent: DirectCollapse));
        }



        public readonly string _Address;
        public readonly string _Name;
        public readonly string _Type;
        public readonly long _Size;
        enum Item_Type
        { 
            File, 
            Directory
        }
        Item_Type _Item_Type;

        public DirectoryItem(string addr)
        {
            _Address = addr;
            _Name = _Address.Substring(_Address.LastIndexOf('\\') + 1);
            _Item_Type = (File.GetAttributes(_Address) & FileAttributes.Directory) == FileAttributes.Directory ? Item_Type.Directory : Item_Type.File;
            _Type = _Item_Type == Item_Type.Directory ? "Directory" : _Address.Substring(_Address.LastIndexOf('.') + 1);
            _Size = _Type.Equals("Directory") ? -1 : (new FileInfo(_Address)).Length;

            StackPanel bar = new StackPanel();
            bar.Orientation = Orientation.Horizontal;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = _Name;

            Image img = new Image();

            TransformedBitmap tbmp = new TransformedBitmap();
            tbmp.BeginInit();
            tbmp.Source = (BitmapSource)(new IconBitmapDecoder(new Uri(
                _Item_Type == Item_Type.File ? @"C:\Users\Owner\source\repos\TreeDirectory\TreeDirectory\document.ico" :
                @"C:\Users\Owner\source\repos\TreeDirectory\TreeDirectory\folder_open.ico"), 0, 0).Frames[0]);
            tbmp.Transform = new ScaleTransform(16.0 / tbmp.Source.Height, 16.0 / tbmp.Source.Width);
            tbmp.EndInit();

            img.Source = tbmp;
            bar.Children.Add(img);
            bar.Children.Add(textBlock);
            Header = bar;

            Fill();
        }

        public string getName()
        { 
            return _Name;
        }

        public void ClearChildren()
        {
            foreach (DirectoryItem child in Items)
                child.Items.Clear();
        }
        public void Fill()
        {
            if(_Item_Type == Item_Type.Directory)
                foreach (string s in Directory.EnumerateFileSystemEntries(_Address))
                    Items.Add(new DirectoryItem(s));
        }
        void FillChildren()
        {
            foreach (DirectoryItem child in Items)
                child.Fill();
        }
    }

    public class DirectoryItemBar
    {
        public string _Name { get; set; }
        public string _Type { get; set; }
        public long _Size { get; set; }

        public readonly DirectoryItem parent;

        public DirectoryItemBar(DirectoryItem di)
        {
            _Name = di._Name;
            _Type = di._Type;
            _Size = di._Size;

            parent = di;
        }
    }
}
