using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Results";

        public static void Main(string[] args)
        {
            CarDealerContext db = new CarDealerContext();
            //Configure();

            ResetDatabase(db);

            string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            string salesJson = File.ReadAllText("../../../Datasets/sales.json");

            string input1 = ImportSuppliers(db, suppliersJson);
            string input2 = ImportParts(db, partsJson);
            string input3 = ImportCars(db, carsJson);
            string input4 = ImportCustomers(db, customersJson);
            string input5 = ImportSales(db, salesJson);

            string json = GetCarsWithTheirListOfParts(db);

            if (!Directory.Exists(ResultDirectoryPath))
            {
                Directory.CreateDirectory(ResultDirectoryPath);
            }

            File.WriteAllText(ResultDirectoryPath + "/cars-and-parts.json", json);

            Console.WriteLine(input1);
            Console.WriteLine(input2);
            Console.WriteLine(input3);
            Console.WriteLine(input4);
            Console.WriteLine(input5);

        }

        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");

            db.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }

        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            Mapper.Configuration.AssertConfigurationIsValid();
        }

        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p => p.SupplierId <= 31)
                .ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}.";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDtos = JsonConvert.DeserializeObject<ImportCarDTO[]>(inputJson);

            List<Car> cars = new List<Car>();

            foreach (var carDto in carsDtos)
            {
                var parts = carDto.PartsId.Select(p => p.Id).Distinct().ToArray();

                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance,
                    PartCars = parts.Select(id => new PartCar
                    {
                        PartId = id
                    })
                    .ToArray()
                };

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        //Problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    name = c.Name,
                    birthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    isYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make.Equals("Toyota"))
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    id = c.Id,
                    make = c.Make,
                    model = c.Model,
                    travelledDistance = c.TravelledDistance
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    partsCount = s.Parts.Count()
                })
                .ToArray();

            string json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithTheirParts = context.Cars
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Select(c => new
                {
                    car = new CarsWithTheirPartsDTO
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance,
                        Parts = c.PartCars.Select(p => new PartsOfCarsDTO
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price
                        })
                        .ToArray()
                    }
                })
                //.ProjectTo<CarsWithTheirPartsDTO>()
                .ToArray();

            string json = JsonConvert.SerializeObject(carsWithTheirParts, Formatting.Indented);

            return json;
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var result = context.Sales
                .Select(s => new
                {
                    fullName = s.Customer.Name,
                    boughtCars = s.Customer.Sales.Count,
                    spentMoney = s.Car.PartCars
                    .Where(pc => pc.CarId == s.CarId)
                    .Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(s => s.spentMoney)
                .ThenByDescending(s => s.boughtCars)
                .ToArray();            

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var result = context.Sales
                .Select(s => new
                {
                    car = new CarsWithTheirPartsDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                    },
                    customerName = s.Customer.Name,
                    s.Discount,
                    price = s.Car.PartCars.Sum(p => p.Part.Price),
                    priceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) -
                                        s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json;
        }       
    }
}