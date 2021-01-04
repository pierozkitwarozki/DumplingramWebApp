using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Stripe.Checkout;

namespace Dumplingram.API.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<Session> CreateSessionAsync(DonationDto donationDto)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = donationDto.Amount,
                            Currency = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Donation"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "http://localhost:4200/success-donation",
                CancelUrl = "http://localhost:4200/donate",
            };
            
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session;
        }
    }
}