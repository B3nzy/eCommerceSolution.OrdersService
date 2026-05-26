using eCommerceSolution.OrdersService.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace eCommerceSolution.OrdersService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{

    private readonly ILogger<OrdersController> _logger;
    private readonly IMediator _mediator;

    public OrdersController(ILogger<OrdersController> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    #region API Endpoints

    /// <summary>
    /// Test endpoint to verify that the Orders Service is running.
    /// </summary>
    /// <returns></returns>
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("Orders Service is running.");
    }


    /// <summary>
    /// Creates a new order based on the provided order details. 
    /// The request body should contain the necessary information to create an order, such as user ID, order date, total bill, and order items. 
    /// The endpoint will return a response indicating whether the order creation was successful or if there were any issues with the request.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest order)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await _mediator.Send(order);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("[END] CreateOrder request processed in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        }

    }

    #endregion
}
