using System;
using System.Collections.Generic;

namespace SHEIN_Clone_MVC.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Unpaid";

        public decimal TotalAmount { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; }
    }
}