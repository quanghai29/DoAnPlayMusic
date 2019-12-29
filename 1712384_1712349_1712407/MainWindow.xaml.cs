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
using System.Diagnostics;

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
        BindingList<songs> MiniLists = new BindingList<songs>();//Thể hiện các bài hát trong 1 list
        BindingList<mylist> MyLists = new BindingList<mylist>();//chứa các list
        BindingList<songs> BigestList = new BindingList<songs>();//chứa toàn bộ bài hát
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Nạp MyLists 
            var filename = "SuperList.txt";
            var Listsfile = "Lists.txt";
            if (File.Exists(Directory.GetCurrentDirectory() + $"\\{filename}"))
            {
                loadAllSongs(filename, BigestList);
                operationListBox.ItemsSource = BigestList;
            }
            if(File.Exists(Directory.GetCurrentDirectory() + $"\\{Listsfile}"))
            {
                loadLists();
            }
            var countLists = MyLists.Count;
            if(countLists>0)
            {
                for(int i=0;i<countLists;i++)
                {
                    var fn = MyLists[i].namelist + ".txt";
                    MyLists[i].songsList = new List<songs>();
                    BindingList<songs> songs = new BindingList<songs>(MyLists[i].songsList);
                    if (songs!=null)
                        loadAllSongs(fn,songs);
                    MyLists[i].countOfList = MyLists[i].songsList.Count;
                }
            }
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
                    namelist = listName,
                    countOfList = 0,
                    songsList = new List<songs>()
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
                var info = new FileInfo(screen.FileName);
                //Thêm bài hát vào danh sách trên màn hình hiện tại
                var song = new songs()
                {
                    pathfile = info,
                    //duration = player.showDuration(),
                    //Index = _lastIndex
                };

                BigestList.Add(song);
                operationListBox.ItemsSource = null;
                operationListBox.ItemsSource = BigestList;
                StaticDiskBorder.Visibility = Visibility.Collapsed;
                RotateDiskBorder.Visibility = Visibility.Visible;
            }
        }

        private void deleteOperationItem_Click(object sender, RoutedEventArgs e)
        {
            deleteSongs(BigestList);
        }

        private void deleteSongs(BindingList<songs> BindingListName)
        {
            if (operationListBox.SelectedIndex == -1)
            {
                return;
            }

            //_fullPaths.RemoveAt(playlistListBox.SelectedIndex);
            while (operationListBox.SelectedItems.Count > 0)
            {
                var index = operationListBox.Items.IndexOf(operationListBox.SelectedItem);
                if (BindingListName[index].isPlaying == true)//xóa một bài nhạc đang chơi
                {
                    player.DeletePlayer();
                    _isPlaying = false;//cập nhật lại trạng thái chơi nhạc
                    _lastIndex = -1;
                }
                BindingListName.RemoveAt(index);
            }
        }


        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSongs(BigestList);
        }

        bool _isPlaying = false;//Trạng thái chơi nhạc(có đang chơi hay không)
        int _lastIndex = -1;//
        /// <summary>
        /// Sau khi chọn 1 bài hát trong Bigest list để nghe->nhấn Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(_isPlaying)
            {
                player.DeletePlayer();
                player = null;
                if (_lastIndex > -1)
                    BigestList[_lastIndex].isPlaying = false;
            }
            int indexSong = operationListBox.SelectedIndex;
            _lastIndex = indexSong;//lưu lại 
            if (indexSong>=0)
            {
                PlayASong(indexSong);
            }
            else
            {
                MessageBox.Show("No file selected!");
            }
        }

        private void PlayASong(int indexSong)
        {
            BigestList[indexSong].isPlaying = true;
            PlaySelectedIndex(indexSong);
            if (player.isEnded())//đã chơi hết bài nhạc
            {
                _isPlaying = false;
                BigestList[indexSong].isPlaying = false;
            }
        }
        /// <summary>
        /// Thực hiện play bài hát được chọn trong list 
        /// </summary>
        /// <param name="indexSong">index của bài hát trong list</param>
        private void PlaySelectedIndex(int indexSong)
        {
            _isPlaying = true;
            var filename = BigestList[indexSong].pathfile;
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

        /// <summary>
        /// Mở hộp thoại PlaylistDialog để user chọn list nhạc để play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayListButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new PlayListDialog(MyLists);
            if (screen.ShowDialog() == true)
            {
                MessageBox.Show(screen.ListNameSelected);
            }
        }


       
        /// <summary>
        /// Save lại những tên List hiện có
        /// </summary>
        private void saveLists()
        {
            const string filename = "Lists.txt";
            var writer = new StreamWriter(filename);
            var count = MyLists.Count;
            //writer.WriteLine(count.ToString());
            writer.WriteLine($"{count}");
            for (int i=0;i<count;i++)
            {
                writer.WriteLine(MyLists[i].namelist);
            }
            writer.Close();
        }

        private void loadLists()
        {
            const string filename = "Lists.txt";
            var reader = new StreamReader(filename);
            var count = int.Parse(reader.ReadLine());
            //Debug.WriteLine(count.ToString());
            for (int i=0;i<count;i++)
            {
                var list = new mylist()
                {
                    namelist = reader.ReadLine(),
                };
                MyLists.Add(list);
            }
            reader.Close();
        }
        /// <summary>
        /// Save lại những bài hát trong một list(có nhiều list)
        /// </summary>
        private void saveSongsOfList()
        {
            var count = MyLists.Count;
            for(int i=0;i<count;i++)
            {
                var filename = MyLists[i].namelist+".txt";
                var writer = new StreamWriter(filename);
                var countSongs = MyLists[i].songsList.Count;
                writer.WriteLine($"{countSongs}");
                for (int j=0;j<countSongs;j++)
                {
                    writeSongToFile(MyLists[i].songsList[j], writer);
                }
                writer.Close();
            }
        }

        /// <summary>
        /// Lưu tất cả bài hát
        /// </summary>
        private void saveAllSongs()
        {
            var count = BigestList.Count;
            const string filename = "SuperList.txt";
            var writer = new StreamWriter(filename);
            writer.WriteLine($"{count}");
            writer.WriteLine($"{_lastIndex}");
            for (int i = 0; i < count; i++)
            {
                writeSongToFile(BigestList[i], writer);
            }
            writer.Close();
        }

        /// <summary>
        /// Load lên tất cả bài hát có trong file filename lưu vào List
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="List"></param>
        private void loadAllSongs(string filename,BindingList<songs>List)
        {
            var reader = new StreamReader(filename);
            var count = int.Parse(reader.ReadLine());
            if(filename=="SuperList.txt")
                _lastIndex = int.Parse(reader.ReadLine());
            for(int i=0;i<count;i++)
            {
                var tokens = reader.ReadLine().Split(new string[] { "@" }, StringSplitOptions.None);
                var song = new songs();
                song.pathfile = new FileInfo(tokens[0]);
                song.singer = tokens[1];
                song.duration = tokens[2];
                if (int.Parse(tokens[3]) == 1)
                    song.isPlaying = true;
                else
                    song.isPlaying = false;
                List.Add(song);
            }
            reader.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="writer"></param>
        private void writeSongToFile(songs song,StreamWriter writer)
        {
            var data = new StringBuilder();
            data.Append(song.pathfile.ToString());
            data.Append("@");
            data.Append(song.singer);
            data.Append("@");
            data.Append(song.duration);
            data.Append("@");
            if (song.isPlaying)
            {
                data.Append("1");
            }
            else
                data.Append("0");
            writer.WriteLine(data);
        }

        /// <summary>
        /// Lưu lại những thứ cần thiết trước khi tắt chương trình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            saveLists();
            saveAllSongs();
            saveSongsOfList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var filename = MyLists[0].namelist + ".txt";
            //MessageBox.Show(filename);
            //saveLists();
            //saveSongsOfList();
        }

        /// <summary>
        /// Sự kiện này xảy ra sau khi sự kiện Loaded. Hiện Message Box hỏi người dùng 
        /// có muốn play lại bài hát đang chơi dở dang ko
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (_lastIndex >= 0)
            {
                //System.Threading.Thread.Sleep(1000);
                if (MessageBox.Show("Do you want to close this window?",
               "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PlayASong(_lastIndex);
                }
                else
                {
                    BigestList[_lastIndex].isPlaying = false;
                }
            }
        }

        private void addToList_Click(object sender, RoutedEventArgs e)
        {
            string listName = inputDiaLog("input", true);
            if (listName != null)
            {
               var check = checkListName(listName);
                var index = operationListBox.SelectedIndex;
               if (check>-1)
                {
                    MyLists[check].songsList.Add(BigestList[index]);
                    MyLists[check].countOfList += 1;
                }
               else//tạo mới danh sách có tên== listName
                {
                    var newList = new mylist()
                    {
                        namelist = listName,
                        songsList=new List<songs>(),
                        countOfList=0
                    };
                    newList.songsList.Add(BigestList[index]);
                    newList.countOfList += 1;

                    MyLists.Add(newList);
                }
            }
        }

        /// <summary>
        /// Kiểm tra tên listName có trong các playlist ko
        /// </summary>
        /// <param name="listName">tên playlist cần kiểm tra</param>
        /// <returns>
        /// index của listName nếu nó có trong các playlist
        /// ngược lại trả về -1
        /// </returns>
        private int checkListName(string listName)
        {
            for(int i=0;i<MyLists.Count; i++)
            {
                if(listName==MyLists[i].namelist)
                {
                    return i;
                }
            }
            return -1;
        }

        private void addSongToList(songs song,List<songs>listSongs)
        {

        }
    }
}
