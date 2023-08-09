using System;

namespace LoginFinal.Models
{
    public class Orders
    {   
        public int Id { get; set; }
        public string OrderDescription { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int IsActive { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }

    }
}
