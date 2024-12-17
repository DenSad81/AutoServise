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
        DetailList detailList = new DetailList(qyantityDetails);
        StoreHouse storeHouse = new StoreHouse(detailList);
        CarMaker carMaker = new CarMaker(detailList);

        AutoServis autoServis = new AutoServis(storeHouse, carMaker.CreateCars());
    }
}

class AutoServis
{
    private int _money = 0;
    private StoreHouse _storeHouse;
    private List<Car> _cars;


    public AutoServis(StoreHouse storeHouse, List<Car> cars)
    {
        _storeHouse = storeHouse;
        _cars = cars;
    }

    public void GetIndexOfNededDetail()
    {
        while (_cars.Count > 0)
        {
            if (Utils.ReadBool("Repair car? Y/N"))
            {
                bool isChangeDetal = true;
                while (isChangeDetal)
                {
                    Detail detail = _cars[0].GetCopyOfFirstBrokenDetail;
                    int nededDetailId = detail.Id;

                    if (Utils.ReadBool("Change detal? Y/N"))
                    {
                        if (_storeHouse.CheckIfDetalPresentById(nededDetailId, out int prisePerDetal, out int prisePerChange))
                        {
                            if (_cars[0].TruRepairFirstBrokenDetail(prisePerDetal, prisePerChange))
                            {
                                if (_storeHouse.TryBuyDetalById(nededDetailId))
                                {
                                    _money += (prisePerDetal + prisePerChange);

                                    if (_cars[0].QuantityBrokenDetails == 0)
                                    {
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
                int fine = 300;
                _cars[0].GetFine(fine);
                _money += fine;

                _cars.RemoveAt(0);
            }
        }
    }


    public void ShowStats()
    {
        Console.Clear();

        foreach (var position in _positions)
            position.ShowStats();

        Console.WriteLine($"Autoservis: {_money}");
    }
}

class Detail
{
    public int Id { get; /*private set;*/ }
    public int Prise { get; /*private set;*/ }
    public int PrisePerChange { get; /*private set;*/ }

    public Detail(int type, int prise = 0, int prisePerChange = 0)
    {
        Id = type;
        Prise = prise;
        PrisePerChange = prisePerChange;
    }

    public void ShowStats()
    {
        Console.Write($"Type: {Id}  Prise: {Prise}  Prise per change: {PrisePerChange}  ");
    }

    public Detail Clone()
    {
        return new Detail(Id, Prise, PrisePerChange);
    }
}
class DetailList
{
    private List<Detail> _details;
    private int _quantityDetails = 99;

    public DetailList(int quantityDetails = 99)
    {
        _quantityDetails = quantityDetails;

        for (int i = 0; i < _quantityDetails; i++)
        {
            int prise = Utils.GenerateRandomInt(50, 100);
            int prisePerChange = Utils.GenerateRandomInt(50, 100);

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
        Detail temp = _details[Utils.GenerateRandomInt(50, _details.Count())];
        return temp.Clone();
    }

    public List<Detail> GetAllDetails()
    {
        List<Detail> temp = new List<Detail>();

        foreach (var detail in _details)
            temp.Add(detail.Clone());

        return temp;
    }
}
class StoreHouse
{
    private List<Detail> _details;
    private List<int> _quantityEachDetails;
    private int _quantityDetailsPerPosition = 11;

    public StoreHouse(DetailList detailList)
    {
        _details = detailList.GetAllDetails();

        for (int i = 0; i < _details.Count; i++)
            _quantityEachDetails[i] = _quantityDetailsPerPosition;
    }

    public bool TryGetCopyDetalById(int id, out Detail detail)
    {
        detail = null;

        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].Id == id)
            {
                if (_quantityEachDetails[i] > 0)
                {
                    detail = _details[i].Clone();
                    _quantityEachDetails[i]--;
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryBuyDetalById(int id, out int prise)
    {
        prise = 0;

        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].Id == id)
            {
                if (_quantityEachDetails[i] > 0)
                {
                    prise = _details[i].Prise;
                    _quantityEachDetails[i]--;
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckIfDetalPresentById(int id, out int prisePerDetal, out int prisePerChange)
    {
        prisePerDetal = 0;
        prisePerChange = 0;

        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].Id == id)
            {
                if (_quantityEachDetails[i] > 0)
                {
                    prisePerDetal = _details[i].Prise;
                    prisePerChange = _details[i].PrisePerChange;
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryBuyDetalById(int id)
    {
        if (_quantityEachDetails[id] > 0)
        {
            _quantityEachDetails[id]--;
            return true;
        }

        return false;
    }


}
class CarMaker
{
    private DetailList _detailList;

    public CarMaker(DetailList detailList)
    {
        _detailList = detailList;
    }

    private Car CreateCar()
    {
        int minQuantityBrokenDetails = 3;
        int maxQuantityBrokenDetails = 6;
        int quantityDetails = Utils.GenerateRandomInt(minQuantityBrokenDetails, maxQuantityBrokenDetails);
        List<Detail> details = new List<Detail>();

        for (int i = 0; i < quantityDetails; i++)
            details.Add(_detailList.GetRandomDetail());

        return new Car(details);
    }

    public List<Car> CreateCars()
    {
        int quantityCars = 11;
        List<Car> cars = new List<Car>();

        for (int i = 0; i < quantityCars; i++)
            cars.Add(CreateCar());

        return cars;
    }
}

class Car
{
    private List<Detail> _brokenDetails;
    private int _money = 1000;

    public Car(List<Detail> details)
    {
        _brokenDetails = details;
    }

    public int QuantityBrokenDetails => _brokenDetails.Count;

    public Detail GetCopyOfFirstBrokenDetail
    {
        get
        {
            if (QuantityBrokenDetails == 0)
                return null;

            return _brokenDetails[0].Clone();
        }
    }

    public int SummAllBrokenDetails
    {
        get
        {
            int sum = 0;

            foreach (var brokenDetail in _brokenDetails)
                sum += brokenDetail.Prise;

            return sum;
        }
    }

    public bool TruRepairFirstBrokenDetail(int prisePerDetal, int prisePerChange)
    {
        int money = _money;

        if (money - (prisePerDetal + prisePerChange) >= 0)
        {
            _money -= (prisePerDetal + prisePerChange);
            _brokenDetails.RemoveAt(0);
            return true;
        }

        return false;
    }

    public void GetFine(int fine) =>
        _money -= fine;
}



static class Utils
{
    private static Random s_random = new Random();

    public static int ReadInt(string text = "", int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int number;
        Console.Write(text + " ");

        while (int.TryParse(Console.ReadLine(), out number) == false || number > maxValue || number < minValue)
            Console.Write(text + " ");

        return number;
    }

    public static int GenerateRandomInt(int min, int max)
    {
        return s_random.Next(min, max);
    }

    public static bool GenerateRandomBool()
    {
        return s_random.Next(2) == 0;
    }

    static public string ReadString(string text = "")
    {
        Console.Write(text + " ");
        string tempString = Console.ReadLine().ToLower();
        Console.WriteLine();
        return tempString;
    }

    static public bool ReadBool(string text = "", string yes = "y", string no = "n")
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

    static public void ShowArrayString(string[] inputArray)
    {
        for (int i = 0; i < inputArray.Length; i++)
        {
            Console.WriteLine(inputArray[i]);
        }
        Console.WriteLine();
    }

    static public void ShowDoubleArrayInt(int[,] array)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int k = 0; k < array.GetLength(1); k++)
            {
                Console.Write(array[i, k]);
                Console.Write(" ");
            }

            Console.WriteLine();
        }
    }

    static public void DeleteLastElementInArrayString(ref string[] inputArray)
    {
        if (inputArray.Length > 0)
        {
            string[] tempArray = new string[inputArray.Length - 1];

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = inputArray[i];
            }

            inputArray = tempArray;
        }
        else
        {
            Console.WriteLine("Невозможно удалить то чего нет");
        }
    }

    static public int[,] DeleteElementInDoubleArrayInt(int[,] array, int posOfElement)
    {
        if (posOfElement >= array.GetLength(0) || posOfElement < 0)
        {
            Console.WriteLine("Incorrect index");
            return null;
        }

        int[,] arrayTemp = new int[array.GetLength(0) - 1, array.GetLength(1)];

        int posInArray = 0;
        int posInTempArray = 0;

        while (posInArray < array.GetLength(0))
        {
            if (posInArray != posOfElement)
            {
                for (int t = 0; t < array.GetLength(1); t++)
                    arrayTemp[posInTempArray, t] = array[posInArray, t];

                posInTempArray++;
            }

            posInArray++;
        }

        return arrayTemp;
    }

    public static string[,] DeleteElementInDoubleArrayString(string[,] array, int positionOfElement)
    {
        if (positionOfElement >= array.GetLength(0) || positionOfElement < 0)
        {
            Console.WriteLine("Incorrect index");
            return null;
        }

        string[,] arrayNew = new string[array.GetLength(0) - 1, array.GetLength(1)];

        int positionInArray = 0;
        int positionInNewArray = 0;

        while (positionInArray < array.GetLength(0))
        {
            if (positionInArray != positionOfElement)
            {
                for (int t = 0; t < array.GetLength(1); t++)
                    arrayNew[positionInNewArray, t] = array[positionInArray, t];

                positionInNewArray++;
            }

            positionInArray++;
        }

        return arrayNew;
    }

    static public int SerchDataInArrayOfString(string[] inputArray, string inputData)
    {
        string tempString;
        string tempData;

        for (int i = 0; i < inputArray.Length; i++)
        {
            tempString = inputArray[i].ToLower();
            tempData = inputData.ToLower();

            if (tempString.Contains(tempData))
            {
                return i;
            }
        }

        return -1;
    }
}