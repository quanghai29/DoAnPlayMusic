using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace _1712384_1712349_1712407
{
    public class Player
    {
        public MediaPlayer sound { get; set; }
        public FileInfo pathfile { get; set; }
        public DispatcherTimer timer { get; set;}

        public void DeletePlayer()
        {
            sound.Stop();
            timer.Stop();
            timer = null;
        }
        public void init()
        {
            sound = new MediaPlayer();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
        }

        public void listening()
        {
            sound.Open(new Uri(pathfile.FullName,UriKind.Absolute));
            sound.Play();
            timer.Start();
        }

        public string showTitle()
        {
            var Title = "";
            var filename = pathfile.Name;
            var shortname = System.IO.Path.GetFileNameWithoutExtension(filename);

            var currentPos = sound.Position.ToString(@"mm\:ss");
            var duration = showDuration();
            Title = String.Format($"{currentPos} / {duration} - {shortname}");
            return Title;
        }

        public string showDuration()
        {
            if (sound.NaturalDuration.HasTimeSpan == false)
                return "trinh";
            string duration= sound.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            return duration;
        }

        public bool isEnded()
        {
            if (sound.Position == sound.NaturalDuration)
                return true;
            return false;
        }
    }

    public class SoundSystem
    {
        public void setVolume(int level)
        {
            //set the volume level
        }
    }

    public class songs
    {
        public FileInfo pathfile { get; set; }
        public string singer { get; set; }
        public string duration { get; set; }
        public bool isPlaying { get; set; }
        //public int index { get; set; }//index của bài hát trong danh sách tổng
    }

    public class mylist
    {
        public string namelist { get; set; }
        public List<songs> songsList{ get; set; }

        public int countOfList { get; set; }//số lượng bài hát trong một list
    }
}
