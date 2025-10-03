using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using autobid.Domain.Users;
using autobid.Domain.Auctions;     // IAuctionHouse + AuctionHouse
using autobid.Domain.Vehicles;
using autobid.Domain.Database;

namespace autobid.ReactiveUI.ViewModels;

public sealed class SetForSaleViewModel : ViewModelBase
{
    // Valg til UI
    public enum VehicleCategory { Heavy, Personal }
    public enum HeavyKind { Truck, Bus }
    public enum PersonalKind { Private, Professional }

    // Kilder til ComboBox'ene (enum-værdierne)
    public VehicleCategory[] Categories { get; } =
        Enum.GetValues(typeof(VehicleCategory)).Cast<VehicleCategory>().ToArray();

    public HeavyKind[] HeavyKinds { get; } =
        Enum.GetValues(typeof(HeavyKind)).Cast<HeavyKind>().ToArray();

    public PersonalKind[] PersonalKinds { get; } =
        Enum.GetValues(typeof(PersonalKind)).Cast<PersonalKind>().ToArray();


    private readonly User _seller;
    private readonly IAuctionHouse _house;


    public SetForSaleViewModel(User user)
    : this(new autobid.Domain.Auctions.AuctionHouse(new SqlAuctionRepository()), user)
    {
    }
    public SetForSaleViewModel(IAuctionHouse house, User userID) : base("Create auction")
    {
        _house = house;
        _seller = userID;

        var currentYear = DateTime.Now.Year;
        Years = new ObservableCollection<int>(
            Enumerable.Range(0, 60).Select(i => currentYear - i)
        );

        //Years = new ObservableCollection<int>(Enumerable.Range(DateTime.Now.Year, -60).ToList());
        CloseAuctionDate = DateTime.Today.AddDays(7);

        // Defaults
        SelectedCategory = VehicleCategory.Heavy;
        SelectedHeavy = HeavyKind.Truck;
        SelectedPersonal = PersonalKind.Private;

        UpdateVisibilityFlags();
        Recalc();

        CreateAuctionCommand = ReactiveCommand.CreateFromTask(CreateAsync, this.WhenAnyValue(_ => _.CanCreate));
        CancelCommand = ReactiveCommand.Create(NavBack);
    }

    // ---------- fælles felter (venstre) ----------
    string? _name; public string? Name { get => _name; set { this.RaiseAndSetIfChanged(ref _name, value); Recalc(); } }
    int? _mileage; public int? Mileage { get => _mileage; set { this.RaiseAndSetIfChanged(ref _mileage, value); Recalc(); } }
    string? _reg; public string? RegNum { get => _reg; set { this.RaiseAndSetIfChanged(ref _reg, value); Recalc(); } }
    public ObservableCollection<int> Years { get; }

    public DateTimeOffset? CloseAuctionDateOffset
    {
        get => new DateTimeOffset(CloseAuctionDate);
        set { if (value != null) CloseAuctionDate = value.Value.Date; }
    }
    int? _year; public int? Year { get => _year; set { this.RaiseAndSetIfChanged(ref _year, value); Recalc(); } }
    decimal _startingBid; public decimal StartingBid { get => _startingBid; set { this.RaiseAndSetIfChanged(ref _startingBid, value); Recalc(); } }
    DateTime _close; public DateTime CloseAuctionDate { get => _close; set { this.RaiseAndSetIfChanged(ref _close, value); Recalc(); } }

    // ---------- valg: kategori + subtype ----------
    VehicleCategory _cat;
    public VehicleCategory SelectedCategory
    {
        get => _cat;
        set { this.RaiseAndSetIfChanged(ref _cat, value); UpdateVisibilityFlags(); Recalc(); }
    }

    HeavyKind _heavy;
    public HeavyKind SelectedHeavy
    {
        get => _heavy;
        set { this.RaiseAndSetIfChanged(ref _heavy, value); UpdateVisibilityFlags(); Recalc(); }
    }

    PersonalKind _personal;
    public PersonalKind SelectedPersonal
    {
        get => _personal;
        set { this.RaiseAndSetIfChanged(ref _personal, value); UpdateVisibilityFlags(); Recalc(); }
    }

    // RadioButton-binds (med setter = ændrer enum)
    public bool IsHeavy { get => SelectedCategory == VehicleCategory.Heavy; set { if (value) SelectedCategory = VehicleCategory.Heavy; } }
    public bool IsPersonal { get => SelectedCategory == VehicleCategory.Personal; set { if (value) SelectedCategory = VehicleCategory.Personal; } }
    public bool IsTruck { get => SelectedHeavy == HeavyKind.Truck; set { if (value) SelectedHeavy = HeavyKind.Truck; } }
    public bool IsBus { get => SelectedHeavy == HeavyKind.Bus; set { if (value) SelectedHeavy = HeavyKind.Bus; } }
    public bool IsPrivate { get => SelectedPersonal == PersonalKind.Private; set { if (value) SelectedPersonal = PersonalKind.Private; } }
    public bool IsProfessional { get => SelectedPersonal == PersonalKind.Professional; set { if (value) SelectedPersonal = PersonalKind.Professional; } }

    // Synligheds-bools til XAML
    bool _showHeavy; public bool ShowHeavy { get => _showHeavy; private set => this.RaiseAndSetIfChanged(ref _showHeavy, value); }
    bool _showTruck; public bool ShowTruck { get => _showTruck; private set => this.RaiseAndSetIfChanged(ref _showTruck, value); }
    bool _showBus; public bool ShowBus { get => _showBus; private set => this.RaiseAndSetIfChanged(ref _showBus, value); }
    bool _showPersonal; public bool ShowPersonal { get => _showPersonal; private set => this.RaiseAndSetIfChanged(ref _showPersonal, value); }
    bool _showPrivate; public bool ShowPrivate { get => _showPrivate; private set => this.RaiseAndSetIfChanged(ref _showPrivate, value); }
    bool _showProfessional; public bool ShowProfessional { get => _showProfessional; private set => this.RaiseAndSetIfChanged(ref _showProfessional, value); }

    void UpdateVisibilityFlags()
    {
        ShowHeavy = SelectedCategory == VehicleCategory.Heavy;
        ShowPersonal = !ShowHeavy;

        ShowTruck = ShowHeavy && SelectedHeavy == HeavyKind.Truck;
        ShowBus = ShowHeavy && SelectedHeavy == HeavyKind.Bus;

        ShowPrivate = ShowPersonal && SelectedPersonal == PersonalKind.Private;
        ShowProfessional = ShowPersonal && SelectedPersonal == PersonalKind.Professional;
    }

    // ---------- tekniske felter (bruges af flere typer) ----------
    double? _engineLiters; public double? EngineLiters { get => _engineLiters; set { this.RaiseAndSetIfChanged(ref _engineLiters, value); Recalc(); } }
    bool _towBar = true; public bool TowBar { get => _towBar; set => this.RaiseAndSetIfChanged(ref _towBar, value); }

    // Heavy fælles
    double? _heightMeter; public double? HeightMeter { get => _heightMeter; set => this.RaiseAndSetIfChanged(ref _heightMeter, value); }
    double? _length; public double? Length { get => _length; set => this.RaiseAndSetIfChanged(ref _length, value); }
    double? _weightKg; public double? WeightKg { get => _weightKg; set => this.RaiseAndSetIfChanged(ref _weightKg, value); }

    // Heavy: Truck
    int? _payloadKg; public int? PayloadKg { get => _payloadKg; set => this.RaiseAndSetIfChanged(ref _payloadKg, value); }

    // Heavy: Bus
    int? _busSeats; public int? BusSeatsAmount { get => _busSeats; set => this.RaiseAndSetIfChanged(ref _busSeats, value); }
    int? _busBeds; public int? BusBedsAmount { get => _busBeds; set => this.RaiseAndSetIfChanged(ref _busBeds, value); }
    bool _busHasToilet; public bool BusHasToilet { get => _busHasToilet; set => this.RaiseAndSetIfChanged(ref _busHasToilet, value); }

    // Personal fælles
    int? _seats; public int? SeatsAmount { get => _seats; set => this.RaiseAndSetIfChanged(ref _seats, value); }
    double? _trunkL; public double? TrunkL { get => _trunkL; set => this.RaiseAndSetIfChanged(ref _trunkL, value); }
    double? _trunkW; public double? TrunkW { get => _trunkW; set => this.RaiseAndSetIfChanged(ref _trunkW, value); }
    double? _trunkH; public double? TrunkH { get => _trunkH; set => this.RaiseAndSetIfChanged(ref _trunkH, value); }

    // Personal: Private
    bool _hasIsofix; public bool HasIsofix { get => _hasIsofix; set => this.RaiseAndSetIfChanged(ref _hasIsofix, value); }

    // Personal: Professional
    bool _hasSafetyBar; public bool HasSafetyBar { get => _hasSafetyBar; set => this.RaiseAndSetIfChanged(ref _hasSafetyBar, value); }
    int? _trailerKg; public int? TrailerCapacityKg { get => _trailerKg; set => this.RaiseAndSetIfChanged(ref _trailerKg, value); }

    // ---------- validation ----------
    bool _canCreate; public bool CanCreate { get => _canCreate; private set => this.RaiseAndSetIfChanged(ref _canCreate, value); }

    void Recalc()
    {
        (double minL, double maxL) range = SelectedCategory == VehicleCategory.Heavy
            ? (4.2, 15.0)
            : (0.7, 10.0);

        var litersOk = EngineLiters is null || (EngineLiters >= range.minL && EngineLiters <= range.maxL);

        var baseOk = !string.IsNullOrWhiteSpace(Name)
                  && !string.IsNullOrWhiteSpace(RegNum)
                  && Year is not null
                  && StartingBid > 0m
                  && CloseAuctionDate > DateTime.Today
                  && litersOk;

        var typeOk = (ShowTruck || ShowBus || ShowPrivate || ShowProfessional);

        CanCreate = baseOk && typeOk;
    }

    // ---------- commands ----------
    public ReactiveCommand<Unit, Unit> CreateAuctionCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    async Task CreateAsync()
    {
        var vehicle = BuildVehicle();
        await _house.SetForSale(vehicle, _seller, StartingBid, (DateTimeOffset)this.CloseAuctionDateOffset); // returnerer auctionId
        MainWindowViewModel.ChangeContent(new HomeViewModel(_seller));
        await Task.CompletedTask;
    }

    Vehicle BuildVehicle()
    {
        var name = Name!.Trim();
        var km = Mileage ?? 0;
        var reg = RegNum!.Trim();    // Vehicle vil selv validere regex LLDDDDD
        var year = Year!.Value;
        // default afhænger af kategori (bare for UX – domæneklasserne tjekker selv ranges)
        var liters = EngineLiters ?? (SelectedCategory == VehicleCategory.Heavy ? 6.0 : 1.6);

        if (ShowHeavy)
        {
            if (ShowTruck)
            {
                var t = new Truck(id: 0, name, km, reg, year, liters, TowBar);

                if (HeightMeter is double hm) t.HeightMeter = hm;
                if (Length is double le) t.Length = le;
                if (WeightKg is double wg) t.WeightKg = wg;

                if (PayloadKg is int pl) t.PayloadKg = pl;

                return t;
            }
            else // Bus
            {
                var b = new Bus(id: 0, name, km, reg, year, liters, TowBar);

                if (HeightMeter is double hm) b.HeightMeter = hm;
                if (Length is double le) b.Length = le;
                if (WeightKg is double wg) b.WeightKg = wg;

                if (BusSeatsAmount is int s) b.SeatsAmount = s;
                if (BusBedsAmount is int bd) b.BedsAmount = bd;
                b.HasToilet = BusHasToilet;

                return b;
            }
        }
        else // Personal
        {
            PersonalCar car = ShowPrivate
                ? new PrivatePersonalCar(id: 0, name, km, reg, year, liters, TowBar)
                : new ProfessionalPersonalCar(id: 0, name, km, reg, year, liters, TowBar, TrailerCapacityKg ?? 0);

            if (SeatsAmount is int seats) car.SeatsAmount = seats;

            // Sæt trunk kun hvis alle tre værdier er udfyldt
            if (TrunkL is double l && TrunkW is double w && TrunkH is double h)
                car.Trunk = (l, w, h);

            if (car is PrivatePersonalCar priv) priv.HasIsofix = HasIsofix;
            if (car is ProfessionalPersonalCar prof) prof.HasSafetyBar = HasSafetyBar;

            return car;
        }
    }


    void NavBack() => MainWindowViewModel.ChangeContent(new HomeViewModel(_seller));
}
