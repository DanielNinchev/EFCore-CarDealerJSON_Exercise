using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CarDealer.Models
{
    public class Car
    {
        public Car()
        {
            this.Sales = new List<Sale>();
            this.PartCars = new List<PartCar>();
        }

        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }

        public virtual ICollection<PartCar> PartCars { get; set; } = new List<PartCar>();
    }
}