using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Dumplingram.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Dumplingram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IPaymentService _service;

        public CheckoutController(IPaymentService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(DonationDto donationDto)
        {
            try 
            {
                return Ok(await _service.CreateSessionAsync(donationDto));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}