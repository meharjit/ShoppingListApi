using System;
using api.shoppingapp.harj.it.Domain.DataStores;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICustomerDataStore _customerDataStore;

        public CustomerRepository(ICustomerDataStore customerDataStore)
        {
            _customerDataStore = customerDataStore;
        }

        public Customer GetCustomer(int customerId)
        {
            throw new NotImplementedException();
        }

        public Customer Login(string email, string password)
        {
            return _customerDataStore.GetCustomerByEmailAndPassword(email, password);
        }

        public void SaveCustomer(CreateCustomerRequestV1 customerObject, string password = null)
        {
            var findCustomerByEmailResult = _customerDataStore.FindCustomerByEmail(customerObject.Email);
            if (findCustomerByEmailResult.Count == 0)
            {
                _customerDataStore.Write(customerObject.Name, customerObject.Email, customerObject.Password);
            }
            else if (findCustomerByEmailResult[0] == customerObject.Guid)
            {
                //TODO: add a test case where this function is called to update (i.e the customer exists), but password is passed in as null
                // should either bomb out with validation error
                // or preferably re seed the password from the data layer.
                // and lets not forget about password hashing. MD5 for now should be sufficient.
                // Make sure we can increase the security... implement an IEncryptPassword interface
                _customerDataStore.Update(customerObject.Guid, customerObject.Name, customerObject.Email, password);
            }
            else
            {
                throw new ArgumentException("Customer already exists with matching email and guid.");
            }
        }

        public Customer GetCustomer(string customerGuid)
        {
            return _customerDataStore.GetCustomerByGuid(customerGuid);
        }
    }
}