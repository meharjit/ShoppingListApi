using System.Collections.Generic;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.Repositories
{
    public interface ICustomerRepository
    {
        void SaveCustomer(CreateCustomerRequestV1 customerObject, string password);
        Customer GetCustomer(string customerGuid);
        Customer GetCustomer(int customerId);
        Customer Login(string email, string password);
    }

    public interface IListRepository
    {
        void SaveList(string customerGuid, CreateListRequestV1 createListRequest);
        ListBase GetListForCustomer(string customerGuid, string listGuid);
        List<ListBase> GetListsForCustomer(string customerGuid);
    }
}