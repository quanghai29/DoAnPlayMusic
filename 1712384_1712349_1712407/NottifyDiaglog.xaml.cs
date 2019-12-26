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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for NottifyDiaglog.xaml
    /// </summary>
    public partial class NottifyDiaglog : Window
    {
        public string strInput { get; set; }
        public bool input = false;
        public NottifyDiaglog(string note,bool animation)
        {
            InitializeComponent();
            if(note=="input")
            {
                labelNotify.Visibility = Visibility.Collapsed;
                inputTextBox.Visibility = Visibility.Visible;
                input = true;
            }
            else
            {
                inputTextBox.Visibility = Visibility.Collapsed;
                labelNotify.Content = note;
            }
            if (animation)
                animateBegin();
            this.Left = 500;
            this.Top = 300;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (input == true)
            {
                if(inputTextBox.Text=="")
                {
                    NotifyErr.Visibility = Visibility.Visible;
                    return;
                }
                strInput = inputTextBox.Text;
            }
            this.DialogResult = true;
            this.Close();
        }

        
        private void animateBegin()
        {
            var animation = new DoubleAnimation();
            animation.From = 0.0;
            animation.By = 270;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.65));
            
            //animation.AutoReverse = true;
            //animation.RepeatBehavior = new RepeatBehavior(2);
            
            
            
            Storyboard story = new Storyboard();
            story.Children.Add(animation);
            Storyboard.SetTargetName(animation, notifydialog.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
            story.Begin(this);
            
        }
    }
}
