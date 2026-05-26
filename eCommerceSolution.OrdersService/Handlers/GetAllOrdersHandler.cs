using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.Models.DTOs.GetAllOrders;
using eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;
using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.OrdersService.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersRequest, GetAllOrdersResponse>
{

    private readonly ApplicationDbContext _dbContext;

    public GetAllOrdersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAllOrdersResponse?> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
    {
        List<Order>? orders = await _dbContext.Orders.ToListAsync();
        if (orders == null)
        {
            return null;
        }
        List<GetOrderByIdResponse> orderResponses = orders.Select(o =>
        {
            return new GetOrderByIdResponse
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                OrderItems = o.OrderItems,
                TotalBill = o.TotalBill,
                UserId = o.UserId
            };
        }).ToList();
        GetAllOrdersResponse getAllOrdersResponse = new GetAllOrdersResponse { Orders = orderResponses };
        return getAllOrdersResponse;
    }
}
