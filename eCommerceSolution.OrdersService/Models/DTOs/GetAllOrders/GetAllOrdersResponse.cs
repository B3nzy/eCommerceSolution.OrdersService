using eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;

namespace eCommerceSolution.OrdersService.Models.DTOs.GetAllOrders;

public class GetAllOrdersResponse
{
    public List<GetOrderByIdResponse>? Orders { get; set; }
}
