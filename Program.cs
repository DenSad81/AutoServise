﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        int qyantityDetails = 111;
        DetailList detailList = new DetailList(qyantityDetails);
        StoreHouse storeHouse = new StoreHouse(detailList);

        CarMaker carMaker = new CarMaker(detailList);


        // storeHouse.ShowStats();

        List<Car> cars = carMaker.CreateCars();


        foreach (var car in cars)
            car.ShowStats();



        AutoServis autoServis = new AutoServis(storeHouse, cars);
        autoServis.Play();
    }
}

public class AutoServis
{
    private int _money = 0;
    private int _fine = 300;
    private StoreHouse _storeHouse;
    private List<Car> _cars;

    public AutoServis(StoreHouse storeHouse, List<Car> cars)
    {
        _storeHouse = storeHouse;
        _cars = cars;
    }

    public void Play()
    {
        while (_cars.Count > 0)
        {
            this.ShowStats();
            _cars[0].ShowStats();

            if (Utils.ReadBool("Repair car? Y/N"))
            {
                bool isChangeDetal = true;

                while (isChangeDetal)
                {
                    Detail detail = _cars[0].GetCopyOfFirstBrokenDetail;
                    int detailId = detail.Id;
                    detail.ShowStats();

                    if (Utils.ReadBool("Change detal? Y/N"))
                    {
                        if (_storeHouse.CheckIfDetalPresentById(detailId, out int prisePerDetal, out int prisePerChange))
                        {
                            if (_cars[0].TruRepairFirstBrokenDetail(prisePerDetal, prisePerChange))
                            {
                                if (_storeHouse.TryBuyDetalById(detailId))
                                {
                                    _money += (prisePerDetal + prisePerChange);

                                    if (_cars[0].QuantityBrokenDetails == 0)
                                    {
                                        _cars[0].ShowStats();
                                        _cars.RemoveAt(0);
                                        isChangeDetal = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int fine = _cars[0].SummAllBrokenDetails;
                        _cars[0].GetFine(fine);
                        _money += fine;

                        _cars.RemoveAt(0);
                        isChangeDetal = false;
                    }
                }
            }
            else
            {
                _cars[0].GetFine(_fine);
                _money += _fine;

                _cars.RemoveAt(0);
            }
        }
    }

    private void ShowStats() =>
        Console.WriteLine($"Ballance autoservis {_money}  Quantity car in enqique {_cars.Count}");
}

public class Detail
{
    public Detail(int type, int prise = 0, int prisePerChange = 0)
    {
        Id = type;
        Prise = prise;
        PrisePerChange = prisePerChange;
    }

    public int Id { get; }
    public int Prise { get; }
    public int PrisePerChange { get; }

    public virtual void ShowStats() =>
        Console.WriteLine($"Type of detail: {Id}  Prise: {Prise}  Prise per change: {PrisePerChange}  ");

    public Detail Clone() =>
        new Detail(Id, Prise, PrisePerChange);
}

public class DetailAndQuantity : Detail
{
    public DetailAndQuantity(int type, int prise, int prisePerChange, int quantity) : base(type, prise, prisePerChange)
    {
        Quantity = quantity;
    }

    public DetailAndQuantity(Detail detail, int quantity) : base(detail.Id, detail.Prise, detail.PrisePerChange)
    {
        Quantity = quantity;
    }

    public int Quantity { get; private set; }

    public DetailAndQuantity Clone() =>
        new DetailAndQuantity(Id, Prise, PrisePerChange, Quantity);

    public bool DecreaseQuantity()
    {
        if (Quantity > 0)
        {
            Quantity--;
            return true;
        }

        return false;
    }

    public override void ShowStats() =>
      Console.WriteLine($"Type of detail: {Id}  Prise: {Prise}  Prise per change: {PrisePerChange}  Quantity: {Quantity}");
}

public class DetailAndQuality : Detail
{
    public DetailAndQuality(int type, int prise, int prisePerChange, bool quality) : base(type, prise, prisePerChange)
    {
        Quality = quality;
    }

    public DetailAndQuality(Detail detail, bool quality) : base(detail.Id, detail.Prise, detail.PrisePerChange)
    {
        Quality = quality;
    }

    public bool Quality { get; private set; }

    public DetailAndQuality Clone() =>
        new DetailAndQuality(Id, Prise, PrisePerChange, Quality);

    public override void ShowStats() =>
    Console.WriteLine($"Type of detail: {Id}  Prise: {Prise}  Prise per change: {PrisePerChange}  Quality: {Quality}");

    public void RepeirDetal()
        => Quality = true;
}




public class DetailList
{
    private List<Detail> _details = new List<Detail>();

    public DetailList(int quantityDetails = 99, int minPrice = 50, int maxPrice = 100)
    {
        for (int i = 0; i < quantityDetails; i++)
        {
            int prise = Utils.GenerateRandomInt(minPrice, maxPrice);
            int prisePerChange = Utils.GenerateRandomInt(minPrice, maxPrice);

            _details.Add(new Detail(i, prise, prisePerChange));
        }
    }

    public void ShowDetails()
    {
        foreach (var detail in _details)
            detail.ShowStats();
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

//public class StoreHouse
//{
//    private List<Detail> _details;
//    private List<int> _quantityEachDetails;

//    public StoreHouse(DetailList detailList, int quantityDetailsPerPosition = 11)
//    {
//        _details = detailList.GetAllDetails();
//        _quantityEachDetails = new List<int>(_details.Count);

//        for (int i = 0; i < _details.Count; i++)
//            _quantityEachDetails.Add(quantityDetailsPerPosition);
//    }

//    public bool CheckIfDetalPresentById(int id, out int prisePerDetal, out int prisePerChange)
//    {
//        prisePerDetal = 0;
//        prisePerChange = 0;

//        for (int i = 0; i < _details.Count; i++)
//        {
//            if (_details[i].Id == id)
//            {
//                if (_quantityEachDetails[i] > 0)
//                {
//                    prisePerDetal = _details[i].Prise;
//                    prisePerChange = _details[i].PrisePerChange;
//                    return true;
//                }
//            }
//        }

//        return false;
//    }

//    public bool TryBuyDetalById(int id)
//    {
//        if (_quantityEachDetails[id] > 0)
//        {
//            _quantityEachDetails[id]--;
//            return true;
//        }

//        return false;
//    }
//}

public class StoreHouse
{
    private List<DetailAndQuantity> _details = new List<DetailAndQuantity>();

    public StoreHouse(DetailList detailList, int quantityDetailsPerPosition = 11)
    {
        foreach (var detail in detailList.GetAllDetails())
            _details.Add(new DetailAndQuantity(detail, quantityDetailsPerPosition));
    }

    public bool CheckIfDetalPresentById(int id, out int prisePerDetal, out int prisePerChange)
    {
        prisePerDetal = 0;
        prisePerChange = 0;

        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].Id == id)
            {
                if (_details[i].Quantity > 0)
                {
                    prisePerDetal = _details[i].Prise;
                    prisePerChange = _details[i].PrisePerChange;
                    return true;
                }
                else
                {
                    Console.WriteLine("Detail is fiend, byt it quantity is zero");
                    return false;
                }
            }
        }

        Console.WriteLine("Detail is not fiend");
        return false;
    }

    public bool TryBuyDetalById(int id)
    {
        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].Id == id)
            {
                if (_details[i].Quantity > 0)
                {
                    if (_details[i].DecreaseQuantity())
                        return true;
                }
            }
        }

        return false;
    }

    public void ShowStats()
    {
        foreach (var item in _details)
            item.ShowStats();
    }
}






//public class CarMaker
//{
//    private int _quantityCars = 11;
//    private int _minQuantityBrokenDetailsPerCar = 2;
//    private int _maxQuantityBrokenDetailsPerCar = 6;
//    private DetailList _detailList;

//    public CarMaker(DetailList detailList)
//    {
//        _detailList = detailList;
//    }

//    public List<Car> CreateCars()
//    {
//        List<Car> cars = new List<Car>();

//        for (int i = 0; i < _quantityCars; i++)
//            cars.Add(CreateCar(i));

//        return cars;
//    }

//    private Car CreateCar(int id)
//    {
//        int quantityDetails = Utils.GenerateRandomInt(_minQuantityBrokenDetailsPerCar, _maxQuantityBrokenDetailsPerCar);
//        List<Detail> details = new List<Detail>();

//        for (int i = 0; i < quantityDetails; i++)
//            details.Add(_detailList.GetRandomDetail());

//        return new Car(id, details);
//    }
//}

public class CarMaker
{
    private int _quantityCars = 3;
    private int _minQuantityBrokenDetailsPerCar = 2;
    private int _maxQuantityBrokenDetailsPerCar = 6;
    private DetailList _detailList;

    public CarMaker(DetailList detailList)
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
        int quantityDetails = Utils.GenerateRandomInt(_minQuantityBrokenDetailsPerCar, _maxQuantityBrokenDetailsPerCar);
        List<DetailAndQuality> details = new List<DetailAndQuality>();

        for (int i = 0; i < quantityDetails; i++)
        {
            var temp = new DetailAndQuality(_detailList.GetRandomDetail(), Utils.GenerateRandomBool());
            details.Add(temp);
        }

        return new Car(carId, details);
    }
}

//public class Car
//{
//    private List<Detail> _brokenDetails;
//    private int _money = 1000;

//    public Car(int id, List<Detail> details)
//    {
//        Id = id;
//        _brokenDetails = details;
//    }

//    public int Id { get; }

//    public int QuantityBrokenDetails => _brokenDetails.Count;

//    public Detail GetCopyOfFirstBrokenDetail
//    {
//        get
//        {
//            if (QuantityBrokenDetails == 0)
//                return null;

//            return _brokenDetails[0].Clone();
//        }
//    }

//    public int SummAllBrokenDetails
//    {
//        get
//        {
//            int sum = 0;

//            foreach (var brokenDetail in _brokenDetails)
//                sum += brokenDetail.Prise;

//            return sum;
//        }
//    }

//    public bool TruRepairFirstBrokenDetail(int prisePerDetal, int prisePerChange)
//    {
//        int money = _money;

//        if (money - (prisePerDetal + prisePerChange) >= 0)
//        {
//            _money -= (prisePerDetal + prisePerChange);
//            _brokenDetails.RemoveAt(0);
//            return true;
//        }

//        return false;
//    }

//    public void GetFine(int fine) =>
//        _money -= fine;

//    public void ShowStats()
//    {
//        Console.WriteLine($"ID car: {Id}  Ballance: {_money}");

//        foreach (var brokenDetail in _brokenDetails)
//            brokenDetail.ShowStats();
//    }
//}



public class Car
{
    private List<DetailAndQuality> _details;
    private int _money = 1000;

    public Car(int carId, List<DetailAndQuality> details)
    {
        CarId = carId;
        _details = details;
    }

    public int CarId { get; }

    public int QuantityBrokenDetails
    {
        get
        {
            int quantityBrokenDetails = 0;
            foreach (var detail in _details)
            {
                if (detail.Quality == false)
                    quantityBrokenDetails++;
            }

            return quantityBrokenDetails;
        }
    }

    public Detail GetCopyOfFirstBrokenDetail
    {
        get
        {
            foreach (var detail in _details)
            {
                if (detail.Quality == false)
                    return detail.Clone();
            }

            Console.WriteLine("Broken detail is not present");
            return null;
        }
    }

    public int SummAllBrokenDetails
    {
        get
        {
            int sum = 0;

            foreach (var brokenDetail in _details)
                sum += brokenDetail.Prise;

            return sum;
        }
    }

    public bool TruRepairFirstBrokenDetail(int prisePerDetal, int prisePerChange)
    {
        int money = _money;

        foreach (var detail in _details)
        {
            if (detail.Quality == false)
            {
                if (money - (prisePerDetal + prisePerChange) >= 0)
                {
                    _money -= (prisePerDetal + prisePerChange);
                    detail.RepeirDetal();
                    return true;
                }
                else
                {
                    Console.WriteLine("Money not enaf for repering first broken detail");
                    return false;
                }
            }
        }

        return false;
    }

    public void GetFine(int fine) =>
        _money -= fine;

    public void ShowStats()
    {
        Console.WriteLine($"ID car: {CarId}  Ballance car: {_money}");

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