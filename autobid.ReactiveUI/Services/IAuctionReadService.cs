using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.Services
{
    public interface IAuctionReadService
    {
        Task<IEnumerable<(string Name, int Year, decimal? HighestBid)>> GetAuctionsBySellerAsync(uint sellerId);
        Task<IEnumerable<(string Name, int Year, decimal? HighestBid)>> GetCurrentAuctionsAsync();
    }
}
