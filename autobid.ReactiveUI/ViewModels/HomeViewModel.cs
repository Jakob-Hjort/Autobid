using autobid.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class HomeViewModel
	{
		User _user;
		public HomeViewModel(User user)
		{
			_user = user;
		}

		public HomeViewModel()
		{
			
		}
	}
}
