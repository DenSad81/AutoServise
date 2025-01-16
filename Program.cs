using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        int qyantityDetails = 111;
        ListOfDetails detailList = new ListOfDetails(qyantityDetails);
        StoreHouse storeHouse = new StoreHouse(detailList);
        CarMaker carMaker = new CarMaker(detailList);
        List<Car> cars = carMaker.CreateCars();
        AutoService autoService = new AutoService(storeHouse, cars);

        autoService.Play();
    }
}

public class AutoService
{
    private int _money = 0;
    private int _fine1 = 300;
    private int _fine2 = 100;
    private StoreHouse _storeHouse;
    private Queue<Car> _cars;

    public AutoService(StoreHouse storeHouse, List<Car> cars)
    {
        _storeHouse = storeHouse;
        _cars = new Queue<Car>(cars);
    }

    public void Play()
    {
        const string Accept = "y";
        const string NotAccept = "n";

        foreach (var car in _cars)
            car.ShowStats();

        while (_cars.Count > 0)
        {
            ShowStats();
            _cars.Peek().ShowStats();

            if (Utils.ReadBool("Repair car? Y/N", Accept, NotAccept))
            {
                if (_cars.Peek().TryGetfFirstBrokenDetail(out Detail temp) == true)
                {
                    bool isChangeDetal = true;

                    while (isChangeDetal)
                    {
                        _cars.Peek().TryGetfFirstBrokenDetail(out Detail detail);
                        int detailId = detail.Id;
                        detail.ShowStats();

                        if (Utils.ReadBool("Change detal? Y/N", Accept, NotAccept))
                        {
                            if (ChangeDetail(detailId))
                                isChangeDetal = false;
                        }
                        else
                        {
                            DoNotChangeDetail(_cars.Peek().SummAllBrokenDetails);
                            isChangeDetal = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Broken details is not present");
                    DoNotChangeDetail(_fine2);
                }
            }
            else
            {
                DoNotChangeDetail(_fine1);
            }
        }

        ShowStats();
    }

    private bool ChangeDetail(int detailId)
    {
        if (_storeHouse.GetIfDetalPresentById(detailId, out DetailAndQuantity detailAndQuantity, out int pricePerDetal, out int pricePerChange))
        {
            if (TryChangeFirstBrokenDetail(_cars.Peek(), detailAndQuantity.Detail))
            {
                if (_storeHouse.TryBuyDetalById(detailId, out DetailAndQuantity detai))
                {
                    _money += (pricePerDetal + pricePerChange);

                    if (_cars.Peek().QuantityBrokenDetails == 0)
                    {
                        _cars.Peek().ShowStats();
                        _cars.Dequeue();

                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool TryChangeFirstBrokenDetail(Car car, Detail detail)
    {
        foreach (var detailAndQuality in car.GetCopyListOfDetails())
        {
            if (detailAndQuality.IsGoodQuality == false)
            {
                car.RemoveDetail(detailAndQuality);
                car.AddGoodDetail(new DetailAndQuality(detail, true));

                return true;
            }
        }

        return false;
    }

    private void DoNotChangeDetail(int fine)
    {
        _money -= fine;
        _cars.Dequeue();
    }

    private void ShowStats() =>
        Console.WriteLine($"Ballance autoService {_money}  Quantity car in enqique {_cars.Count}");
}

public class Detail
{
    public Detail(int type, int price = 0, int pricePerChange = 0)
    {
        Id = type;
        Price = price;
        PricePerChange = pricePerChange;
    }

    public int Id { get; }
    public int Price { get; }
    public int PricePerChange { get; }

    public virtual void ShowStats() =>
        Console.WriteLine($"Type of detail: {Id}  Price: {Price}  Price per change: {PricePerChange}  ");

    public Detail Clone() =>
        new Detail(Id, Price, PricePerChange);
}

public class DetailAndQuantity
{
    public DetailAndQuantity(Detail detail, int quantity)
    {
        Detail = detail;
        Quantity = quantity;
    }

    public Detail Detail { get; private set; }

    public int Quantity { get; private set; }

    public new DetailAndQuantity Clone() =>
        new DetailAndQuantity(Detail, Quantity);

    public bool DecreaseQuantity()
    {
        if (Quantity > 0)
        {
            Quantity--;

            return true;
        }

        return false;
    }

    public void ShowStats() =>
      Console.WriteLine($"Type of detail: {Detail.Id}  Price: {Detail.Price}  Price per change: {Detail.PricePerChange}  Quantity: {Quantity}");
}

public class DetailAndQuality
{
    public DetailAndQuality(Detail detail, bool quality)
    {
        Detail = detail;
        IsGoodQuality = quality;
    }

    public Detail Detail { get; private set; }

    public bool IsGoodQuality { get; private set; }

    public new DetailAndQuality Clone() =>
        new DetailAndQuality(Detail, IsGoodQuality);

    public void ShowStats() =>
    Console.WriteLine($"Type of detail: {Detail.Id}  Price: {Detail.Price}  Price per change: {Detail.PricePerChange}  Quality: {IsGoodQuality}");
}

public class ListOfDetails
{
    private List<Detail> _details = new List<Detail>();

    public ListOfDetails(int quantityDetails = 99, int minPrice = 50, int maxPrice = 100)
    {
        for (int i = 0; i < quantityDetails; i++)
        {
            int price = Utils.GenerateRandomInt(minPrice, maxPrice);
            int pricePerChange = Utils.GenerateRandomInt(minPrice, maxPrice);

            _details.Add(new Detail(i, price, pricePerChange));
        }
    }

    public Detail GetRandomDetail()
    {
        Detail detail = _details[Utils.GenerateRandomInt(0, _details.Count())];

        return detail.Clone();
    }

    public List<Detail> GetAllDetails()
    {
        List<Detail> details = new List<Detail>();

        foreach (var detail in _details)
            details.Add(detail.Clone());

        return details;
    }
}

public class StoreHouse
{
    private List<DetailAndQuantity> _detailsAndQuantity = new List<DetailAndQuantity>();

    public StoreHouse(ListOfDetails detailList, int quantityDetailsPerPosition = 11)
    {
        foreach (var detail in detailList.GetAllDetails())
            _detailsAndQuantity.Add(new DetailAndQuantity(detail, quantityDetailsPerPosition));
    }

    public bool GetIfDetalPresentById(int id, out DetailAndQuantity detailAndQuantity, out int pricePerDetal, out int pricePerChange)
    {
        pricePerDetal = 0;
        pricePerChange = 0;
        detailAndQuantity = null;

        for (int i = 0; i < _detailsAndQuantity.Count; i++)
        {
            if (_detailsAndQuantity[i].Detail.Id == id)
            {
                if (_detailsAndQuantity[i].Quantity > 0)
                {
                    pricePerDetal = _detailsAndQuantity[i].Detail.Price;
                    pricePerChange = _detailsAndQuantity[i].Detail.PricePerChange;
                    detailAndQuantity = _detailsAndQuantity[i].Clone();

                    return true;
                }

                Console.WriteLine("Detail is fiend, byt it quantity is zero");

                return false;
            }
        }

        Console.WriteLine("Detail is not fiend");

        return false;
    }

    public bool TryBuyDetalById(int id, out DetailAndQuantity detailAndQuantity)
    {
        detailAndQuantity = null;

        for (int i = 0; i < _detailsAndQuantity.Count; i++)
        {
            if (_detailsAndQuantity[i].Detail.Id == id)
            {
                if (_detailsAndQuantity[i].Quantity > 0)
                {
                    if (_detailsAndQuantity[i].DecreaseQuantity())
                    {
                        detailAndQuantity = _detailsAndQuantity[i].Clone();
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

public class CarMaker
{
    private int _quantityCars = 2;
    private int _minQuantityDetailsPerCar = 2;
    private int _maxQuantityDetailsPerCar = 6;
    private ListOfDetails _detailList;

    public CarMaker(ListOfDetails detailList)
    {
        _detailList = detailList;
    }

    public List<Car> CreateCars()
    {
        List<Car> cars = new List<Car>();

        for (int i = 0; i < _quantityCars; i++)
            cars.Add(CreateCar(i));

        return cars;
    }

    private Car CreateCar(int carId = 0)
    {
        int quantityDetails = Utils.GenerateRandomInt(_minQuantityDetailsPerCar, _maxQuantityDetailsPerCar);
        List<DetailAndQuality> details = new List<DetailAndQuality>();

        for (int i = 0; i < quantityDetails; i++)
        {
            var temp = new DetailAndQuality(_detailList.GetRandomDetail(), Utils.GenerateRandomBool());
            details.Add(temp);
        }

        return new Car(carId, details);
    }
}

public class Car
{
    private List<DetailAndQuality> _details;

    public Car(int carId, List<DetailAndQuality> details)
    {
        Id = carId;
        _details = details;
    }

    public int Id { get; }

    public int QuantityBrokenDetails
    {
        get
        {
            int quantityBrokenDetails = 0;
            foreach (var detail in _details)
            {
                if (detail.IsGoodQuality == false)
                    quantityBrokenDetails++;
            }

            return quantityBrokenDetails;
        }
    }

    public int SummAllBrokenDetails
    {
        get
        {
            int sum = 0;

            foreach (var brokenDetail in _details)
                sum += brokenDetail.Detail.Price;

            return sum;
        }
    }

    public List<DetailAndQuality> GetCopyListOfDetails()
    {
        List<DetailAndQuality> temp = new List<DetailAndQuality>();

        foreach (var item in _details)
            temp.Add(item.Clone());

        return temp;
    }

    public void RemoveDetail(DetailAndQuality detailAndQuality)
    {
        foreach (var detail in _details)
        {
            if (detail.Detail.Id == detailAndQuality.Detail.Id)
            {
                _details.Remove(detail);

                return;
            }
        }
    }

    public void AddGoodDetail(DetailAndQuality detail)
    => _details.Add(detail);

    public bool TryGetfFirstBrokenDetail(out Detail detailOut)
    {
        detailOut = null;

        foreach (var detail in _details)
        {
            if (detail.IsGoodQuality == false)
            {
                detailOut = detail.Detail.Clone();

                return true;
            }
        }

        Console.WriteLine("Broken detail is not present");

        return false;
    }

    public void ShowStats()
    {
        Console.WriteLine($"ID car: {Id}  Ballance car: ########");

        foreach (var detail in _details)
            detail.ShowStats();
    }
}

public static class Utils
{
    private static Random s_random = new Random();

    public static int GenerateRandomInt(int min, int max)
    {
        return s_random.Next(min, max);
    }

    public static bool GenerateRandomBool()
    {
        return s_random.Next(2) == 0;
    }

    public static bool ReadBool(string text = "", string yes = "y", string no = "n")
    {
        Console.Write(text + " ");
        bool isOut = false;
        bool isRun = true;

        while (isRun)
        {
            string stringFromConsole = (Console.ReadLine()).ToLower();

            if (stringFromConsole == yes)
            {
                isOut = true;
                isRun = false;
            }

            if (stringFromConsole == no)
            {
                isOut = false;
                isRun = false;
            }
        }

        return isOut;
    }
}