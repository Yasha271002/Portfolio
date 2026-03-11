using CommunityToolkit.Mvvm.ComponentModel;

namespace VolgaTermalBathsEnter.Models.Client.User.Ticket;

public partial class TicketPersonalInfoModel : ObservableObject
{
    [ObservableProperty] private string? _firstName = string.Empty;
    [ObservableProperty] private string? _secondName = string.Empty;
    [ObservableProperty] private string? _lastName = string.Empty;
    [ObservableProperty] private string? _fullName = string.Empty;
}