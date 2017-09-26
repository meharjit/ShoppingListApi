using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using api.shoppingapp.harj.it.Domain.Repositories;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Controllers
{
    public class ListController : ApiController
    {
        private readonly IListRepository _listRepositoryObject;

        public ListController(IListRepository listRepositoryObject)
        {
            _listRepositoryObject = listRepositoryObject;
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists")]
        public Task<HttpResponseMessage> CreateList([FromUri] string customerGuid, [FromBody] CreateListRequestV1 createListRequest)
        {
            if (createListRequest == null || string.IsNullOrEmpty(customerGuid) || string.IsNullOrEmpty(createListRequest.Name))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
            _listRepositoryObject.SaveList(customerGuid,createListRequest);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists/{listGuid}")]
        public Task<HttpResponseMessage> GetCustomerListByGuid([FromUri] string customerGuid, [FromUri] string listGuid)
        {
            var listResponse = _listRepositoryObject.GetListForCustomer(customerGuid, listGuid);
            var mappedListResponse = (listResponse == null) ? null : new GetListResponseV1() {Name = listResponse.Name};
            var httpStatusCode = (listResponse == null) ? HttpStatusCode.NotFound : HttpStatusCode.OK;
            return Task.FromResult(new HttpResponseMessage(httpStatusCode)
            {
                Content = new ObjectContent(typeof(GetListResponseV1), mappedListResponse,
                    new JsonMediaTypeFormatter())
            });
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists")]
        public Task<HttpResponseMessage> GetCustomersLists([FromUri] string customerGuid)
        {
            var listResponse = _listRepositoryObject.GetListsForCustomer(customerGuid);
            var mappedListResponse = listResponse.Select(e=>new GetListResponseV1(){Name = e.Name}).ToList();
            var httpStatusCode = HttpStatusCode.OK;
            return Task.FromResult(new HttpResponseMessage(httpStatusCode)
            {
                Content = new ObjectContent(typeof(List<GetListResponseV1>), mappedListResponse,
                    new JsonMediaTypeFormatter())
            });
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists/{listGuid}")]
        public Task<HttpResponseMessage> DeleteList([FromUri] string customerGuid, [FromUri] string listGuid)
        {
            return null;
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists/{listGuid}")]
        public Task<HttpResponseMessage> PostItemToList([FromUri] string customerGuid, [FromUri] string listGuid, [FromBody] AddItemToListRequestV1 addItemToListRequest)
        {
            var httpStatusCode = HttpStatusCode.NotFound;
            return Task.FromResult(new HttpResponseMessage(httpStatusCode)
            {
                Content = new ObjectContent(typeof(CreateListRequestV1), null, 
                    new JsonMediaTypeFormatter())
            });
        }

        [HttpPost]
        [Route("customers/{customerGuid}/lists/{listGuid}/{itemGuid}")]
        public Task<HttpResponseMessage> DeleteItemFromList([FromUri] string customerGuid, [FromUri] string listGuid, [FromUri] string itemGuid)
        {
            return null;
        }
        // add item to existing list
        // delete item from list
        // add item to new, non existent list
        //  - this will check the token, to confirm the list belongs to the customer
        //  - 

        //add to list (string customerGuid, string itemGuid)
        // if the list exists
    }
}
