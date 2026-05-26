using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;

namespace eCommerceSolution.OrdersService.Models.DTOs;

public class CreateOrderRequest : IRequest<CreateOrderResponse>
{
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalBill { get; set; }
    public List<OrderItem>? OrderItems { get; set; }
}
