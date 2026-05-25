namespace eCommerceSolution.OrdersService.Models.Entities;

public class OrderItem
{
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
