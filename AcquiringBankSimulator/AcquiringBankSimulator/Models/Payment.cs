using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcquiringBankSimulator.Models
{
    public class Payment
    {       
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int CardCVV { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CreditCardNumber { get; set; }
        public string Currency { get; set; }
    }
}
