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

        // Vises i headeren
        public string Greeting => $"Velkommen, {_user.Username}";

        // Venstremenuens punkter
        public ObservableCollection<NavItem> Pages { get; }

        // Det view der vises i content-området
        private ViewModelBase? _currentPage;
        public ViewModelBase? CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        // Selektion i venstremenuen
        private NavItem? _selectedPage;
        public NavItem? SelectedPage
        {
            get => _selectedPage;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPage, value);
                if (value is not null) CurrentPage = value.ViewModel;
            }
        }

        public ReactiveCommand<Unit,Unit> GoToHomeCommand { get; }
        public ReactiveCommand<Unit,Unit> GoToProfileCommand { get; }
        public ReactiveCommand<Unit,Unit> GoToSetForSaleCommand { get; }

        public ShellViewModel(User user)
        {
            _user = user;

            GoToHomeCommand = ReactiveCommand.Create(GoToHome);
            GoToProfileCommand = ReactiveCommand.Create(GoToProfile);
            GoToSetForSaleCommand = ReactiveCommand.Create(GoToSetForSale);

            //var profileSvc = new UserProfileReadService(); // din eksisterende service

            //Pages = new ObservableCollection<NavItem>
            //{
            //    new NavItem("Home",        new HomeViewModel(_user),                    "🏠"),
            //    new NavItem("Set for sale",new SetForSaleViewModel(_user),             "🛒"),
            //    new NavItem("Profile",     new ProfileViewModel(profileSvc, _user),    "👤"),
            //};

            //SelectedPage = Pages.FirstOrDefault(); // vælg første som default
        }

        void GoToSetForSale() =>
            CurrentPage = new SetForSaleViewModel(_user);

        void GoToProfile() =>
            CurrentPage = new ProfileViewModel(new UserProfileReadService(), _user);

        void GoToHome() =>
            CurrentPage = new HomeViewModel(_user);
    }
}
