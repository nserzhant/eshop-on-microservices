using EShop.Ordering.Api.Models;
using EShop.Ordering.Core.Interfaces;
using EShop.Ordering.Infrastructure.Read;
using EShop.Ordering.Infrastructure.Read.Queries;
using EShop.Ordering.Infrastructure.Read.ReadModels;
using EShop.Ordering.Infrastructure.Read.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShop.Ordering.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderQueryService _orderQueryService;

    public OrderController(IOrderRepository orderRepository, IOrderQueryService orderQueryService)
    {
        _orderRepository = orderRepository;
        _orderQueryService = orderQueryService;
    }

    [ProducesResponseType(typeof(OrderReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderAsync(int orderId)
    {
        var order = await _orderQueryService.GetByIdAsync(orderId);
        var customerId = getCustomerId();

        if (order == null)
        {
            return NotFound();
        }

        if (customerId != order.CustomerId)  
        {
            return Forbid();
        }

        return Ok(order);
    }

    [ProducesResponseType(typeof(ListOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync()   
    {
        ListOrderQuery listOrderQuery = new ListOrderQuery()
        {
            CustomerId = getCustomerId(),
            PageIndex = 0,
            PageSize = int.MaxValue,
            OrderBy = ListOrderOrderBy.Id,
            OrderByDirection = OrderByDirections.DESC
        };

        var result = await _orderQueryService.ListOrdersAsync(listOrderQuery);

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OrderDomainErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPost("{orderId:int}/return")]
    public async Task<IActionResult> ReturnOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        var customerId = getCustomerId();

        if (order == null)
        {
            return NotFound();
        }

        if (customerId != order.CustomerId)
        {
            return Forbid();
        }

        order.Return();

        await _orderRepository.SaveChangesAsync();

        return Ok();
    }

    private Guid getCustomerId()
    {
        var customerId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (customerId == null)
        {
            return Guid.Empty;
        }

        return new Guid(customerId);
    }
}
