using CommunityToolkit.Mvvm.Input;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using VolgaTermalBathsEnter.ViewModels.Pages;

namespace VolgaTermalBathsEnter.ViewModels.Popups;

public partial class ErrorPopupViewModel(
    INavigationService closeModalNavigationService,
    NavigationService<WelcomeUserViewModel> welcomePageNavigationService)
    : BasePopupViewModel(closeModalNavigationService)
{
    [RelayCommand]
    private void OnMainPage()
    {
        CloseContainerCommand.Execute(false);
        welcomePageNavigationService.Navigate();
    }

    [RelayCommand]
    private void Back() => CloseContainerCommand.Execute(false);
}