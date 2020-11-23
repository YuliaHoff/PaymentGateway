using Microsoft.Extensions.Logging;
using PaymentGateway.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class BankService : IBankService
    {
        private readonly PaymentGatewayContext _context;
        private readonly ILogger<BankService> _logger;

        public BankService(PaymentGatewayContext context, ILogger<BankService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BankResponse> MakePaymentInTheBank(Payment payment)
        {
            BankResponse bankResponse = null;
            // Try to make payment in the bank
            using (var client = new HttpClient())
            {
                // Bank API address
                client.BaseAddress = new Uri("https://localhost:44317/ProccessPayment");

                // Process payment in POST call to Bank and get payment result
                var postTask = await client.PostAsJsonAsync<Payment>("ProccessPayment", payment);
                bankResponse = postTask.Content.ReadFromJsonAsync<BankResponse>().Result;
            }

            if (bankResponse == null)
            {
                return null;
            }

            payment.BankConfirmationID = bankResponse.BankConfirmationID;
            payment.IsPaymentSuccessful = bankResponse.IsPaymentSuccessful;            

            _context.PaymentItems.Add(payment);
            await _context.SaveChangesAsync();

            return bankResponse;
        }
    }
}
