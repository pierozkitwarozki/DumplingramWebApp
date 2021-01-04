using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> MakeCharge([FromBody]string token) 
        {
            
            var options = new ChargeCreateOptions
            {
                Amount = 999,
                Currency = "usd",
                Description = "Example charge",
                Source = token
            };
            
            var service = new ChargeService();
            var charge = await service.CreateAsync(options);
            return Ok(charge);
        }
    }
}