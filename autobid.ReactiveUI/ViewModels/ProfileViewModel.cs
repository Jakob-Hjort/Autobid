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
    private readonly User _user;                               // gem hele brugeren

    public ViewModelActivator Activator { get; } = new();

    public ProfileViewModel(IUserProfileReadService read, User user) : base("Profile")
    {
        _read = read;
        _user = user;

        RefreshCommand = ReactiveCommand.CreateFromTask(LoadAsync);
        ChangePasswordCommand = ReactiveCommand.Create(ChangePassword);
        BackCommand = ReactiveCommand.Create(NavigateBack);

        // Autoload når view aktiveres (kræver ReactiveUserControl i viewet)
        this.WhenActivated(d =>
        {
            RefreshCommand.Execute().Subscribe().DisposeWith(d);
        });
    }

    // -------- Bindable properties --------
    private string? _username;
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
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

    // -------- Commands (som XAML’en binder til) --------
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangePasswordCommand { get; }
    public ReactiveCommand<Unit, Unit> BackCommand { get; }

    // -------- Handlers --------
    private async Task LoadAsync()
    {
        var p = await _read.GetAsync(_user.Id);
        Username = p.Username;
        Balance = p.Balance;
        AuctionCount = p.AuctionCount;
        AuctionsWon = p.AuctionsWon;
    }

    private void ChangePassword()
    {
        // TODO: Navigér til ChangePasswordViewModel eller åbn dialog
        // MainWindowViewModel.ChangeContent(new ChangePasswordViewModel(_user));
    }

    private void NavigateBack()
    {
        MainWindowViewModel.ChangeContent(new HomeViewModel(_user));
    }
}
