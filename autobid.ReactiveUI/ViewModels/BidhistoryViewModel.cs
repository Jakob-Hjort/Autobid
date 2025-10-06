using autobid.Domain.Auctions;
using autobid.Domain.Database;
using autobid.Domain.Users;
using autobid.ReactiveUI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace autobid.ReactiveUI.ViewModels
{
    public class BidHistoryViewModel : ViewModelBase
    {
        public ObservableCollection<autobid.Domain.Auctions.BidHistoryEntry> Bids { get; } = new();

        private readonly BidRepository _bidRepo = new();
        private readonly User _User;

        public BidHistoryViewModel(User currentUser) : base("Bid History")
        {
            _User = currentUser;
            Load();
        }

        private async void Load()
        {
            var bids = await _bidRepo.GetBidHistoryForUserAsync(_User.Id);
            foreach (var b in bids)
                Bids.Add(b);
        }
    }
}