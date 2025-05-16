using System.Text;
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

    public partial class MainWindow : Window
    {

        //private Frame _mainFrame;


        public MainWindow()
        {
            InitializeComponent();
            
        }

        //public MainWindow(Frame mainFrame)
        //{
        //    _mainFrame = mainFrame;
        //}

        private void WelcomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers mode.xaml (UserControl)
            //_mainFrame.Content = new mode(_mainFrame);
           // grid_Window.Children.Clear();
            mode modePage = new mode();
               modePage.Show();
            // MainFrame.Content = modePage;
        }




    }

}


