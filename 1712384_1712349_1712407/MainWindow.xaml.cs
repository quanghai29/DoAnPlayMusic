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
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents _hook;
        public MainWindow()
        {
            InitializeComponent();

            //Đăng kí sự kiện hook 
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += keyUp_hook;
        }
        Player player;
       
        // Convert:Chức năng trung gian để chuyển đổi các playlist
        //ánh xạ các playlist vào Listview để áp các hành động của user lên playlist được chọn
        BindingList<songs> Convert = new BindingList<songs>();
        BindingList<mylist> MyLists = new BindingList<mylist>();//chứa các list
        BindingList<songs> BigestList = new BindingList<songs>();//chứa toàn bộ bài hát

        int countRepeat = 0; // số lần lặp
        bool stop = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Nạp MyLists 
            var filename = "SuperList.txt";
            var Listsfile = "Lists.txt";
            if (File.Exists(Directory.GetCurrentDirectory() + $"\\{filename}"))
            {
                loadAllSongs(filename, BigestList);
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

            if (_isPlayingMiniList >= 0)
            {
                Convert = new BindingList<songs>(MyLists[_isPlayingMiniList].songsList);
                ListNameLable.Content = MyLists[_isPlayingMiniList].namelist;
            }
            else
            {
                Convert = BigestList;
            }

            operationListBox.ItemsSource = Convert;
        }

        int secTotal = 0;
        private void timer_Tick(object sender, EventArgs e)
        {
            lblProgressStatus.Text = player.showPlayTime();
            if (secTotal == 0)
            {
                var duration = player.sound.NaturalDuration;
                if (duration.HasTimeSpan)
                {
                    var min = int.Parse(duration.TimeSpan.ToString(@"mm"));
                    var sec = int.Parse(duration.TimeSpan.ToString(@"ss"));
                    secTotal = min * 60 + sec;
                }
            }
            else
            {
                musicProgressBar.Value += (double)100 / secTotal;
            }
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

        /// <summary>
        /// Thêm một bài hát vào danh sách tổng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                if (File.Exists(Directory.GetCurrentDirectory() + $"\\SuperList.txt")==false)
                {
                    Convert = BigestList;
                    operationListBox.ItemsSource = Convert;
                }
                //BigestList.Add(song);
                Convert.Add(song);
                //operationListBox.ItemsSource = null;
                //operationListBox.ItemsSource = BigestList;
            }
        }

        private void deleteOperationItem_Click(object sender, RoutedEventArgs e)
        {
            //deleteSongs(BigestList);
            deleteSongs(Convert);
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
                if (BindingListName[index].isPlaying == true)//xóa một bài nhạc đang chơi(đang chơi dang dở, được lưu lại)
                {
                    if (_isPlaying)//đang có bài hát được chơi
                    {
                        player.DeletePlayer();
                        _isPlaying = false;//cập nhật lại trạng thái chơi nhạc
                    }
                   
                    _lastIndex = -1;
                }
                BindingListName.RemoveAt(index);
            }
        }


        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            //deleteSongs(BigestList);
            deleteSongs(Convert);
        }

        bool _isPlaying = false;//Trạng thái chơi nhạc(có đang chơi hay không)
        int _lastIndex = -1;//lưu lại index(trong listview) của bài hát vừa mới play xong 
        int  _isPlayingMiniList = -1;//đang nghe nhạc ở danh sách tổng
        /// <summary>
        /// Sau khi chọn 1 bài hát trong Bigest list để nghe->nhấn Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSong = operationListBox.SelectedIndex;
            if(indexSong>=0)
            {
                var screen = new CountRepeat();
                if (screen.ShowDialog() == true)
                {
                    countRepeat = screen.countRepeat;
                }

                if (_isPlaying && indexSong >= 0)
                {
                    player.DeletePlayer();
                    player = null;
                    if (_lastIndex > -1)
                    {
                        Convert[_lastIndex].isPlaying = false;
                    }
                }

                _lastIndex = indexSong;//lưu lại 
               
                PlayButton.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Visible;
                PlayASong(indexSong);
            }
            else
            {
                System.Windows.MessageBox.Show("No file selected!");
                return;
            }
            player.sound.MediaEnded += player_MediaEnded_PlayOne;
           
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying && _lastIndex >= 0)
            {
                player.DeletePlayer();
                player = null;
                Convert[_lastIndex].isPlaying = false;
                _isPlaying = false;
                _lastIndex = -1;
                songNameTextblock.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Collapsed;
                PlayButton.Visibility = Visibility.Visible;
                StaticDiskBorder.Visibility = Visibility.Visible;
                RotateDiskBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void player_MediaEnded_PlayOne(object sender, EventArgs e)
        {

            if (countRepeat == 0)
            {
                StaticDiskBorder.Visibility = Visibility.Visible;
                RotateDiskBorder.Visibility = Visibility.Collapsed;
                return;
            }
                
            if (countRepeat == 1)
            {
                _isPlaying = false;
                Convert[_lastIndex].isPlaying = false;
                PlayASong(_lastIndex);
                player.sound.MediaEnded += player_MediaEnded_PlayOne;
            }

        }

        private void PlayASong(int indexSong)
        {
            if (player != null)
            {//kill đi đối tượng player đang tồn tại
                player.DeletePlayer();
                player = null;
            }
            secTotal = 0;
            musicProgressBar.Value = 0;

            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            songNameTextblock.Visibility = Visibility.Visible;
            songNameTextblock.Content= System.IO.Path.GetFileNameWithoutExtension(Convert[indexSong].pathfile.Name); 
            

            Convert[indexSong].isPlaying = true;
            PlaySelectedIndex(indexSong);
            if (player.isEnded())//đã chơi hết bài nhạc
            {
                _isPlaying = false;
                Convert[indexSong].isPlaying = false;
            }
        }
        /// <summary>
        /// Thực hiện play bài hát được chọn trong list 
        /// </summary>
        /// <param name="indexSong">index của bài hát trong list</param>
        private void PlaySelectedIndex(int indexSong)
        {
            _isPlaying = true;
            var filename = Convert[indexSong].pathfile;
            //Tạo một lượt chơi nhạc
            player = new Player()
            {
                pathfile = filename,
            };
            player.pathfile = filename;
            player.init();
            player.listening();
            songNameTextblock.Visibility = Visibility.Visible;
            songNameTextblock.Content = player.getFileName();
            StaticDiskBorder.Visibility = Visibility.Collapsed;
            RotateDiskBorder.Visibility = Visibility.Visible;
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
                //MessageBox.Show(screen.ListNameSelected);
                var listnameSelected= screen.ListNameSelected;
                var indexSelected = screen.indexSelected;
                if(listnameSelected!="")
                {
                    _isPlayingMiniList = indexSelected;//đang chơi bài nhạc thuộc List thứ indexSelected trong List<mylist>
                    ListNameLable.Content = listnameSelected;
                    BindingList<songs> listData = new BindingList<songs>(MyLists[indexSelected].songsList);
                    Convert = listData;
                    operationListBox.ItemsSource = Convert;
                }
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
            writer.WriteLine($"{_isPlayingMiniList}");
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
            _isPlayingMiniList = int.Parse(reader.ReadLine());
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
        /// Lưu tất cả bài hát có trong danh sách tổng
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
            DisposeHook();
        }

        /// <summary>
        /// Button All songs trên màn hình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _isPlayingMiniList = -1;
            Convert = BigestList;
            ListNameLable.Content = "Bài hát";
            operationListBox.ItemsSource = Convert;
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
                if (System.Windows.MessageBox.Show("Do you want to continue with last section",
               "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PlayASong(_lastIndex);
                }
                else
                {
                    Convert[_lastIndex].isPlaying = false;
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

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayAllButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new CountRepeat();
            if(screen.ShowDialog() == true)
            {
                countRepeat = screen.countRepeat;
            }

            _lastIndex = 0;
            if(_isPlaying)
            {
                player.DeletePlayer();
            }
           
            PlayASong(_lastIndex);
            player.sound.MediaEnded += player_MediaEnded;
           
        }

        private void player_MediaEnded(object sender, EventArgs e)
        {
            if (_lastIndex == (operationListBox.Items.Count - 1))
            {
                if(countRepeat == 0)
                {
                    StaticDiskBorder.Visibility = Visibility.Visible;
                    RotateDiskBorder.Visibility = Visibility.Collapsed;
                    return;
                }
                    
                if(countRepeat == 1)
                {
                    _lastIndex = 0;
                    _isPlaying = false;
                    Convert[_lastIndex].isPlaying = false;
                    PlayASong(_lastIndex);
                    player.sound.MediaEnded += player_MediaEnded;
                }
            }
            else
            {
                _isPlaying = false;
                Convert[_lastIndex].isPlaying = false;
                _lastIndex++;
                PlayASong(_lastIndex);
                player.sound.MediaEnded += player_MediaEnded;
            }
            
        }

        List<int> Indexes = new List<int>();

        /// <summary>
        ///  play ngẫu nhiên
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayRandomButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new CountRepeat();
            if (screen.ShowDialog() == true)
            {
                countRepeat = screen.countRepeat;
            }

            Indexes.Clear();
            for (int i = 0; i < operationListBox.Items.Count; i++)
            {
                Indexes.Add(i);
            }

            Random r = new Random();
            _lastIndex = Indexes[r.Next(0, Indexes.Count)];
            Indexes.Remove(_lastIndex);

            if (_isPlaying)
            {
                player.DeletePlayer();
            }

            PlayASong(_lastIndex);
            player.sound.MediaEnded += player_MediaEndedRandom;

        }

        private void player_MediaEndedRandom(object sender, EventArgs e)
        {
            if (Indexes.Count == 0)
            {
                if (countRepeat == 0)
                {
                    StaticDiskBorder.Visibility = Visibility.Visible;
                    RotateDiskBorder.Visibility = Visibility.Collapsed;
                    return;
                }
                    
                if (countRepeat == 1)
                {
                    Indexes.Clear();
                    for (int i = 0; i < operationListBox.Items.Count; i++)
                    {
                        Indexes.Add(i);
                    }

                    Random r1 = new Random();
                    _lastIndex = Indexes[r1.Next(0, Indexes.Count)];
                    Indexes.Remove(_lastIndex);

                    _isPlaying = false;
                    Convert[_lastIndex].isPlaying = false;
                    PlayASong(_lastIndex);
                    player.sound.MediaEnded += player_MediaEnded;
                }
            }

            _isPlaying = false;
            Convert[_lastIndex].isPlaying = false;

            Random r = new Random();
            _lastIndex = Indexes[r.Next(0, Indexes.Count)];
            Indexes.Remove(_lastIndex);
  

            PlayASong(_lastIndex);

            

            player.sound.MediaEnded += player_MediaEndedRandom;
        }

       

        private void editOperationItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("SuperTop Sorry!This function is not be installed");
        }


        private void keyUp_hook(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //quay lại bài trước
            if(e.KeyCode==Keys.Left)
            {
                if(_isPlaying==true && _lastIndex>0)
                {
                    player.DeletePlayer();
                    player = null;
                    Convert[_lastIndex].isPlaying = false;
                    PlayASong(_lastIndex-1);
                    _lastIndex -= 1;//cập nhật lại _lastIndex
                }
            }

            //chuyển sang bài kế
            if (e.KeyCode == Keys.Right)
            {
                var count = operationListBox.Items.Count;
                if (_isPlaying == true && _lastIndex < count-1)
                {
                    player.DeletePlayer();
                    player = null;
                    Convert[_lastIndex].isPlaying = false;
                    PlayASong(_lastIndex + 1);
                    _lastIndex += 1;//cập nhật lại _lastIndex
                }
            }

            //play
           

            //pause
           
        }

        private void DisposeHook()
        {
            _hook.KeyUp -= keyUp_hook;
            _hook.Dispose();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                player.sound.Pause();
                StaticDiskBorder.Visibility = Visibility.Visible;
                RotateDiskBorder.Visibility = Visibility.Visible;
            }
            else
            {
                player.sound.Play();
                StaticDiskBorder.Visibility = Visibility.Collapsed;
                RotateDiskBorder.Visibility = Visibility.Visible;
            }
            _isPlaying = !_isPlaying;
        }
    }
}
