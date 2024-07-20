using EShop.Basket.Api.Models;
using EShop.Basket.Core.Interfaces;
using EShop.Basket.Core.Models;
using EShop.Basket.Integration.Events;
using MassTransit;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketController(ILogger<BasketController> logger,
        IBasketRepository basketRepository,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _basketRepository = basketRepository;
        _publishEndpoint = publishEndpoint;
    }

    [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> GetBasketAsync()
    {
        var customerId = getCustomerId();
        var basket = await _basketRepository.GetBasketAsync(customerId);

        return Ok(basket);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<IActionResult> SaveBasketAsync(CustomerBasket basket)
    {
        var customerId = getCustomerId();
        await _basketRepository.SaveBasketAsync(customerId, basket);

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOutAsync(CheckoutDTO checkoutDTO)
    {
        var customerId = getCustomerId();
        var email = getCustomerEmail();

        if (email == string.Empty)
        {
            return Forbid();
        }

        var basket = await _basketRepository.GetBasketAsync(customerId);
        var checkoutEvent = new BasketCheckedOutEvent
            (
                Guid.NewGuid(),
                customerId,
                email,
                checkoutDTO.ShippingAddress,
                basket.Id,
                basket.Items.Select(itm => new BasketCheckoutItem
                (
                    itm.CatalogItemId,
                    itm.ItemName,
                    itm.Qty,
                    itm.Description,
                    itm.Price,
                    itm.TypeName,
                    itm.BrandName,
                    itm.PictureUri
                )).ToList()
            );

        await _publishEndpoint.Publish(checkoutEvent);

        return Ok();
    }

    private string getCustomerEmail() => User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ?? string.Empty;

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
