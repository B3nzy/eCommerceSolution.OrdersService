using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.Models.DTOs.GetOrderById;
using eCommerceSolution.OrdersService.Models.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerceSolution.OrdersService.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdRequest, GetOrderByIdResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetOrderByIdHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
    {
        Order? order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == Guid.Parse(request.OrderId));

        if (order == null)
        {
            return null;
        }

        return new GetOrderByIdResponse
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            OrderItems = order.OrderItems,
            TotalBill = order.TotalBill,
            UserId = order.UserId
        };
    }
}
