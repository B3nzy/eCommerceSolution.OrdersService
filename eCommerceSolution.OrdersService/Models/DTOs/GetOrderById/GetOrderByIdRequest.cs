using MediatR;

namespace eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;

public class GetOrderByIdRequest :IRequest<GetOrderByIdResponse>
{
    public string OrderId { get; set; }
}
