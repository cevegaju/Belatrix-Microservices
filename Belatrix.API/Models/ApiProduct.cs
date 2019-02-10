using System;

namespace Belatrix.API.Models
{
    public class ApiProduct
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool IsAvailable { get; set; }
    }
}
