using Core;
using SberUniversity.Helpers;
using SberUniversity.Model;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SberUniversity.ViewModel.Window
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly JsonHelper _jsonHelper;
        private BaseInactivityHelper _inactivity;

        private DispatcherTimer _timer = new();
        private int _sec = 0;
        private ICommand? _stopTimerCommand;
        private ICommand? _startTimerCommand;

        private SettingsModel Settings
        {
            get => GetOrCreate<SettingsModel>();
            set => SetAndNotify(value);
        }

        public MainWindowViewModel()
        {
            ExplorerHelper.KillExplorer();

            _jsonHelper = new JsonHelper();
            Settings = new SettingsModel();

            GetSettings("Settings.json");

            _inactivity = new BaseInactivityHelper(Settings.InactivityTime);
            _inactivity.OnInactivity += InactivityOnOnInactivity;
        }

        private void InactivityOnOnInactivity(int inactivitytime)
        {
            CommonCommands.NavigateCommand.Execute(PageTypes.StartPage);
        }

        private void GetSettings(string path)
        {
            Settings = _jsonHelper.ReadJsonFromFile<SettingsModel>(path, Settings);
            SingletonSettings.Instance.Settings = Settings;
        }

        private void Timer(object sender, EventArgs eventArgs)
        {
            _sec++;
            if (_sec < 7) return;

            ExplorerHelper.RunExplorer();
            Application.Current.Shutdown();
        }

        public ICommand StopTimerCommand => _stopTimerCommand ??= new RelayCommand(a =>
        {
            _timer.Tick -= Timer;
            _timer.Stop();
            _sec = 0;
        });

        public ICommand StartTimerCommand => _startTimerCommand ??= new RelayCommand(a =>
        {
            _timer?.Stop();
            _sec = 0;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer;
            _timer.Start();
        });
    }
}