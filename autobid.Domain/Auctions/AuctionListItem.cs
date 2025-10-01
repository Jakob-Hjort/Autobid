using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Auctions
{
	public record AuctionListItem(string CarName, int Year, decimal LastBid, string Username);
}
