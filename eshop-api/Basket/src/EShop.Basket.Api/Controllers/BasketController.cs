using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShop.Basket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;
    private readonly IBasketRepository _basketRepository;
    private readonly IBasketService _basketService;

    public BasketController(ILogger<BasketController> logger, 
        IBasketRepository basketRepository,
        IBasketService basketService)
    {
        _logger = logger;
        _basketRepository = basketRepository;
        _basketService = basketService;
    }

    [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetBasketAsync()
    {
        var customerId = getCustomerId();
        var basket = await _basketRepository.GetBasketAsync(customerId);

        return Ok(basket);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> SaveBasketAsync(CustomerBasket basket)
    {
        var customerId = getCustomerId();
        await _basketRepository.SaveBasketAsync(customerId, basket);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOutAsync()
    {
        var customerId = getCustomerId();

        await _basketService.CheckOutAsync(customerId);

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
