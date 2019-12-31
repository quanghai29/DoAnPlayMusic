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
using System.Windows.Shapes;

namespace _1712384_1712349_1712407
{
    /// <summary>
    /// Interaction logic for CountRepeat.xaml
    /// </summary>
    public partial class CountRepeat : Window
    {
        public CountRepeat()
        {
            InitializeComponent();

        }

        public int countRepeat;

        private void SubmitDirecting(object sender, RoutedEventArgs e)
        {
            if (onceRadio.IsChecked == true)
            {
                countRepeat = 0;
                this.DialogResult = true;
            }
            else if (infinityRadio.IsChecked == true)
            {
                countRepeat = 1;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("No selected!");
            }

        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
