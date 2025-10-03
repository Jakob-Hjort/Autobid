using autobid.Domain.Database;
using autobid.Domain.Security;
using autobid.Domain.Users;
using autobid.Domain.Vehicles;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
namespace autobid.ReactiveUI.ViewModels;

public class CreateUserViewModel : ViewModelBase
{
	bool _isCorporate = false;
	public bool IsCorporate
	{
		get => _isCorporate;
		set
		{
			if ( value)
			{
				CPR = string.Empty;
				Balance = 0;
			}
			else
			{
				CVR = string.Empty;
				Credit = 0;
			}
			this.RaiseAndSetIfChanged(ref _isCorporate, value, nameof(IsCorporate));
		}
	}

	string _username = string.Empty;
	public string Username
	{
		get => _username;
		set => this.RaiseAndSetIfChanged(ref _username, value, nameof(Username));
	}
	
	string _password = string.Empty;
	public string Password
	{
		get => _password;
		set => this.RaiseAndSetIfChanged(ref _password, value, nameof(Password));
	}

	string _passwordRepeat = string.Empty;
	public string PasswordRepeat
	{
		get => _passwordRepeat;
		set => this.RaiseAndSetIfChanged(ref _passwordRepeat, value, nameof(PasswordRepeat));
	}

	string _cpr = string.Empty;
	public string CPR
	{
		get => this._cpr;
		set => this.RaiseAndSetIfChanged(ref _cpr, value, nameof(CPR));
	}

	string _cvr = string.Empty;
	public string CVR
	{
		get => _cvr;
		set => this.RaiseAndSetIfChanged(ref _cvr, value, nameof(CVR));
	}

	int _credit = 0;
	public int Credit
	{
		get => _credit;
		set => this.RaiseAndSetIfChanged(ref _credit, value, nameof(Credit));
	}

	int _balance = 0;
	public int Balance
	{
		get => _balance;
		set => this.RaiseAndSetIfChanged(ref _balance, value, nameof(Balance));
	}

	UserRepository repository = new();
	public ReactiveCommand<Unit, Task> CreateUserCommand { get;}
	public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

	public CreateUserViewModel() : base("Create user")
	{
		CreateUserCommand = ReactiveCommand.Create(CreateUser);
		GoBackCommand = ReactiveCommand.Create(GoBack);

	}

	private void GoBack()
	{
		MainWindowViewModel.ChangeContent(new LoginViewModel());
	}

	private async Task CreateUser()
	{
		if ((Password != PasswordRepeat && Password.Length < User.MinPasswordLength && 
			Password.Length > User.MaxPasswordLength) || await repository.UsernameExistsAsync(Username))
		{
			return;
		}

		string hash = new Hasher().Hash(Password);

        User created;

        if (IsCorporate)
		{
            created  = new CorporateCustomer(0, Username, hash, CVR, Credit, Balance);
            created.Id = Convert.ToUInt32(await repository.Add((CorporateCustomer)created));
        }
		else
		{
            created = new PrivateCustomer(0, Username, hash, CPR, Balance);
            created.Id = Convert.ToUInt32(await repository.Add((PrivateCustomer)created));
        }
        


        MainWindowViewModel.ChangeContent(new ShellViewModel(created));
    }
}
