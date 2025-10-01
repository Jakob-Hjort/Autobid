using autobid.Domain.Users;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using autobid.Domain.Database;

namespace autobid.ReactiveUI.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly User _user;

        public ObservableCollection<AuctionRow> YourAuctions { get; } = new();
        public ObservableCollection<AuctionRow> CurrentAuctions { get; } = new();

        public ReactiveCommand<Unit, Unit> SetForSaleCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowProfileCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowBidHistoryCommand { get; }

        public HomeViewModel(User user)
        {
            _user = user;

            // demo-data – slet når I loader fra repo
            YourAuctions.Add(new("Ford Escort", 1983, 3000m));
            YourAuctions.Add(new("Tesla Model 3", 2016, null));
            CurrentAuctions.Add(new("Ford Escort", 1983, 3000m));
            CurrentAuctions.Add(new("Tesla Model 3", 2016, null));
            CurrentAuctions.Add(new("Scania R 730 V8", 2019, null));
            CurrentAuctions.Add(new("Skoda Octavia", 2008, null));

            SetForSaleCommand = ReactiveCommand.Create(() => { /* open create-auction */ });
            ShowProfileCommand = ReactiveCommand.Create(OpenProfile);    // ← hook metoden op her
            ShowBidHistoryCommand = ReactiveCommand.Create(() => { /* navigate bid history */ });
        }

        private void OpenProfile()
        {
            var svc = new autobid.Domain.Database.UserProfileReadService();
            var vm = new ProfileViewModel(svc, _user);     // giv User, ikke kun Id
            MainWindowViewModel.ChangeContent(vm);          // VM-first → ViewLocator viser viewet
        }
    }

    public record AuctionRow(string VehicleName, int Year, decimal? Bid);
}
