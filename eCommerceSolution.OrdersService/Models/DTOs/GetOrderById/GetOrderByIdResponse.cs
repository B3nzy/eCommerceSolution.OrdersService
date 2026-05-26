using eCommerceSolution.OrdersService.Models.Entities;

namespace eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;

public class GetOrderByIdResponse
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
