using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FGC.Payments.Application.Services;

namespace FGC.Payments.Api.Controllers;

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        var payment = await _paymentService.GetByOrderIdAsync(orderId);
        return payment is null ? NotFound() : Ok(payment);
    }
}
