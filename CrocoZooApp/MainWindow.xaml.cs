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

namespace CrocoZooApp;


    public partial class WelcomePage : UserControl
    {

    private Frame _mainFrame;

    public WelcomePage(Frame mainFrame)
    {
        InitializeComponent();
        _mainFrame = mainFrame;
    }

    private void WelcomeButton_Click(object sender, RoutedEventArgs e)
    {
        // Naviguer vers mode.xaml (UserControl)
        _mainFrame.Content = new mode(_mainFrame);
    }

}
