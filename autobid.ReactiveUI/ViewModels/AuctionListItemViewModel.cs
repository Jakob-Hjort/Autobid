using autobid.Domain.Auctions;
using autobid.Domain.Database;
using autobid.Domain.Users;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels;

public class AuctionListItemViewModel : ViewModelBase
{
	AuctionListItem _auctionListItem;
	public ReactiveCommand<Unit,Unit> GoToBidCommand { get; }
	public uint Id => _auctionListItem.Id;
	public string CarName => _auctionListItem.CarName;
	public int Year => _auctionListItem.Year;
	public decimal Bid => _auctionListItem.Bid;
	public string Username => _auctionListItem.Username;
	User _currentUser;

	public AuctionListItemViewModel(AuctionListItem auctionListItem, User currentUser)
	{
		_auctionListItem = auctionListItem;
		_currentUser = currentUser;
		GoToBidCommand = ReactiveCommand.CreateFromTask(GoToBidPage);
	}

	public bool IsUsersAuction() =>
		Username == _currentUser.Username;

	async Task GoToBidPage()
	{
		if (IsUsersAuction())
		{
			ShellViewModel.ChangeContent(new AuctionAcceptBidViewModel(new Bid(_currentUser, Bid)));
		}
		else
		{
			SqlAuctionRepository repo = new();
			Auction? auction = await repo.FindById(_auctionListItem.Id);

			if (auction != null)
				ShellViewModel.ChangeContent(new AuctionMakeBidViewModel(auction));
		}
	}
}
