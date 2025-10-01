using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

using autobid.Domain.Auctions;
using autobid.Domain.Database;   // SqlAuctionRepository (gør klassen public hvis den er internal)
using autobid.Domain.Users;
using autobid.Domain.Vehicles;

namespace autobid.ReactiveUI.ViewModels;

public sealed class SetForSaleViewModel : ViewModelBase
{
    private readonly User _seller;

    public SetForSaleViewModel(User seller)
    {
        _seller = seller;

        Years = new ObservableCollection<int>(Enumerable.Range(DateTime.Now.Year, -60).ToList());
        CloseAuctionDate = DateTime.Today.AddDays(7);
        SelectedVehicleType = "Truck";   // mockup'et viser Truck

        CreateAuctionCommand = ReactiveCommand.CreateFromTask(CreateAsync, this.WhenAnyValue(_ => _.CanCreate));
        CancelCommand = ReactiveCommand.Create(NavBack);
    }

    // -------- venstre side --------
    string? _name; public string? Name { get => _name; set { this.RaiseAndSetIfChanged(ref _name, value); Recalc(); } }
    int? _mileage; public int? Mileage { get => _mileage; set { this.RaiseAndSetIfChanged(ref _mileage, value); Recalc(); } }
    string? _regNum; public string? RegNum { get => _regNum; set { this.RaiseAndSetIfChanged(ref _regNum, value); Recalc(); } }
    public ObservableCollection<int> Years { get; }
    int? _year; public int? Year { get => _year; set { this.RaiseAndSetIfChanged(ref _year, value); Recalc(); } }
    decimal _startingBid; public decimal StartingBid { get => _startingBid; set { this.RaiseAndSetIfChanged(ref _startingBid, value); Recalc(); } }
    DateTime _close; public DateTime CloseAuctionDate { get => _close; set { this.RaiseAndSetIfChanged(ref _close, value); Recalc(); } }

    // -------- højre side (truck felter + type) --------
    public ObservableCollection<string> VehicleTypes { get; } =
        new(new[] { "Truck", "Car", "Van", "Motorcycle" });
    string _vt; public string SelectedVehicleType { get => _vt; set => this.RaiseAndSetIfChanged(ref _vt, value); }
    double? _height; public double? Height { get => _height; set => this.RaiseAndSetIfChanged(ref _height, value); }
    double? _length; public double? Length { get => _length; set => this.RaiseAndSetIfChanged(ref _length, value); }
    double? _weight; public double? Weight { get => _weight; set => this.RaiseAndSetIfChanged(ref _weight, value); }
    double? _engineLiters; public double? EngineLiters { get => _engineLiters; set => this.RaiseAndSetIfChanged(ref _engineLiters, value); }
    bool _towBar = true; public bool TowBar { get => _towBar; set => this.RaiseAndSetIfChanged(ref _towBar, value); }

    // -------- validation/commands --------
    bool _canCreate; public bool CanCreate { get => _canCreate; private set => this.RaiseAndSetIfChanged(ref _canCreate, value); }
    void Recalc()
        => CanCreate = !string.IsNullOrWhiteSpace(Name)
                       && !string.IsNullOrWhiteSpace(RegNum)
                       && Year is not null
                       && StartingBid > 0
                       && CloseAuctionDate > DateTime.Today;

    public ReactiveCommand<Unit, Unit> CreateAuctionCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    async Task CreateAsync()
    {
        // 1) bygg vehicle (Truck – passer til jeres domæneklasse)
        Vehicle vehicle = BuildVehicle();

        // 2) kør forretningsreglen via AuctionHouse
        var repo = new SqlAuctionRepository();                    // sørg for public class
        var house = new AuctionHouse(repo);                        // IAuctionHouse-impl
        var id = house.SætTilSalg(vehicle, _seller, StartingBid);

        // TODO: CloseAuctionDate kan gemmes når I udvider schema – ignoreres her

        // 3) tilbage til Home
        MainWindowViewModel.ChangeContent(new HomeViewModel(_seller));
    }

    Vehicle BuildVehicle()
    {
        var name = Name!.Trim();
        var km = Mileage ?? 0;
        var reg = RegNum!.Trim();
        var year = Year!.Value;

        switch (SelectedVehicleType)
        {
            case "Truck":
            default:
                var liters = EngineLiters ?? 5.0;
                var t = new Truck(id: 0, name: name, km: km, regNo: reg, year: year,
                                  engineLiters: liters, towHitch: TowBar);
                if (Height is not null) t.HeightMeter = Height.Value;
                if (Length is not null) t.Length = Length.Value;
                if (Weight is not null) t.WeightKg = Weight.Value;
                return t;
        }
    }

    void NavBack() => MainWindowViewModel.ChangeContent(new HomeViewModel(_seller));
}
