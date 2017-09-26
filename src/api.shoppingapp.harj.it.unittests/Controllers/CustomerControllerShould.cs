using System;
using System.Collections.Generic;
using System.Net;
using api.shoppingapp.harj.it.Controllers;
using api.shoppingapp.harj.it.Domain.Repositories;
using api.shoppingapp.harj.it.Domain.Validators;
using contracts.shoppingapp.harj.it;
using Castle.Components.DictionaryAdapter;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace api.shoppingapp.harj.it.unittests.Controllers
{
    [TestFixture]
    public class CustomerControllerShould
    {
        string password = "p@ssword1";

        [Test]
        public void Call_the_create_customer_repository_when_customerobject_is_valid()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1(){Password = password};
            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            Mock<ICreateCustomerRequestValidatorV1> createCustomerRequestValidatorV1Mock = new Mock<ICreateCustomerRequestValidatorV1>();
            CustomerController customerController = new CustomerController(createCustomerRequestValidatorV1Mock.Object, customerRepositoryMock.Object);
            createCustomerRequestValidatorV1Mock.Setup(r => r.GetValidationErrors(customerObject))
                .Returns(new List<string>());
            customerRepositoryMock.Setup(r => r.SaveCustomer(customerObject, password));

            //Act
            var response = customerController.UpsertRecord(customerObject);

            //Assert
            customerRepositoryMock.Verify(r => r.SaveCustomer(customerObject, password), Times.Once());
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.Created);
        }

        [Test]
        public void Not_Call_the_create_customer_repository_when_customerobject_is_invalid()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1();

            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            Mock<ICreateCustomerRequestValidatorV1> createCustomerRequestValidatorV1Mock = new Mock<ICreateCustomerRequestValidatorV1>();
            CustomerController customerController = new CustomerController(createCustomerRequestValidatorV1Mock.Object, customerRepositoryMock.Object);
            createCustomerRequestValidatorV1Mock.Setup(r => r.GetValidationErrors(customerObject))
                .Returns(new List<string>(){"some validation error happened"});
            customerRepositoryMock.Setup(r => r.SaveCustomer(customerObject, password));

            //Act
            var response = customerController.UpsertRecord(customerObject);

            //Assert
            customerRepositoryMock.Verify(r => r.SaveCustomer(customerObject, password), Times.Never);
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void Return_validation_error_on_argumentexception()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1(){Password = password};
            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            Mock<ICreateCustomerRequestValidatorV1> createCustomerRequestValidatorV1Mock = new Mock<ICreateCustomerRequestValidatorV1>();
            CustomerController customerController = new CustomerController(createCustomerRequestValidatorV1Mock.Object, customerRepositoryMock.Object);
            createCustomerRequestValidatorV1Mock.Setup(r => r.GetValidationErrors(customerObject)).Returns(new List<string>());
            customerRepositoryMock.Setup(r => r.SaveCustomer(customerObject, password)).Throws(new ArgumentException("shits fucked yo"));

            //Act
            var response = customerController.UpsertRecord(customerObject);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Result.StatusCode);
        }


        [Test]
        public void Call_the_get_customer_repository_when_requested_With_An_Id()
        {
            // Arrange
            string customerGuid = "some_guid_value";
            Customer customer = new Customer() { Email = "test@example.com", Guid = "some_guid_value", Name = "John Smith" };

            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            CustomerController customerController = new CustomerController(null, customerRepositoryMock.Object);
            customerRepositoryMock.Setup(r => r.GetCustomer(customerGuid)).Returns(customer);

            //Act
            var response = customerController.GetCustomer(customerGuid);

            //Assert
            var getCustomerResponseV1 = JsonConvert.DeserializeObject<GetCustomerResponseV1>(response.Result.Content.ReadAsStringAsync().Result);
            Assert.IsInstanceOf<GetCustomerResponseV1>(getCustomerResponseV1);
            customerRepositoryMock.Verify(r => r.GetCustomer(customerGuid), Times.Once());
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(customer.Guid, getCustomerResponseV1.Guid);
            Assert.AreEqual(customer.Name, getCustomerResponseV1.Name);
            Assert.AreEqual(customer.Email, getCustomerResponseV1.Email);
        }


        [Test]
        public void Call_the_get_customer_repository_when_requested_With_An_Id_that_doesnt_Exist()
        {
            // Arrange
            string customerGuid = "some_guid_value";
            Customer customer = new Customer() { Email = "test@example.com", Guid = "some_guid_value", Name = "John Smith" };

            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            CustomerController customerController = new CustomerController(null, customerRepositoryMock.Object);
            customerRepositoryMock.Setup(r => r.GetCustomer(customerGuid)).Returns((Customer)null);

            //Act
            var response = customerController.GetCustomer(customerGuid);

            //Assert
            customerRepositoryMock.Verify(r => r.GetCustomer(customerGuid), Times.Once());
            Assert.IsNull(response.Result.Content);
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public void Call_the_get_customer_repository_when_requested_With_Email_Password_That_Doesnt_Exist()
        {
            // Arrange
            string customerGuid = "some_guid_value";
            LoginCustomerRequestV1 loginCustomerRequest = new LoginCustomerRequestV1() { Email = "test@example.com", Password = Guid.NewGuid().ToString() };

            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            CustomerController customerController = new CustomerController(null, customerRepositoryMock.Object);
            customerRepositoryMock.Setup(r => r.Login(loginCustomerRequest.Email, loginCustomerRequest.Password)).Returns((Customer)null);

            //Act
            var response = customerController.AuthenticateCustomer(loginCustomerRequest);

            //Assert
            Assert.IsNull(response.Result.Content);
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.Unauthorized);
        }

        [Test]
        public void Call_the_get_customer_repository_when_requested_With_Email_Password_That_Does_Exist()
        {
            // Arrange
            string customerGuid = "some_guid_value";
            LoginCustomerRequestV1 loginCustomerRequest = new LoginCustomerRequestV1() { Email = "test@example.com", Password = Guid.NewGuid().ToString() };
            var customer = new Customer(){Email = loginCustomerRequest.Email, Name = "John Smith", Guid = Guid.NewGuid().ToString()};

            Mock<ICustomerRepository> customerRepositoryMock = new Mock<ICustomerRepository>();
            CustomerController customerController = new CustomerController(null, customerRepositoryMock.Object);
            customerRepositoryMock.Setup(r => r.Login(loginCustomerRequest.Email, loginCustomerRequest.Password)).Returns(customer);

            //Act
            var response = customerController.AuthenticateCustomer(loginCustomerRequest);

            //Assert
            var loginCustomerResponse = JsonConvert.DeserializeObject<LoginCustomerResponseV1>(response.Result.Content.ReadAsStringAsync().Result);
            Assert.IsNotNull(response.Result.Content);
            Assert.AreEqual(customer.Guid, loginCustomerResponse.Guid);
            Assert.AreEqual(customer.Email, loginCustomerResponse.Email);
            Assert.AreEqual(customer.Name, loginCustomerResponse.Name);
            Assert.AreEqual(response.Result.StatusCode, HttpStatusCode.OK);
        }
    }
}
