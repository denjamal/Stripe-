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

    }
}
