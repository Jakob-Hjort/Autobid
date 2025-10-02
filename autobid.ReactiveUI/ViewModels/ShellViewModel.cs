// autobid.ReactiveUI/ViewModels/ShellViewModel.cs
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using autobid.Domain.Users;
using autobid.Domain.Database;
using System.Reactive;

namespace autobid.ReactiveUI.ViewModels
{
    // Simpel menu-item model til venstremenuen
    public sealed class NavItem
    {
        public string Title { get; }
        public string? Icon { get; }          // valgfri: emoji / tegn
        public ViewModelBase ViewModel { get; }

        public NavItem(string title, ViewModelBase viewModel, string? icon = null)
        {
            Title = title;
            ViewModel = viewModel;
            Icon = icon;
        }
    }

    // Selve shellens ViewModel (header + venstremenu + content)
    public sealed class ShellViewModel : ViewModelBase
    {
        private readonly User _user;

       


        private ViewModelBase? _currentPage;
        public ViewModelBase? CurrentPage
        {
            get => _currentPage;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentPage, value);
            }
        }

        

        public ReactiveCommand<Unit,Unit> GoToHomeCommand { get; }
        public ReactiveCommand<Unit,Unit> GoToProfileCommand { get; }
        public ReactiveCommand<Unit,Unit> GoToSetForSaleCommand { get; }

        public ShellViewModel(User user) : base("Shell view")
        {
            _user = user;

            GoToHomeCommand = ReactiveCommand.Create(GoToHome);
            GoToProfileCommand = ReactiveCommand.Create(GoToProfile);
            GoToSetForSaleCommand = ReactiveCommand.Create(GoToSetForSale);

            GoToHome();
        }

        void GoToSetForSale()
        {
            CurrentPage = new SetForSaleViewModel(_user);
        }
           

        void GoToProfile() =>
            CurrentPage = new ProfileViewModel(new UserProfileReadService(), _user);

        void GoToHome() =>
            CurrentPage = new HomeViewModel(_user);
    }
}
