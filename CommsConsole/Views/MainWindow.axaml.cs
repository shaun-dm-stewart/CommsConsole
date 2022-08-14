using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Packet;
using CommsConsole.ViewModels;
using Protocols;
using System.IO.Ports;

namespace CommsConsole.Views
{
    public partial class MainWindow : Window
    {
        readonly IMainWindowViewModel _mwvm;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMainWindowViewModel mwvm)
        {
            InitializeComponent();
            _mwvm = mwvm;
            this.DataContext = _mwvm;
        }
    }
}
