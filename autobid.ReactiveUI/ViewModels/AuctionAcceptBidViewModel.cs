using autobid.Domain.Auctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class AuctionAcceptBidViewModel : ViewModelBase
	{
		Bid? _bid;
		public AuctionAcceptBidViewModel(Bid bid) : base("Accept Bid")
		{
			_bid = bid;
		}
	}
}
