using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.shoppingapp.harj.it.Domain.DataStores;
using api.shoppingapp.harj.it.Domain.Repositories;
using contracts.shoppingapp.harj.it;
using Moq;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace api.shoppingapp.harj.it.unittests.Domain.Repositories
{
    [TestFixture]
    public class CustomerRepositoryShould
    {
        [Test]
        public void Write_to_disk_when_no_customers_exist_in_datastore()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1();
            Mock<ICustomerDataStore> customerDataStoreMock = new Mock<ICustomerDataStore>();
            CustomerRepository customerRepository = new CustomerRepository(customerDataStoreMock.Object);
            customerDataStoreMock.Setup(r => r.FindCustomerByEmail(customerObject.Email)).Returns(new List<string>());
            customerDataStoreMock.Setup(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            customerRepository.SaveCustomer(customerObject);

            // Assert
            customerDataStoreMock.Verify(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }

        [Test]
        public void Not_save_to_disk_but_should_update_when_email_exists_in_data_store_and_guid_matches()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1() { Guid = "customer_actual_guid" };
            Mock<ICustomerDataStore> customerDataStoreMock = new Mock<ICustomerDataStore>();
            CustomerRepository customerRepository = new CustomerRepository(customerDataStoreMock.Object);
            customerDataStoreMock.Setup(r => r.FindCustomerByEmail(customerObject.Email)).Returns(new List<string> { "customer_actual_guid" });
            customerDataStoreMock.Setup(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            customerDataStoreMock.Setup(r => r.Update(customerObject.Guid, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("customer_actual_guid");

            // Act
            customerRepository.SaveCustomer(customerObject);

            // Assert
            customerDataStoreMock.Verify(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            customerDataStoreMock.Verify(r => r.Update(customerObject.Guid, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Not_save_to_disk_or_update_when_email_exists_in_data_store_and_guid_doesnt_match()
        {
            // Arrange
            CreateCustomerRequestV1 customerObject = new CreateCustomerRequestV1() { Guid = "customer_actual_guid" };
            Mock<ICustomerDataStore> customerDataStoreMock = new Mock<ICustomerDataStore>();
            CustomerRepository customerRepository = new CustomerRepository(customerDataStoreMock.Object);
            customerDataStoreMock.Setup(r => r.FindCustomerByEmail(customerObject.Email)).Returns(new List<string> { "existing_customer_guid" });
            customerDataStoreMock.Setup(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            customerDataStoreMock.Setup(r => r.Update(customerObject.Guid, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("customer_actual_guid");
            bool exceptionRaised = false;

            // Act
            try
            {
                customerRepository.SaveCustomer(customerObject);
            }
            catch (ArgumentException e)
            {
                exceptionRaised = true;
                Assert.AreEqual("Customer already exists with matching email and guid.", e.Message);
            }

            // Assert
            customerDataStoreMock.Verify(r => r.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            customerDataStoreMock.Verify(r => r.Update(customerObject.Guid, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.IsTrue(exceptionRaised);
        }

        [Test]
        public void Return_null_when_customer_doesnt_exist()
        {
            //Arrange
            var customerGuid = Guid.NewGuid().ToString();
            Mock<ICustomerDataStore> customerDataStore = new Mock<ICustomerDataStore>();
            customerDataStore.Setup(x => x.GetCustomerByGuid(customerGuid)).Returns((Customer)null);
            ICustomerRepository repository = new CustomerRepository(customerDataStore.Object);
            
            //Act
            Customer customer = repository.GetCustomer(customerGuid);

            //Assert
            Assert.IsNull(customer);
        }

        [Test]
        public void Return_object_when_customer_exists()
        {
            //Arrange
            var customerGuid = Guid.NewGuid().ToString();
            Mock<ICustomerDataStore> customerDataStore = new Mock<ICustomerDataStore>();
            customerDataStore.Setup(x => x.GetCustomerByGuid(customerGuid)).Returns(new Customer());
            ICustomerRepository repository = new CustomerRepository(customerDataStore.Object);

            //Act
            Customer customer = repository.GetCustomer(customerGuid);

            //Assert
            Assert.IsNotNull(customer);
        }


        [Test]
        public void Return_null_when_email_password_pair_doesnt_exist()
        {
            //Arrange
            string email = "test@example.com";
            string password = "P@ssword1";

            var customerGuid = Guid.NewGuid().ToString();
            Mock<ICustomerDataStore> customerDataStore = new Mock<ICustomerDataStore>();
            customerDataStore.Setup(x => x.GetCustomerByEmailAndPassword(email, password)).Returns((Customer)null);
            ICustomerRepository repository = new CustomerRepository(customerDataStore.Object);

            //Act
            Customer customer = repository.Login(email, password);

            //Assert
            Assert.IsNull(customer);
        }
    }
}
