using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Models
{
    public class PaymentGatewayContext : DbContext  
    {
        public PaymentGatewayContext(DbContextOptions<PaymentGatewayContext> options)
           : base(options)
        {
            // TODO: remove func, no need to init after a real db is added
            TempInitDB();
        }

        public DbSet<Payment> PaymentItems { get; set; }
        public DbSet<User> Users { get; set; }
        private void TempInitDB()
        {
            if (Users.Count() > 0)
                return;

            // Actual password for each user is the UserName: example - UserName = "Apple" => Password = "Apple"
            Users.Add(new User() { ID = "asdf", UserName = "Apple", HashedPassword = "BkOnj+cw8IyjFGqRTds2AoBV5OOBKJ8RNQ/o+TO60Oc=", Salt="superRandomSalt" });
            Users.Add(new User() { ID = "asdg", UserName = "Amazon", HashedPassword = "oFd8x54iQJOx5/2ViKeFxeuKh4+K8QI8kJc4OUjCyPw=", Salt = "evenMoreRandomSalt" });

            PaymentItems.AddRange(
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 32, UserID = "asdf", CreditCardNumber = "1234561234561234", CVV = 657, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 35, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 131, Currency = "EURO", ExpirationDate = new DateTime(2022, 11, 2), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 22, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 985, Currency = "EURO", ExpirationDate = new DateTime(2023, 11, 3), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 42, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 123, Currency = "EURO", ExpirationDate = new DateTime(2023, 11, 4), IsPaymentSuccessful = true },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 12, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 524, Currency = "EURO", ExpirationDate = new DateTime(2023, 11, 5), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 42, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 634, Currency = "EURO", ExpirationDate = new DateTime(2024, 11, 6), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 32, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 467, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 32, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 923, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 32, UserID = "asdf", CreditCardNumber = "1234578902237320", CVV = 947, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = false },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 32, UserID = "asdg", CreditCardNumber = "1234578902237320", CVV = 352, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 12, UserID = "asdg", CreditCardNumber = "1234678902237320", CVV = 799, Currency = "EURO", ExpirationDate = new DateTime(2023, 11, 7), IsPaymentSuccessful = true },
                new Payment() { ID = Guid.NewGuid().ToString(), Amount = 39, UserID = "asdg", CreditCardNumber = "1234678902237320", CVV = 845, Currency = "EURO", ExpirationDate = new DateTime(2023, 11, 8), IsPaymentSuccessful = false });
            SaveChanges();
        }
    }
}
