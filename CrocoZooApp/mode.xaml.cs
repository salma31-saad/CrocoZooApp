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


namespace CrocoZooApp
{
    /// <summary>
    /// Logique d'interaction pour menu.xaml
    /// </summary>
    public partial class mode : Window
    {

        //private Frame _mainFrame;
        //public mode(Frame mainFrame)
        //{
        //    InitializeComponent();
        //   _mainFrame = mainFrame;
        //}

        public mode()
        {
            InitializeComponent();

        }
        private void BackToWelcome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindowPage = new MainWindow();
            mainWindowPage.Show();
        }
    }
}
