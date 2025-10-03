using autobid.Domain.Auctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class AuctionMakeBidViewModel : ViewModelBase
	{
		Auction _auction;
		Bid? bid;
		public AuctionMakeBidViewModel(Auction auction): base("Make Bid")
		{
			_auction = auction;
			bid = auction.HighestBid;
		}
	}
}
