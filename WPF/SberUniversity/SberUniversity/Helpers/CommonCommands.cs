using Core;
using SberUniversity.Views.Page;
using System.Windows.Controls;
using System.Windows.Input;
using SberUniversity.Model;
using SberUniversity.ViewModel.Views;
using SberUniversity.Views.Popups;
using UserControl = System.Windows.Controls.UserControl;

namespace SberUniversity.Helpers
{
    public static class CommonCommands
    {
        private static Page? GetPageByType(PageTypes pageType)
        {
            return pageType switch
            {
                PageTypes.StartPage => new StartPage(),
                PageTypes.CompletedPage => new CompletedPage(),
                PageTypes.None => null,
                _ => null
            };
        }

        private static Page? GetPageByContent(object content)
        {
            Page? page = content switch
            {
                QuizModel quiz => new QuizPage()
                {
                    DataContext = new QuizPageViewModel(quiz)
                },
                ResultModel result => new ResultPage()
                {
                    DataContext = new ResultPageViewModel(result)
                },
                _ => null
            };
            return page;
        }

        public static ICommand NavigateCommand { get; } = new RelayCommand(f =>
        {
            Page? page;
            if (f is PageTypes pageType)
            {
                page = GetPageByType(pageType);
            }
            else
            {
                page = GetPageByContent(f);
            }

            if (page == null) return;
            NavigationManager.MainFrame.Navigate(page);
        });

        private static UserControl? GetPopupByType(PopupTypes popupType)
        {
            return popupType switch
            {
                PopupTypes.PersonalDataPopup => new UserAgreement(),
                PopupTypes.UserAgreements => new PersonalData(),
                PopupTypes.ErrorPopup => new ErrorPopup(),
                _ => null
            } ;
        }

        public static ICommand? GoBackCommand { get; } =
            new RelayCommand(f => { NavigationManager.MainFrame.GoBack(); });

        public static ICommand? GoBackPopupCommand { get; } = new RelayCommand(f =>
        {
            NavigationManager.PopupFrame.GoBack();
        });

        public static ICommand ClosePopupCommand { get; } =
            new RelayCommand(obj => { NavigationManager.ClosePopup(); });

        public static ICommand OpenPopupCommand { get; } = new RelayCommand(obj =>
        {
            UserControl? popup;
            if (obj is PopupTypes popupType)
            {
                popup = GetPopupByType(popupType);
            }
            else
            {
                popup = GetPopupByContent(obj);
            }

            if (popup is null)
                return;
            NavigationManager.PopupFrame?.Navigate(popup);
            NavigationManager.Instance.IsPopupOpen = true;
        });

        private static UserControl? GetPopupByContent(object content)
        {
            return content switch
            {
                _ => null
            };
        }
    }
}