using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Stores;

namespace TheBookOfMemory.ViewModels.Popups;

public partial class ReviewPhotoPopupViewModel(
    string photo,
    CloseNavigationService<ModalNavigationStore> closePopupNavigationService,
    GoBackNavigationService<ModalNavigationStore> goBackNavigationService) : BasePopupViewModel(closePopupNavigationService)
{
    [ObservableProperty]
    private string _photo = photo;

    [RelayCommand]
    private void GoBack() => goBackNavigationService.Navigate();
}