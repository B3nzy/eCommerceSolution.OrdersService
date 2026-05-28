using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Models.DTOs.CreateOrder;
using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;

namespace eCommerceSolution.OrdersService.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UsersMicroserviceHttpClient _usersMicroserviceHttpClient;
    private readonly ProductsMicroserviceHttpClient _productsMicroserviceHttpClient;


    public CreateOrderHandler(ApplicationDbContext dbContext, UsersMicroserviceHttpClient usersMicroserviceHttpClient, ProductsMicroserviceHttpClient productsMicroserviceHttpClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _usersMicroserviceHttpClient = usersMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(usersMicroserviceHttpClient));
        _productsMicroserviceHttpClient = productsMicroserviceHttpClient ?? throw new ArgumentNullException(nameof(productsMicroserviceHttpClient));
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
                Message = "User with ID {request.UserId} not found."
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
                    Message = "Product with ID {item.ProductId} not found."
                };
            }
        }

        Order orderRequest = new Order
        {
            OrderId = Guid.NewGuid(),
            UserId = request.UserId,
            OrderDate = request.OrderDate,
            TotalBill = request.TotalBill,
            OrderItems = request.OrderItems ?? new List<OrderItem>()
        };

        _dbContext.Orders.Add(orderRequest);
        int rowsAffected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (rowsAffected > 0)
        {
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
