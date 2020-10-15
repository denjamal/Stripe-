using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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



        [HttpGet]
        [Route("create-payment-intent")]
        public ActionResult Create()
        {
            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Amount = 1000,
                Currency = "gbp",
                PaymentMethodTypes = new List<string> {
                        "card",
                    },

            }, new RequestOptions
            {
                IdempotencyKey = Guid.NewGuid().ToString() // TODO: this id should be related to the Campaign(Id)
            });
            return Json(new { clientSecret = paymentIntent.ClientSecret });
        }

    }
}
