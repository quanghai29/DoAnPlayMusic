using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for PlayListDialog.xaml
    /// </summary>
    public partial class PlayListDialog : Window
    {
        public string ListNameSelected = "";
        private BindingList<mylist> copy_myLists = new BindingList<mylist>();
        public PlayListDialog(BindingList<mylist> myLists)
        {
            InitializeComponent();
            copy_myLists = myLists;
            playListListView.ItemsSource = copy_myLists;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var index = playListListView.SelectedIndex;
            if(index>-1)
            {
                ListNameSelected = copy_myLists[index].namelist;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please choose a list to play!");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var index = playListListView.SelectedIndex;
            if(index>-1)
            {
                var fn = Directory.GetCurrentDirectory() + "\\" + copy_myLists[index].namelist+".txt";
                File.Delete(fn);
                copy_myLists.RemoveAt(index);
            }
            else
            {
                MessageBox.Show("Nothing to delete");
            }
        }
    }
}
