using System.Collections.Generic;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.DataStores
{
    public interface ICustomerDataStore
    {
        string Write(string name, string emaill, string password);
        List<string> FindCustomerByEmail(string email);
        string Update(string guid, string name, string emaill, string password);
        Customer GetCustomerByGuid(string customerGuid);
        Customer GetCustomerByEmailAndPassword(string email, string password);
        string GetPasswordForGuid(string customerObjectGuid);
    }
}