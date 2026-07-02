using eCommerce.Microservices.Events.Order;
using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Models.DTOs.CreateOrder;
using eCommerceSolution.OrdersService.Models.Entities;
using MassTransit;
using MediatR;

namespace eCommerceSolution.OrdersService.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UsersMicroserviceHttpClient _usersMicroserviceHttpClient;
    private readonly ProductsMicroserviceHttpClient _productsMicroserviceHttpClient;
    private readonly IPublishEndpoint _publishEndpoint;


    public CreateOrderHandler(ApplicationDbContext dbContext, UsersMicroserviceHttpClient usersMicroserviceHttpClient, ProductsMicroserviceHttpClient productsMicroserviceHttpClient, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _usersMicroserviceHttpClient = usersMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(usersMicroserviceHttpClient));
        _productsMicroserviceHttpClient = productsMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(productsMicroserviceHttpClient));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new CreateOrderResponse
            {
                Success = false,
                Message = "Invalid order request."
            };
        }
        else if (request.OrderItems == null || !request.OrderItems.Any())
        {
            return new CreateOrderResponse
            {
                Success = false,
                Message = "Order must contain at least one item."
            };
        }

        bool userExists = await _usersMicroserviceHttpClient.UserExistsAsync(request.UserId);
        if (!userExists)
        {
            return new CreateOrderResponse
            {
                Success = false,
                Message = $"User with ID {request.UserId} not found."
            };
        }

        foreach (var item in request.OrderItems)
        {
            bool productExists = await _productsMicroserviceHttpClient.ProductExistsAsync(item.ProductId);
            if (!productExists)
            {
                return new CreateOrderResponse
                {
                    Success = false,
                    Message = $"Product with ID {item.ProductId} not found."
                };
            }
        }

        Order orderRequest = new Order
        {
            OrderId = Guid.NewGuid(),
            UserId = request.UserId,
            OrderDate = request.OrderDate,
            TotalBill = request.TotalBill,
            OrderItems = request.OrderItems ?? new List<eCommerceSolution.OrdersService.Models.Entities.OrderItem>()
        };

        _dbContext.Orders.Add(orderRequest);
        int rowsAffected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (rowsAffected > 0)
        {
            OrderPlaced orderPlacedEvent = new OrderPlaced
            {
                OrderId = orderRequest.OrderId,
                UserId = orderRequest.UserId,
                OrderDate = orderRequest.OrderDate,
                TotalBill = orderRequest.TotalBill,
                OrderItems = orderRequest.OrderItems.Select(item => new eCommerce.Microservices.Events.Order.OrderItem
                {
                    ProductId = item.ProductId,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            await _publishEndpoint.Publish(orderPlacedEvent, cancellationToken);
            return new CreateOrderResponse
            {
                Success = true,
                Message = "Order created successfully."
            };
        }
        else
        {
            return new CreateOrderResponse
            {
                Success = false,
                Message = "Failed to create order."
            };
        }
    }
}
