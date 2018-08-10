using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.App.Dtos.Export
{

    public class CarPriceAndDiscountExportDto
    {
        public double Discount { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
