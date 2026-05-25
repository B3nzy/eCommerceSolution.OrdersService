namespace eCommerceSolution.OrdersService.Models.Entities;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
