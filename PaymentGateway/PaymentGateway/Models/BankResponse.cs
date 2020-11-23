using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class BankResponse
    {
        public string PaymentID { get; set; }
        public bool IsPaymentSuccessful { get; set; }
        public string BankConfirmationID { get; set; }
    }
}
