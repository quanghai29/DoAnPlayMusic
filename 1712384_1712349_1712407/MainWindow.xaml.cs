using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.Build.Tasks;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            
        }
        Player player;
        BindingList<songs> ListSongs = new BindingList<songs>();
        BindingList<mylist> MyLists = new BindingList<mylist>();
        int _lastIndex = -1;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Title= player.showTitle();
        }

        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // lấy input chỉ bằng một nốt nhạc
            string listName = inputDiaLog("input", true);
            if(listName!=null)
            {
                var list = new mylist()
                {
                    namelist = listName
                };
                MyLists.Add(list);
                listFavouriteSong.ItemsSource = MyLists;
            }
        }

        //Hàm hỗ trợ gọi hộp thoại input
        private string inputDiaLog(string inputBoolen, bool animation)
        {
            var screen = new NottifyDiaglog(inputBoolen, animation);
            if (screen.ShowDialog() == true)
            {
                return screen.strInput;
            }
            return null;
        }

        private void OpenSongButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new Microsoft.Win32.OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                _lastIndex += 1;
                var info = new FileInfo(screen.FileName);
                //Thêm bài hát vào danh sách trên màn hình hiện tại
                var song = new songs()
                {
                    pathfile = info,
                    //duration = player.showDuration(),
                    //Index = _lastIndex
                };

                ListSongs.Add(song);
                operationListBox.ItemsSource = null;
                operationListBox.ItemsSource = ListSongs;
                StaticDiskBorder.Visibility = Visibility.Collapsed;
                RotateDiskBorder.Visibility = Visibility.Visible;
            }
        }

        private void deleteOperationItem_Click(object sender, RoutedEventArgs e)
        {
            deleteSongs();
        }

        private void deleteSongs()
        {
            if (operationListBox.SelectedIndex == -1)
            {
                return;
            }

            //_fullPaths.RemoveAt(playlistListBox.SelectedIndex);
            while (operationListBox.SelectedItems.Count > 0)
            {
                var index = operationListBox.Items.IndexOf(operationListBox.SelectedItem);
                ListSongs.RemoveAt(index);
            }
        }


        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSongs();
        }

        bool _isPlaying = false;

        /// <summary>
        /// Sau khi chọn 1 bài hát trong list để nghe->nhấn Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(_isPlaying)
            {
                player.sound.Stop();
                player.timer=null;
                player = null;
            }
            int indexSong = operationListBox.SelectedIndex;
            if (indexSong>=0)
            {
                PlaySelectedIndex(indexSong);
            }
            else
            {
                MessageBox.Show("No file selected!");
            }
        }

        /// <summary>
        /// Thực hiện play bài hát được chọn trong list 
        /// </summary>
        /// <param name="indexSong">index của bài hát trong list</param>
        private void PlaySelectedIndex(int indexSong)
        {
            _isPlaying = true;
            var filename = ListSongs[indexSong].pathfile;
            //Tạo một lượt chơi nhạc
            player = new Player()
            {
                pathfile = filename
            };
            player.init();
            player.listening();
            //Tính thời gian
            player.timer.Tick += timer_Tick;
        }
    }
}
