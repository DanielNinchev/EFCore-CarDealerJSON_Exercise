using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //this.CreateMap<Part, PartCar>()
            //    .ForMember(x => x.PartId, y => y.MapFrom(x => x.Id));

            //this.CreateMap<Car, PartCar>()
            //    .ForMember(x => x.CarId, y => y.MapFrom(x => x.Id));

            this.CreateMap<Part, PartsOfCarsDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.Price, y => y.MapFrom(x => x.Price));

            this.CreateMap<Car, CarsWithTheirPartsDTO>()
                .ForMember(x => x.Parts, y => y.MapFrom(x => x.PartCars));
        }
    }
}
