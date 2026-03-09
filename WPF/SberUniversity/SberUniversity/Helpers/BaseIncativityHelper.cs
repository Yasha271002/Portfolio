using SberUniversity.Utilities;
using System.Windows.Threading;

namespace SberUniversity.Helpers
{
    public class BaseInactivityHelper
    {
        public BaseInactivityHelper(int waitTime)
        {
            _inactivityTime = waitTime;
            _timer.Tick += TimerOnTick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }

        private readonly int _inactivityTime;
        private readonly DispatcherTimer _timer = new();

        public event InactivityEvent? OnInactivity;

        public delegate void InactivityEvent(int inactivityTime);

        private void TimerOnTick(object? sender, EventArgs e)
        {
            if (UserInactivity.GetSeconds() < _inactivityTime || UserInactivity.GetSeconds() > _inactivityTime) return;
            OnInactivity?.Invoke(UserInactivity.GetSeconds());
        }
    }

    public class InactivityHelper(int waitTime) : BaseInactivityHelper(waitTime);

    public class PasswordInactivityHelper(int waitTime) : BaseInactivityHelper(waitTime);

    public class UpdateCacheInactivityHelper(int waitTime) : BaseInactivityHelper(waitTime);
}