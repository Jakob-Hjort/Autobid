using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.Domain.Auctions
{
    public class BidHistoryEntry
    {
        public string ?VehicleName { get; init; }
        public int Year { get; init; }
        public decimal BidAmount { get; init; }
        public decimal? FinalAmount { get; init; }
        public bool IsWinner { get; init; }
        public bool IsNotWinner { get; init; }
    }

}
