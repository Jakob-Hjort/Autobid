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
        public AuctionListItemViewModel SelectedItemFromYourAuctions
        {
            set
            {
                Task.Run(async () =>
                {
                    Auction? auction = await _repository.FindById(value.Id);
                    if (auction != null)
                        ShellViewModel.ChangeContent(new AuctionAcceptBidViewModel(auction));

				});
            }
        }

		public AuctionListItemViewModel SelectedItemFromCurrentAuctions
		{
			set
			{
				Task.Run(async () =>
				{
					Auction? auction = await _repository.FindById(value.Id);
					if (auction != null)
                    {
						if (_user.Username == value.Username)
                        {
							ShellViewModel.ChangeContent(new AuctionAcceptBidViewModel(auction));
						}
                        else
                        {
							ShellViewModel.ChangeContent(new AuctionMakeBidViewModel(auction, _user));
						}
					}

				});
			}
		}


		private readonly User _user;
        readonly SqlAuctionRepository _repository = new();
        public ObservableCollection<AuctionListItemViewModel> YourAuctions { get; } = new();
        public ObservableCollection<AuctionListItemViewModel> CurrentAuctions { get; } = new();

        public ReactiveCommand<Unit, Unit> SetForSaleCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowProfileCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowBidHistoryCommand { get; }


        public HomeViewModel(User user) : base("Home view")
        {
            _user = user;

            SetForSaleCommand = ReactiveCommand.Create(OpenSetForSale);
            ShowProfileCommand = ReactiveCommand.Create(OpenProfile);
            ShowBidHistoryCommand = ReactiveCommand.Create(OpenBidHistory);
            _=LoadAuctions();
        }

        private async Task LoadAuctions()
        {
            var auctionListItems = await _repository.GetAllAuctonOpenListItems();

			foreach (var auction in auctionListItems)
            {
                CurrentAuctions.Add(new(auction, _user));
            }

            foreach (var auction in auctionListItems.Where((auction) => auction.Username == _user.Username))
            {
                YourAuctions.Add(new(auction, _user));
            }

		}

		private void OpenSetForSale()
        {
            var vm = new SetForSaleViewModel(_user); // VM-first
            MainWindowViewModel.ChangeContent(vm);   // ViewLocator viser SetForSaleView
        }

        private void OpenProfile()
        {
            var svc = new UserProfileReadService();
            var vm = new ProfileViewModel(svc, _user);     // giv User, ikke kun Id
            ShellViewModel.ChangeContent(vm);          // VM-first → ViewLocator viser viewet
        }

        private void OpenBidHistory()
        {
            var vm = new BidHistoryViewModel(_user);     
            ShellViewModel.ChangeContent(vm);          
        }
    }
}
