using autobid.Domain.Users;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using autobid.Domain.Database;
using autobid.Domain.Auctions;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace autobid.ReactiveUI.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly User _user;
        readonly SqlAuctionRepository _repository = new();
        public ObservableCollection<AuctionListItem> YourAuctions { get; } = new();
        public ObservableCollection<AuctionListItem> CurrentAuctions { get; } = new();

        public ReactiveCommand<Unit, Unit> SetForSaleCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowProfileCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowBidHistoryCommand { get; }

        public HomeViewModel(User user) : base("Home view")
        {
            _user = user;

            SetForSaleCommand = ReactiveCommand.Create(OpenSetForSale);
            ShowProfileCommand = ReactiveCommand.Create(OpenProfile);    // ← hook metoden op her
            ShowBidHistoryCommand = ReactiveCommand.Create(() => { /* navigate bid history */ });
        }

        public async Task LoadAuctions()
        {
            var auctionListItems = await _repository.GetAllAuctonOpenListItems();

			foreach (var auction in auctionListItems)
            {
                CurrentAuctions.Add(auction);
            }

            foreach (var auction in auctionListItems.Where((auction) => auction.Username == _user.Username))
            {
                YourAuctions.Add(auction);
            }

		}

		private void OpenSetForSale()
        {
            var vm = new SetForSaleViewModel(_user); // VM-first
            MainWindowViewModel.ChangeContent(vm);   // ViewLocator viser SetForSaleView
        }

        private void OpenProfile()
        {
            var svc = new autobid.Domain.Database.UserProfileReadService();
            var vm = new ProfileViewModel(svc, _user);     // giv User, ikke kun Id
            MainWindowViewModel.ChangeContent(vm);          // VM-first → ViewLocator viser viewet
        }
    }
}
