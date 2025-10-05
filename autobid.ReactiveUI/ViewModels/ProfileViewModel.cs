using autobid.Domain.Database;
using autobid.Domain.Security;
using autobid.Domain.Users;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels;

public sealed class ProfileViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly IUserProfileReadService _read;
    private readonly User _user;

    public ViewModelActivator Activator { get; } = new();

    public ProfileViewModel(IUserProfileReadService read, User user) : base("Profile")
    {
        _read = read;
        _user = user;
        ToggleChangePasswordCommand = ReactiveCommand.Create(() => { IsChangingPassword = true; });
        RefreshCommand = ReactiveCommand.CreateFromTask(LoadAsync);
        ChangePasswordCommand = ReactiveCommand.CreateFromTask(ChangePassword);
        BackCommand = ReactiveCommand.Create(NavigateBack);
        LogOutCommand = ReactiveCommand.Create(LogOut);
        ChangeBalanceCommand = ReactiveCommand.CreateFromTask(ChangeBalance);

        ToggleChangeBalance = ReactiveCommand.Create(() => { IsChangingBalance = true; });

        // Autoload når view aktiveres (kræver ReactiveUserControl i viewet)
        this.WhenActivated(d =>
        {
            RefreshCommand.Execute().Subscribe().DisposeWith(d);
        });
    }

    public ReactiveCommand<Unit, Unit> ToggleChangeBalance {get;}
    public ReactiveCommand<Unit, Unit> ChangeBalanceCommand { get; }

    public bool IsCorporate => _user is CorporateCustomer;

    // -------- Bindable properties --------
    private string? _username;
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    bool _isChangingBalance;
    public bool IsChangingBalance
    {
        get => _isChangingBalance;
        private set => this.RaiseAndSetIfChanged(ref _isChangingBalance, value, nameof(IsChangingBalance));
    }

    bool _isChangingPassword;
    public bool IsChangingPassword
    {
        get => _isChangingPassword;
        private set => this.RaiseAndSetIfChanged(ref _isChangingPassword, value, nameof(IsChangingPassword));
    }

    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set
        {
            this.RaiseAndSetIfChanged(ref _balance, value);
            this.RaisePropertyChanged(nameof(BalanceText));
        }
    }
    public string BalanceText => $"{Balance:0},-";

    private int _auctionCount;
    public int AuctionCount
    {
        get => _auctionCount;
        set => this.RaiseAndSetIfChanged(ref _auctionCount, value);
    }

    private int _auctionsWon;
    public int AuctionsWon
    {
        get => _auctionsWon;
        set => this.RaiseAndSetIfChanged(ref _auctionsWon, value);
    }

    string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set => this.RaiseAndSetIfChanged(ref _newPassword, value, nameof(NewPassword));
    }

    string _newPasswordRepeat = string.Empty;
    public string NewPasswordReapeat
    {
        get => _newPasswordRepeat;
        set => this.RaiseAndSetIfChanged(ref _newPasswordRepeat, value, nameof(NewPasswordReapeat));
    }

    decimal? _newBalance;
    public decimal? NewBalance
    {
        get => _newBalance;
        set => this.RaiseAndSetIfChanged(ref _newBalance, value, nameof(NewBalance));
    }

    // -------- Commands (som XAML’en binder til) --------
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleChangePasswordCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangePasswordCommand { get; }
    public ReactiveCommand<Unit, Unit> BackCommand { get; }
    public ReactiveCommand<Unit, Unit> LogOutCommand { get; }

    // -------- Handlers --------
    private async Task LoadAsync()
    {
        var p = await _read.GetAsync(_user.Id);
        Username = p.Username;
        Balance = p.Balance;
        AuctionCount = p.AuctionCount;
        AuctionsWon = p.AuctionsWon;
    }

    private async Task ChangePassword()
    {
        if (NewPassword != NewPasswordReapeat)
            return;
        Hasher hasher = new();
        string hashed = hasher.Hash(NewPassword);

        UserRepository userRepository = new();
        await userRepository.UpdatePassword(hashed, _user.Id);
    }

    private void NavigateBack()
    {
        MainWindowViewModel.ChangeContent(new ShellViewModel(_user));
    }

    void LogOut()
    {
        MainWindowViewModel.ChangeContent(new LoginViewModel());
    }

    async Task ChangeBalance()
    {
        UserRepository userRepository = new();
        _user.Balance = NewBalance ?? 0;
        Balance = NewBalance ?? 0;

        await userRepository.UpdateBalance(_user.Id, NewBalance ?? 0);
    }
}
