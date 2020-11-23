using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class User
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public string HashedPassword { get; set; }

        public string Salt { get; set; }
    }
}
