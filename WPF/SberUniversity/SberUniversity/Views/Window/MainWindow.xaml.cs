using SberUniversity.Helpers;
using System.Windows.Controls;

namespace SberUniversity.Views.Window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavigationManager.MainFrame = MainFrame.NavigationService;
        }

        private void PopupFrame_OnInitialized(object? sender, EventArgs e)
        {
            if (sender is not Frame frame) return;
            NavigationManager.PopupFrame = frame.NavigationService;
        }
    }
}