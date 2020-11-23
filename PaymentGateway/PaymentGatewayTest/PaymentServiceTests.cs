using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.Models;
using PaymentGateway.Services;
using System;
using System.Threading.Tasks;

namespace PaymentGatewayTest
{
    [TestClass]
    public class PaymentServiceTests
    {
        [TestMethod]
        public async Task TestProcessPayment_BankConfirm()
        {
            // create mock version
            var mockUserService = new Mock<IUserService>();
            var mockBankService = new Mock<IBankService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns("doesnt matter");
            mockUserService.Setup(x => x.GetUserIdFromToken(It.IsAny<string>())).Returns("TestUserID");
            mockBankService.Setup(x => x.MakePaymentInTheBank(It.IsAny<Payment>()))
                          .Returns(Task.FromResult(new BankResponse()
                          {
                              BankConfirmationID = "test-confirmation",
                              IsPaymentSuccessful = true
                          }));

            var service = new PaymentService(null, null, mockUserService.Object, mockBankService.Object, mockHttpContextAccessor.Object);
            var processedPayment = await service.ProcessPayment(new Payment());
            
            Assert.AreEqual(processedPayment.IsPaymentSuccessful, true);
            Assert.AreEqual(processedPayment.UserID, "TestUserID");
            Assert.AreEqual(processedPayment.BankConfirmationID, "test-confirmation");
        }

        [TestMethod]
        public async Task TestProcessPayment_BankReject()
        {
            // create mock version
            var mockUserService = new Mock<IUserService>();
            var mockBankService = new Mock<IBankService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns("doesnt matter");
            mockUserService.Setup(x => x.GetUserIdFromToken(It.IsAny<string>())).Returns("TestUserID");
            mockBankService.Setup(x => x.MakePaymentInTheBank(It.IsAny<Payment>()))
                          .Returns(Task.FromResult(new BankResponse()
                          {
                              BankConfirmationID = "test-confirmation",
                              IsPaymentSuccessful = false
                          }));

            var service = new PaymentService(null, null, mockUserService.Object, mockBankService.Object, mockHttpContextAccessor.Object);
            var processedPayment = await service.ProcessPayment(new Payment());

            Assert.AreEqual(processedPayment.IsPaymentSuccessful, false);
            Assert.AreEqual(processedPayment.UserID, "TestUserID");
            Assert.AreEqual(processedPayment.BankConfirmationID, "test-confirmation");
        }

        [TestMethod]
        public async Task TestGetPaymentByID_IDFound()
        {
            // create mock version
            var mockUserService = new Mock<IUserService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var options = new DbContextOptionsBuilder<PaymentGatewayContext>()
           .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            // Insert data into the database 
            var dbContext = new PaymentGatewayContext(options);
            var paymentId1 = Guid.NewGuid().ToString();
            var paymentId2 = Guid.NewGuid().ToString();
            var paymentId3 = Guid.NewGuid().ToString();
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId1, Amount = 32, UserID = "TestUserID", CreditCardNumber = "1234561234561234", CVV = 657, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId2, Amount = 500, UserID = "TestUserID", CreditCardNumber = "1234561234561235", CVV = 123, Currency = "SHEKEL", ExpirationDate = new DateTime(2025, 5, 1), IsPaymentSuccessful = false });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId3, Amount = 421, UserID = "Test2UserID", CreditCardNumber = "1234561234561236", CVV = 789, Currency = "DOLLAR", ExpirationDate = new DateTime(2021, 2, 13), IsPaymentSuccessful = true });
            dbContext.SaveChanges();

            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns("testMerchantID - doesnt matter");
            mockUserService.Setup(x => x.GetUserIdFromToken(It.IsAny<string>())).Returns("TestUserID");

            var service = new PaymentService(dbContext, null, mockUserService.Object, null, mockHttpContextAccessor.Object);
            var paymentDTO = await service.GetPaymentByID(paymentId1);

            Assert.AreEqual(paymentDTO.ID, paymentId1);
            Assert.AreEqual(paymentDTO.Amount, 32);
            Assert.AreEqual(paymentDTO.IsPaymentSuccessful, true);
            Assert.AreEqual(paymentDTO.ExpirationDate, new DateTime(2020, 12, 1));
            Assert.AreEqual(paymentDTO.MaskedCardNumber, "1234 56XX XXXX 1234");
            Assert.AreEqual(paymentDTO.Currency, "EURO");
        }

        [TestMethod]
        public async Task TestGetPaymentByID_IDNotFound()
        {
            // create mock version
            var mockUserService = new Mock<IUserService>();
            var mockLoggerService = new Mock<ILogger<PaymentService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var options = 
                new DbContextOptionsBuilder<PaymentGatewayContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            // Insert data into the database 
            var dbContext = new PaymentGatewayContext(options);
            var paymentId1 = Guid.NewGuid().ToString();
            var paymentId2 = Guid.NewGuid().ToString();
            var paymentId3 = Guid.NewGuid().ToString();
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId1, Amount = 32, UserID = "TestUserID", CreditCardNumber = "1234561234561234", CVV = 657, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId2, Amount = 500, UserID = "TestUserID", CreditCardNumber = "1234561234561235", CVV = 123, Currency = "SHEKEL", ExpirationDate = new DateTime(2025, 5, 1), IsPaymentSuccessful = false });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId3, Amount = 421, UserID = "Test2UserID", CreditCardNumber = "1234561234561236", CVV = 789, Currency = "DOLLAR", ExpirationDate = new DateTime(2021, 2, 13), IsPaymentSuccessful = true });
            dbContext.SaveChanges();

            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns("testMerchantID - doesnt matter");
            mockUserService.Setup(x => x.GetUserIdFromToken(It.IsAny<string>())).Returns("TestUserID");

            var service = new PaymentService(dbContext, mockLoggerService.Object, mockUserService.Object, null, mockHttpContextAccessor.Object);
            var paymentDTO = await service.GetPaymentByID("aaaa");

            Assert.AreEqual(paymentDTO, null);
        }

        [TestMethod]
        public async Task TestGetPaymentByID_UserNotFound()
        {
            // create mock version
            var mockUserService = new Mock<IUserService>();
            var mockLoggerService = new Mock<ILogger<PaymentService>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var options = 
                new DbContextOptionsBuilder<PaymentGatewayContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            // Insert data into the database 
            var dbContext = new PaymentGatewayContext(options);
            var paymentId1 = Guid.NewGuid().ToString();
            var paymentId2 = Guid.NewGuid().ToString();
            var paymentId3 = Guid.NewGuid().ToString();
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId1, Amount = 32, UserID = "TestUserID", CreditCardNumber = "1234561234561234", CVV = 657, Currency = "EURO", ExpirationDate = new DateTime(2020, 12, 1), IsPaymentSuccessful = true });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId2, Amount = 500, UserID = "TestUserID", CreditCardNumber = "1234561234561235", CVV = 123, Currency = "SHEKEL", ExpirationDate = new DateTime(2025, 5, 1), IsPaymentSuccessful = false });
            dbContext.PaymentItems.Add(new Payment() { ID = paymentId3, Amount = 421, UserID = "Test2UserID", CreditCardNumber = "1234561234561236", CVV = 789, Currency = "DOLLAR", ExpirationDate = new DateTime(2021, 2, 13), IsPaymentSuccessful = true });
            dbContext.SaveChanges();

            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns("testMerchantID - doesnt matter");
            mockUserService.Setup(x => x.GetUserIdFromToken(It.IsAny<string>())).Returns("TestUserID5");

            var service = new PaymentService(dbContext, mockLoggerService.Object, mockUserService.Object, null, mockHttpContextAccessor.Object);
            var paymentDTO = await service.GetPaymentByID(paymentId1);

            Assert.AreEqual(paymentDTO, null);
        }
    }

}
