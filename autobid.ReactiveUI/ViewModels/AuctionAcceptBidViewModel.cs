using autobid.Domain.Auctions;
using autobid.Domain.Common;
using autobid.Domain.Database;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class AuctionAcceptBidViewModel : ViewModelBase
	{
		Auction _auction;
		public bool HasBids => _auction.Bids.Any();
		public decimal MinimumPrice => _auction.MinimumPrice;
		public DateTime CloseDate => _auction.CloseDate.DateTime;
        public decimal HighestBid => _auction.HighestBid?.Amount ?? 0;
        public decimal? CurrentHighestBid => _auction.HighestBid?.Amount;
		public string VehicleName => _auction.Vehicle.Name;
		public EnergyClass VehicleEnergy => _auction.Vehicle.Energy;
		public Fuel VehicleFuel => _auction.Vehicle.Fuel;
		public int VehicleYear => _auction.Vehicle.Year;
		public double KmPerLiter => _auction.Vehicle.KmPerLiter;
		public string SellerUsername => _auction.Seller.Username;
        public ReactiveCommand<Unit, Unit> CloseAuctionCommand { get; }
        public AuctionAcceptBidViewModel(Auction auction) : base("Accept Bid")
		{
			_auction = auction;
			CloseAuctionCommand = ReactiveCommand.CreateFromTask(AcceptBid);
        }

		private async Task AcceptBid()
		{
			SqlAuctionRepository repo = new();
			UserRepository userRepository = new();
            await repo.CloseAuction(_auction.Id);
			Bid bid = _auction.HighestBid!;
			bid.Buyer.Balance -= bid.Amount;
            await userRepository.UpdateBalance(bid.Buyer.Id, bid.Buyer.Balance);
			_auction.Seller.Balance += bid.Amount;
            await userRepository.UpdateBalance(_auction.Seller.Id, _auction.Seller.Balance);

            ShellViewModel.GoToHomePage();
        }
    }
}
