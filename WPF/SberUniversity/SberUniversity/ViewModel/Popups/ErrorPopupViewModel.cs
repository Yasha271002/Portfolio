using System.Windows.Input;
using Core;
using SberUniversity.Helpers;

namespace SberUniversity.ViewModel.Popups
{
    public class ErrorPopupViewModel:ObservableObject
    {
        public ErrorPopupViewModel() { }

        public ICommand OnStartPageCommand => GetOrCreate(new RelayCommand(f=>
        {
            CommonCommands.ClosePopupCommand.Execute(null);
            CommonCommands.NavigateCommand.Execute(PageTypes.StartPage);
        }));
    }
}
