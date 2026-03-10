using System;
using System.Collections.Generic;

namespace QuatTran.Infrastructure;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public int? ShipperId { get; set; }

    public string? ShippingStatus { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Shipper? Shipper { get; set; }

    public virtual User User { get; set; } = null!;
}
