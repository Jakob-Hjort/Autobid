using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Auctions
{
	public record AuctionListItem(uint Id, string CarName, int Year, decimal Bid, string Username);
}
