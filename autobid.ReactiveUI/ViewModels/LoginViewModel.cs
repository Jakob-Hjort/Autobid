using autobid.Domain.Database;
using autobid.Domain.Users;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace autobid.ReactiveUI.ViewModels
{
	public class LoginViewModel : ViewModelBase
	{
		string _passWord = string.Empty;
		
		public string PassWord
		{
			get => _passWord; 
			set => this.RaiseAndSetIfChanged(ref _passWord, value, nameof(PassWord));
		}

		string _username = string.Empty;
		public string Username
		{
			get => _username;
			set => this.RaiseAndSetIfChanged(ref _username, value, nameof(Username));
		}

        UserRepository repository = new();

        public ReactiveCommand<Unit,Unit> GoToSignUpCommand { get; }
		public ReactiveCommand<Unit, Task> LoginCommand { get; }
		public LoginViewModel() : base("Login")
		{
			GoToSignUpCommand = ReactiveCommand.Create(GoToSignUpPage);
			LoginCommand = ReactiveCommand.Create(Login);
		}

		void GoToSignUpPage()
		{
			MainWindowViewModel.ChangeContent(new CreateUserViewModel());
		}

		async Task Login()
		{
			User? user = await repository.LoginAsync(Username, PassWord);

			if (user == null)
			{
				return;
			}

            MainWindowViewModel.ChangeContent(new ShellViewModel(user));
		}
	}
}
