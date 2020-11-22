using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcquiringBankSimulator.Models
{
    public class PaymentResult
    {
        public bool IsPaymentSuccessful { get; set; }
        public string BankConfirmationID { get; set; }
    }
}
