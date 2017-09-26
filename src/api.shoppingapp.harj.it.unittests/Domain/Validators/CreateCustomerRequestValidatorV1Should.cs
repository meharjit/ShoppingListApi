using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.shoppingapp.harj.it.Domain.Validators;
using contracts.shoppingapp.harj.it;
using NUnit.Framework;

namespace api.shoppingapp.harj.it.unittests.Domain.Validators
{
    [TestFixture]
    public class CreateCustomerRequestValidatorV1Should
    {
        [TestCase()]
        public void Return_validation_errors_if_customer_has_no_values()
        {
            CreateCustomerRequestValidatorV1 createCustomerRequestValidator = new CreateCustomerRequestValidatorV1();
            CreateCustomerRequestV1 customer = new CreateCustomerRequestV1();
            var errors = createCustomerRequestValidator.GetValidationErrors(customer);

            Assert.AreEqual(errors.Count, 2);
            Assert.AreEqual(errors[0], "CreateCustomerRequestV1.Name");
            Assert.AreEqual(errors[1], "CreateCustomerRequestV1.Email");
        }

        [Test]
        public void Return_error_if_the_customer_contains_only_an_invalid_email()
        {
            CreateCustomerRequestValidatorV1 createCustomerRequestValidator = new CreateCustomerRequestValidatorV1();
            CreateCustomerRequestV1 customer = new CreateCustomerRequestV1() { Name = "test" };
            var errors = createCustomerRequestValidator.GetValidationErrors(customer);

            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual(errors[0], "CreateCustomerRequestV1.Email");
        }

        [Test]
        public void Return_error_if_the_customer_contains_only_an_invalid_name()
        {
            CreateCustomerRequestValidatorV1 createCustomerRequestValidator = new CreateCustomerRequestValidatorV1();
            CreateCustomerRequestV1 customer = new CreateCustomerRequestV1() { Email = "test@example.com" };
            var errors = createCustomerRequestValidator.GetValidationErrors(customer);

            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual(errors[0], "CreateCustomerRequestV1.Name");
        }

        [TestCase("test", false)]
        [TestCase("test@example.com", true)]
        [TestCase("@", true)]
        public void Return_error_is_email_is_invalid(string email, bool isValid)
        {
            CreateCustomerRequestValidatorV1 createCustomerRequestValidator = new CreateCustomerRequestValidatorV1();
            CreateCustomerRequestV1 customer = new CreateCustomerRequestV1() {Name ="any name", Email = email};
            var errors = createCustomerRequestValidator.GetValidationErrors(customer);

            if (isValid)
            {
                Assert.IsEmpty(errors);
            }
            else
            {
                Assert.AreEqual(errors.Count, 1);
                Assert.AreEqual(errors[0], "CreateCustomerRequestV1.Email");
            }
        }
    }
}
