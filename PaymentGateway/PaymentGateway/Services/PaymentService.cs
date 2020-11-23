using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PaymentGateway.Controllers;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentGatewayContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserService _userService;
        private readonly IBankService _bankService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(PaymentGatewayContext context, ILogger<PaymentService> logger, IUserService userService, IBankService bankService, IHttpContextAccessor httpContext)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
            _httpContext = httpContext;
            _bankService = bankService;
        }

        protected string MaskCardNumber(string cardNumber)
        {
            try
            {
                var firstDigits = cardNumber.Substring(0, 6);
                var lastDigits = cardNumber.Substring(cardNumber.Length - 4, 4);

                var requiredMask = new String('X', cardNumber.Length - firstDigits.Length - lastDigits.Length);

                var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);
                var maskedCardNumberWithSpaces = Regex.Replace(maskedString, ".{4}", "$0 ");
                maskedCardNumberWithSpaces = maskedCardNumberWithSpaces.Substring(0, maskedCardNumberWithSpaces.Length - 1);
                return maskedCardNumberWithSpaces;
            }
            catch (Exception ex)
            {
                _logger.LogError("MaskCardNumber: error occured: " + ex.Message);
                return "";
            }
        } 

        public PaymentDTO PaymentToDTO(Payment payment) =>
            new PaymentDTO()
            {
                ID = payment.ID,
                Currency = payment.Currency,
                Amount = payment.Amount,
                MaskedCardNumber = MaskCardNumber(payment.CreditCardNumber),
                ExpirationDate = payment.ExpirationDate,
                IsPaymentSuccessful = payment.IsPaymentSuccessful
            };

        public List<PaymentDTO> PaymentsToDTOs(IEnumerable<Payment> payments)
        {
            var paymentDTOs = new List<PaymentDTO>();

            foreach (Payment currPayment in payments)
            {
                paymentDTOs.Add(PaymentToDTO(currPayment));
            }
            return paymentDTOs;
        }

        public async Task<IEnumerable<Payment>> GetAllPayments()
        {
            string UserID = _userService.GetUserIdFromToken(_httpContext.HttpContext.User.Identity.Name);
            return await _context.PaymentItems.Where(x => x.UserID == UserID).ToListAsync();
        }

        public async Task<Payment> ProcessPayment(Payment payment)
        {
            payment.ID = Guid.NewGuid().ToString();
            payment.UserID = _userService.GetUserIdFromToken(_httpContext.HttpContext.User.Identity.Name);
            
            var bankResponse = await _bankService.MakePaymentInTheBank(payment);
            if (bankResponse == null)
            {
                return null;
            }

            payment.IsPaymentSuccessful = bankResponse.IsPaymentSuccessful;
            payment.BankConfirmationID = bankResponse.BankConfirmationID;

            return payment; 
        }

        public async Task<PaymentDTO> GetPaymentByID(string paymentID)
        {
            string userID = _userService.GetUserIdFromToken(_httpContext.HttpContext.User.Identity.Name);
            // Make sure user is the right user - don't retrieve a payment that belongs to a different merchant
            var payment = await _context.PaymentItems.Where(x => x.ID == paymentID && x.UserID == userID).SingleOrDefaultAsync();
            if (payment == null)
            {
                _logger.LogInformation("GetPaymentByID: no payment with ID: " + userID + "exists for user: " + userID);
                return null;
            }
            
            return PaymentToDTO(payment);
        }
    }
}
