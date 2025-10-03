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
        private readonly Action? _goBack;


        //public BidHistoryViewModel(BidRepository repo, User userId, Action? goBack = null) : base("Bid history")
        //{
        //    _bidRepo = repo;                  
        //    _User = userId;
        //    _goBack = goBack;



        //    // evt. læg en dummy række for at teste binding
        //    // Entries.Add(new BidHistoryEntry{ VehicleName="(test)", Year=2020, BidAmount=123, FinalAmount=456 });

        //    _ = LoadAsync();                 // BREAKPOINT B: rammer vi inde i LoadAsync?
        //}

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