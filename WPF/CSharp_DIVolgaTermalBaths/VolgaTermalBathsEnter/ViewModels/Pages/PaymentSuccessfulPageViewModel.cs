using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmNavigationLib.Services;

namespace VolgaTermalBathsEnter.ViewModels.Pages;

public partial class PaymentSuccessfulPageViewModel(NavigationService<PaidTicketViewModel> paidTicketNavigationService)
    : ObservableObject
{
    [RelayCommand] private void PaidTicketNavigation() => paidTicketNavigationService.Navigate();
}