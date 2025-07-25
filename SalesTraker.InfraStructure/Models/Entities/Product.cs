﻿namespace SalesTracker.InfraStructure.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }
    }

}
