using System;
using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } // if you have Identity users

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        // Navigation
        public List<OrderItem> OrderItems { get; set; }
        public Invoice Invoice { get; set; }
    }
}