using AcquiringBankSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcquiringBankSimulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProccessPaymentController : ControllerBase
    {
        private static Dictionary<string, bool> _allPayments = new Dictionary<string, bool>();
        private readonly ILogger<ProccessPaymentController> _logger;

        public ProccessPaymentController(ILogger<ProccessPaymentController> logger)
        {
            _logger = logger;
        }

        // POST: ProcessPayment
        [HttpPost]
        public async Task<ActionResult<Payment>> ProcessPayment(Payment payment)
        {
            var paymentResult = new PaymentResult
            {
                BankConfirmationID = Guid.NewGuid().ToString(),
                IsPaymentSuccessful = new Random().Next(2) == 0
            };

            _allPayments.Add(paymentResult.BankConfirmationID, paymentResult.IsPaymentSuccessful);
            return CreatedAtAction(nameof(ProcessPayment), new { id = paymentResult.BankConfirmationID }, paymentResult);
        }
    }
}
