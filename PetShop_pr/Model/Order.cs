using System;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "pending"; // pending, completed, cancelled
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
