using Core;
using System.Windows.Navigation;
using Application = System.Windows.Application;

namespace SberUniversity.Helpers
{
    public enum PageTypes
    {
        StartPage,
        CompletedPage,
        None
    }

    public enum PopupTypes
    {
        PersonalDataPopup,
        UserAgreements,
        ErrorPopup,
        None
    }

    public class NavigationManager : ObservableObject
    {
        public static NavigationManager Instance { get; } = new NavigationManager();

        public static NavigationService MainFrame
        {
            get => Instance.GetOrCreate<NavigationService>();
            set => Instance.SetAndNotify(value, callback: FrameChangedCallback);
        }

        public static NavigationService PopupFrame
        {
            get => Instance.GetOrCreate<NavigationService>();
            set => Instance.SetAndNotify(value);
        }

        public PageTypes CurrentPage
        {
            get => GetOrCreate<PageTypes>();
            set => SetAndNotify(value);
        }

        public bool IsPopupOpen
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value);
        }

        private static void FrameChangedCallback(PropertyChangingArgs<NavigationService> obj)
        {
            if (obj.OldValue is not null)
                obj.OldValue.Navigated -= OnNavigated;

            if (obj.NewValue is not null)
                obj.NewValue.Navigated += OnNavigated;
        }

        private static void OnNavigated(object sender, NavigationEventArgs e)
        {
            Instance.CurrentPage = GetCurrentPage();
        }

        private static PageTypes GetCurrentPage()
        {
            return MainFrame.Content switch
            {
                _ => PageTypes.None
            };
        }

        public static void ClosePopup()
        {
            Instance.IsPopupOpen = false;

            Task.Run(async () =>
            {
                await Task.Delay(500); //Animation

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Instance.IsPopupOpen)
                        PopupFrame?.Navigate(null);
                });
            });
        }

        public static void ClearHistory()
        {
            while (MainFrame.CanGoBack)
            {
                try
                {
                    MainFrame.RemoveBackEntry();
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}