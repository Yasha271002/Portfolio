using Core;
using SberUniversity.Helpers;

namespace SberUniversity.ViewModel.Popups;

public class UserAgreementViewModel : ObservableObject
{
    public string? UserAgreement
    {
        get=> GetOrCreate(string.Empty); 
        set=> SetAndNotify(value);
    }

    public string? SberUniversity
    {
        get => GetOrCreate(string.Empty);
        set => SetAndNotify(value);
    }

    public UserAgreementViewModel()
    {
        UserAgreement = SingletonSettings.Instance.Settings.UserAgreement;
        SberUniversity = SingletonSettings.Instance.Settings.SberUniversity;
    }
}