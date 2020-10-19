using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using Stripe.Models;

namespace Stripe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = "price_1HcXrkG4Qj4VbiKAUJYTP764",
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = "http://localhost:55333",
                CancelUrl = "http://localhost:55333",
            };

            var service = new SessionService();
            try
            {
                Session session = service.Create(options);
                return Json(new { id = session.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            
        }


        [HttpPost("stripe-events")]
        public async Task<IActionResult> StripeEvents()
        {
            const string endpointSecret = "whsec_RxsYX0X8QkUOWwTEUWPmyYAUHVJqWcB0"; // TODO Get it from settings
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.InvoicePaid)
                {
                    var paymentIntent = stripeEvent.Data.Object as Invoice;
                    // We can find customer By email and set current_period_end;
                }
                else if (stripeEvent.Type == Events.InvoicePaymentFailed)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // We can find user and notify about failed payment.
                }
                else if (stripeEvent.Type == Events.InvoicePaymentActionRequired)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // We can find user and notify about action required.
                }
                else if (stripeEvent.Type == Events.InvoiceUpcoming)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // We can find user and notify about upcoming payment.
                }
                else if (stripeEvent.Type == Events.CustomerCreated)
                {
                    var customer = stripeEvent.Data.Object as Customer;
                    // Get extend our customer with a data from Stripe customer(Id)
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    // Store subscription Id to be able to cancel it in the future.
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    // Here we can check the status of the subscription and change current_period_end for the user.
                    // Useful when we have many subscriptions for one user. Other wise we can use InvoicePaid and InvoicePaymentFailed for this purpose.
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
