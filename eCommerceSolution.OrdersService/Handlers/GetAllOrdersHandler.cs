using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Models.DTOs.GetAllOrders;
using eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;
using eCommerceSolution.OrdersService.Models.DTOs.HttpClient.Formats.ProductsMicroservice;
using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.OrdersService.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersRequest, GetAllOrdersResponse>
{

    private readonly ApplicationDbContext _dbContext;
    private readonly ProductsMicroserviceHttpClient _productsMicroserviceHttpClient;

    public GetAllOrdersHandler(ApplicationDbContext dbContext, ProductsMicroserviceHttpClient productsMicroserviceHttpClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _productsMicroserviceHttpClient = productsMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(productsMicroserviceHttpClient));
    }

    public async Task<GetAllOrdersResponse?> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
    {
        List<Order>? orders = await _dbContext.Orders.ToListAsync();
        if (orders == null)
        {
            return null;
        }
        List<GetOrderByIdResponse> orderResponses = new List<GetOrderByIdResponse>();

        foreach (var order in orders)
        {
            List<OrderItemResponse> orderItems = new List<OrderItemResponse>();
            foreach(OrderItem orderItem in order.OrderItems)
            {
                GetProductByIdResponse getProductByIdResponse = await _productsMicroserviceHttpClient.GetProductByIdAsync(orderItem.ProductId);
                if (getProductByIdResponse != null)
                {
                    orderItems.Add(new OrderItemResponse
                    {
                        ProductName = getProductByIdResponse.ProductName,
                        Category = getProductByIdResponse.Category,
                        TotalPrice = orderItem.TotalPrice,
                        UnitPrice = orderItem.UnitPrice,
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.Quantity,
                    });
                }
            }

            orderResponses.Add(new GetOrderByIdResponse
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalBill = order.TotalBill,
                OrderItems = orderItems,
                UserId = order.UserId,
            });
        }

        return new GetAllOrdersResponse
        {
            Orders = orderResponses
        };
    }
}
