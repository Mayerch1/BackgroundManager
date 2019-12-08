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

namespace BackgroundManagerUI
{
    /// <summary>
    /// Interaction logic for UpdateNotifier.xaml
    /// </summary>
    public partial class UpdateNotifier : Window
    {
        public bool? result;

        public UpdateNotifier()
        {
            result = false;
            InitializeComponent();

            System.Media.SystemSounds.Exclamation.Play();
        }

        private void btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            result = true;
            this.Close();
        }

        private void btn_No_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            result = false;
            this.Close();
        }

        private void btn_Never_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null;
            result = null;
            this.Close();
        }
    }
}