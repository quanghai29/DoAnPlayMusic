﻿using System;
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var screen = new Microsoft.Win32.OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                var info = new FileInfo(screen.FileName);
                //Tạo một lượt chơi nhạc
                player = new Player()
                {
                    pathfile = info
                };
                player.init();
                player.listening();
                //Thêm bài hát vào danh sách trên màn hình hiện tại
                var song = new songs()
                {
                    pathfile = info,
                    duration = player.showDuration()
                };

                ListSongs.Add(song);
                operationListBox.ItemsSource = ListSongs;

                //Tính thời gian
                player.timer.Tick += timer_Tick;
               
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Title= player.showTitle();
        }
    }
}