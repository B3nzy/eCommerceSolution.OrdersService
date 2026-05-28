using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;
using eCommerceSolution.OrdersService.Models.DTOs.HttpClient.Formats.ProductsMicroservice;
using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.OrdersService.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdRequest, GetOrderByIdResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ProductsMicroserviceHttpClient _productsMicroserviceHttpClient;

    public GetOrderByIdHandler(ApplicationDbContext dbContext, ProductsMicroserviceHttpClient productsMicroserviceHttpClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _productsMicroserviceHttpClient = productsMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(productsMicroserviceHttpClient));
    }

    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
    {
        Order? order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == Guid.Parse(request.OrderId));

        if (order == null)
        {
            return null;
        }

        List<OrderItemResponse> orderItems = new List<OrderItemResponse>();

        foreach (var item in order.OrderItems)
        {
            GetProductByIdResponse getProductByIdResponse = await _productsMicroserviceHttpClient.GetProductByIdAsync(item.ProductId);
            if (getProductByIdResponse != null)
            {
                orderItems.Add(new OrderItemResponse
                {
                    ProductName = getProductByIdResponse.ProductName,
                    Category = getProductByIdResponse.Category,
                    TotalPrice = item.TotalPrice,
                    UnitPrice = item.UnitPrice,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
            }

        }


        return new GetOrderByIdResponse
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            OrderItems = orderItems,
            TotalBill = order.TotalBill,
            UserId = order.UserId
        };
    }
}
