using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using api.shoppingapp.harj.it.Controllers;
using api.shoppingapp.harj.it.Domain.Repositories;
using contracts.shoppingapp.harj.it;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace api.shoppingapp.harj.it.unittests.Controllers
{
    [TestFixture]
    public class ListControllerWhen
    {
        [TestFixture]
        public class CreatingAListShould
        {
            private string customerGuid;
            private Mock<IListRepository> listRepository;
            private ListController controller;
            private CreateListRequestV1 createListRequest;

            [SetUp]
            public void Set_Up()
            {
                createListRequest = new CreateListRequestV1() { Name = "My new List" };
                listRepository = new Mock<IListRepository>();
                controller = new ListController(listRepository.Object);
                customerGuid = Guid.NewGuid().ToString();
                listRepository.Setup(e => e.SaveList(customerGuid, createListRequest));
            }

            [Test]
            public void Be_Able_To_Create_A_New_List_And_Return_Created_Status_Code()
            {
                var respose = controller.CreateList(customerGuid, createListRequest).Result;

                Assert.AreEqual(HttpStatusCode.Created, respose.StatusCode);
                listRepository.Verify(e => e.SaveList(customerGuid, createListRequest), Times.Once);
            }

            [TestCase("")]
            [TestCase(null)]
            public void Return_Bad_Request_When_Customer_Guid_Is_Empty(string _customerGuid)
            {
                var respose = controller.CreateList(_customerGuid, createListRequest).Result;

                Assert.AreEqual(HttpStatusCode.BadRequest, respose.StatusCode);
                listRepository.Verify(e => e.SaveList(It.IsAny<string>(), It.IsAny<CreateListRequestV1>()), Times.Never);
            }

            [TestCase("")]
            [TestCase(null)]
            public void Return_Bad_Request_When_ListName_Guid_Is_Empty(string listName)
            {
                createListRequest.Name = listName;

                var respose = controller.CreateList(customerGuid, createListRequest).Result;

                Assert.AreEqual(HttpStatusCode.BadRequest, respose.StatusCode);
                listRepository.Verify(e => e.SaveList(It.IsAny<string>(), It.IsAny<CreateListRequestV1>()), Times.Never);
            }

            [Test]
            public void Return_Bad_Request_When_List_isnt_Passed_In()
            {
                createListRequest = null;

                var respose = controller.CreateList(customerGuid, createListRequest).Result;

                Assert.AreEqual(HttpStatusCode.BadRequest, respose.StatusCode);
                listRepository.Verify(e => e.SaveList(It.IsAny<string>(), It.IsAny<CreateListRequestV1>()), Times.Never);
            }
        }

        [TestFixture]
        public class GettingACustomersListShould
        {
            [Test]
            public void Call_The_List_Repository_And_Return_A_List()
            {
                var listGuid = Guid.NewGuid().ToString();
                var customerGuid = Guid.NewGuid().ToString();
                var myListName = "my_list_name";

                Mock<IListRepository> listRepositoryObject = new Mock<IListRepository>();
                ListController controller = new ListController(listRepositoryObject.Object);
                listRepositoryObject.Setup(x => x.GetListForCustomer(customerGuid, listGuid))
                    .Returns(new ListBase() { Name = myListName });

                var response = controller.GetCustomerListByGuid(customerGuid, listGuid).Result;

                var getCustomerResponseV1 = JsonConvert.DeserializeObject<GetListResponseV1>(response.Content.ReadAsStringAsync().Result);

                listRepositoryObject.Verify(x => x.GetListForCustomer(customerGuid, listGuid), Times.Once);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(getCustomerResponseV1);
                Assert.AreEqual(getCustomerResponseV1.Name, myListName);
            }

            [Test]
            public void Call_The_List_Repository_Return_Null_When_No_list()
            {
                var listGuid = Guid.NewGuid().ToString();
                var customerGuid = Guid.NewGuid().ToString();
                var myListName = "my_list_name";

                Mock<IListRepository> listRepositoryObject = new Mock<IListRepository>();
                ListController controller = new ListController(listRepositoryObject.Object);
                listRepositoryObject.Setup(x => x.GetListForCustomer(customerGuid, listGuid))
                    .Returns((ListBase)null);

                var response = controller.GetCustomerListByGuid(customerGuid, listGuid).Result;

                var getCustomerResponseV1 = JsonConvert.DeserializeObject<GetListResponseV1>(response.Content.ReadAsStringAsync().Result);

                listRepositoryObject.Verify(x => x.GetListForCustomer(customerGuid, listGuid), Times.Once);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                Assert.IsNull(getCustomerResponseV1);
            }
        }

        [TestFixture]
        public class GettingACustomersListsShould
        {
            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public void BeAbleToReturnACollectionOfListsForACustomer(int numberOfItemsInList)
            {
                var customerGuid = Guid.NewGuid().ToString();
                var myListName = "my_list_name";
                List<ListBase> listsFromRepo = new List<ListBase>();
                for (int i = 0; i < numberOfItemsInList; i++)
                {
                    listsFromRepo.Add(new ListBase(){Name = myListName+i});
                }
                Mock<IListRepository> listRepositoryObject = new Mock<IListRepository>();
                ListController controller = new ListController(listRepositoryObject.Object);
                listRepositoryObject.Setup(x => x.GetListsForCustomer(customerGuid))
                    .Returns(listsFromRepo);

                var response = controller.GetCustomersLists(customerGuid).Result;

                var customersLists = JsonConvert.DeserializeObject<List<GetListResponseV1>>(response.Content.ReadAsStringAsync().Result);

                listRepositoryObject.Verify(x => x.GetListsForCustomer(customerGuid), Times.Once);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(customersLists);
                Assert.AreEqual(customersLists.Count, listsFromRepo.Count);
            }
            
        }

        [TestFixture]
        public class AddAndItemToListShould
        {
            public void Should_be_able_to_add_item()
            {
            }

            [Test]
            public void Return_not_found_if_there_are_no_matching_lists()
            {
                var nonExistentCustomerGuid = "customer_doesnt_exist";
                var listGiuid = Guid.NewGuid().ToString();

                Mock<IListRepository> listRepositoryObject = new Mock<IListRepository>();
                ListController controller = new ListController(listRepositoryObject.Object);
                listRepositoryObject.Setup(x => x.GetListForCustomer(nonExistentCustomerGuid, listGiuid)).Returns((CreateListRequestV1)null);
                
                var response = controller.PostItemToList(nonExistentCustomerGuid, listGiuid, new AddItemToListRequestV1()).Result;

                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
