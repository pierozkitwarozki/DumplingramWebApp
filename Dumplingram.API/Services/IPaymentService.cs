using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Stripe.Checkout;

namespace Dumplingram.API.Services
{
    public interface IPaymentService
    {
         Task<Session> CreateSessionAsync(DonationDto donationDto);
    }
}