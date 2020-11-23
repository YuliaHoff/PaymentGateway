using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class PaymentDTO
    {
        public string ID { get; set; }
        public string MaskedCardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public bool IsPaymentSuccessful { get; set; }
    
    }
}
