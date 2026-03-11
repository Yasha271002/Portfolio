using CommunityToolkit.Mvvm.Input;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Stores;
using VolgaTermalBathsEnter.ViewModels.Pages;

namespace VolgaTermalBathsEnter.ViewModels.Popups;

public partial class ThanksGoTurnstilePopupViewModel(
    CloseNavigationService<ModalNavigationStore> closeModalNavigationService,
    NavigationService<WelcomeUserViewModel> welcomeUserNavigationService)
    : BasePopupViewModel(closeModalNavigationService)
{
    [RelayCommand]
    private void WelcomeUserNavigation()
    {
        CloseContainerCommand.Execute(false);
        welcomeUserNavigationService.Navigate();
    }
}