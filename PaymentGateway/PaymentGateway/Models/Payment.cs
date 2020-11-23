using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Payment
    {
        public string ID { get; set; }

        public string UserID { get; set; }

        [Required]
        public string CreditCardNumber { get; set; }


        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public int CVV { get; set; }

        public bool IsPaymentSuccessful { get; set; }

        public string BankConfirmationID { get; set; }

        public override bool Equals(Object paymentB)
        {
            return this.ID == ((Payment)paymentB).ID;
        }
    }
}
