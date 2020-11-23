using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentGateway.Controllers;
using PaymentGateway.Models;
using PaymentGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayTest
{
    [TestClass]
    public class PaymentControllerTest
    {
        [TestMethod]
        public async Task TestGetPaymentByID_PaymentFound()
        {
            // create mock versions
            var mockPaymentService = new Mock<IPaymentService>();
            var mockLoggerService = new Mock<ILogger<PaymentsController>>();

            var paymentDTO = new PaymentDTO() { ID = Guid.NewGuid().ToString()};
            mockPaymentService.Setup(x => x.GetPaymentByID(It.IsAny<string>())).Returns(Task.FromResult(paymentDTO));

            PaymentsController controller = new PaymentsController(null, mockLoggerService.Object, null, mockPaymentService.Object);
            var result = await controller.GetPaymentByID(paymentDTO.ID);

            Assert.IsInstanceOfType(result.Result, typeof(Microsoft.AspNetCore.Mvc.OkObjectResult));
            var paymentDTOresult = (result.Result as Microsoft.AspNetCore.Mvc.OkObjectResult).Value;
            Assert.IsNotNull(paymentDTOresult);
            Assert.IsInstanceOfType(paymentDTOresult, typeof(PaymentDTO));
            Assert.AreEqual((paymentDTOresult as PaymentDTO).ID, paymentDTO.ID);
        }

        [TestMethod]
        public async Task TestGetPaymentByID_PaymentNotFound()
        {
            // create mock versions
            var mockPaymentService = new Mock<IPaymentService>();
            var mockLoggerService = new Mock<ILogger<PaymentsController>>();

            var paymentDTO = new PaymentDTO() { ID = Guid.NewGuid().ToString() };
            mockPaymentService.Setup(x => x.GetPaymentByID(It.IsAny<string>())).Returns(Task.FromResult<PaymentDTO>(null));

            PaymentsController controller = new PaymentsController(null, mockLoggerService.Object, null, mockPaymentService.Object);
            var result = await controller.GetPaymentByID(paymentDTO.ID);

            Assert.IsInstanceOfType(result.Result, typeof(Microsoft.AspNetCore.Mvc.NotFoundResult));
        }
    }
}
