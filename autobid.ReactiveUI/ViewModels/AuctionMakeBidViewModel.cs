using autobid.Domain.Auctions;
using autobid.Domain.Common.Enums;
using autobid.Domain.Database;
using autobid.Domain.Users;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class AuctionMakeBidViewModel : ViewModelBase
	{
		User _user;
		Auction _auction;
		Bid? bid;
		SqlAuctionRepository _auctionRepository = new();
        public decimal MinimumValue
		{
			get
			{
				return _auction.MinimumPrice;
            }
        }


		public decimal HighestBid => bid?.Amount ?? 0;
        public string VehicleName => _auction.Vehicle.Name;
		public DateTime CloseDate => _auction.CloseDate.DateTime;
		public bool IsBidOn => bid != null;
        public EnergyClass VehicleEnergy => _auction.Vehicle.Energy;
        public Fuel VehicleFuel => _auction.Vehicle.Fuel;
        public int VehicleYear => _auction.Vehicle.Year;
        public double KmPerLiter => _auction.Vehicle.KmPerLiter;
        public string SellerUsername => _auction.Seller.Username;
        public ReactiveCommand<Unit, Unit> CancelBidCommand { get; }
        public ReactiveCommand<Unit, Unit> MakeBidCommand { get; }
        public ReactiveCommand<Unit, Unit> StartBidCommand { get; }
        private decimal _bidAmount;
		public decimal BidAmount
		{
			get => _bidAmount;
			set
			{
                this.RaiseAndSetIfChanged(ref _bidAmount, value, nameof(BidAmount));

				if (value > _user.Balance || value <= HighestBid)
					CanMakeBid = false;
                else
					CanMakeBid = true;
            }
        }

		bool isMakingBid = false;
		public bool IsMakingBid
		{
			get => isMakingBid;
			private set => this.RaiseAndSetIfChanged(ref isMakingBid, value, nameof(IsMakingBid));
        }

		bool _canMakeBid = false;
		public bool CanMakeBid
		{
			get => _canMakeBid;
			private set => this.RaiseAndSetIfChanged(ref _canMakeBid, value, nameof(CanMakeBid));
        }

        public AuctionMakeBidViewModel(Auction auction, User user): base("Make Bid")
		{
			MakeBidCommand = ReactiveCommand.CreateFromTask(MakeBid);
			CancelBidCommand = ReactiveCommand.Create(() => { IsMakingBid = false; });
			StartBidCommand = ReactiveCommand.Create(() => { IsMakingBid = true; });
            _auction = auction;
			bid = auction.HighestBid;
			_user = user;
        }

		async Task MakeBid()
		{
			if (_user.Balance < BidAmount)
				return;

            Bid bid = new(_user, BidAmount);
			_auction.AddBid(bid);
			this.RaisePropertyChanged(nameof(HighestBid));
            await _auctionRepository.AddBid(_auction.Id, new(_user, BidAmount));
			isMakingBid = false;
			ShellViewModel.ChangeContent(new BidHistoryViewModel(_user));
        }
    }
}
