using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using contracts.shoppingapp.harj.it;
using NUnit.Framework;
using RestSharp;

namespace api.shoppingapp.harj.it.acceptancetests
{
    [TestFixture]
    public class CustomerControllerShould
    {
        [Test]
        public void CreateACustomerWhenRequested()
        {
            var createCustomerRequest = new CreateCustomerRequestV1()
            {
                Email = Guid.NewGuid() + "test@example.com",
                Password = "password",
                Name = "John Smith",
                Guid = Guid.NewGuid().ToString()
            };

            // given i create a customer
            var createCustomerResponse = CreateCustomer(createCustomerRequest);
            Assert.AreEqual(HttpStatusCode.Created, createCustomerResponse.StatusCode);

            // when i try to request the same customer
            var getCustomerResponse = GetCustomerByGuid(createCustomerRequest.Guid);

            // then the response is http status code 404
            Assert.AreEqual(HttpStatusCode.NotFound, getCustomerResponse.StatusCode);
        }

        [Test]
        public void LogInANewlyCreatedCustomer()
        {
            var createCustomerRequest = new CreateCustomerRequestV1()
            {
                Email = Guid.NewGuid() + "test@example.com",
                Password = "password",
                Name = "John Smith",
                Guid = Guid.NewGuid().ToString()
            };

            // given create a customer
            var createCustomerResponse = CreateCustomer(createCustomerRequest);
            Assert.AreEqual(HttpStatusCode.Created, createCustomerResponse.StatusCode);

            // and i authenticate that customer
            IRestResponse<LoginCustomerResponseV1> loginCustomerResponse = LoginToCustomer(createCustomerRequest.Email, createCustomerRequest.Password);
            Assert.AreEqual(HttpStatusCode.OK, loginCustomerResponse.StatusCode);

            // when i try to request the same customer using the authenticate payload
            var getCustomerResponse = GetCustomerByGuid(loginCustomerResponse.Data.Guid);

            // then the response is http status code 200
            Assert.AreEqual(HttpStatusCode.OK, getCustomerResponse.StatusCode);
        }

        private IRestResponse<LoginCustomerResponseV1> LoginToCustomer(string email, string password)
        {
            Uri HostUrl = new Uri("http://api.shoppingapp.harj.it/authenticate/");
            string CreateCustomerPath = "";

            var restClient = new RestClient(HostUrl);
            var request = new RestRequest(Method.POST);
            request.Resource = CreateCustomerPath;
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new LoginCustomerRequestV1() {Email = email, Password = password});
            var response = restClient.Execute<LoginCustomerResponseV1>(request);
            return response;
        }

        private IRestResponse<GetCustomerResponseV1> GetCustomerByGuid(string guid)
        {
            Uri HostUrl = new Uri("http://api.shoppingapp.harj.it/customers/"+guid);
            string CreateCustomerPath = "";

            var restClient = new RestClient(HostUrl);
            var request = new RestRequest(Method.GET);
            request.Resource = CreateCustomerPath;
            request.RequestFormat = DataFormat.Json;
            var response = restClient.Execute<GetCustomerResponseV1>(request);
            return response;
        }

        private static IRestResponse<CreateCustomerRequestV1> CreateCustomer(CreateCustomerRequestV1 createCustomerRequest)
        {
            Uri HostUrl = new Uri("http://api.shoppingapp.harj.it/customers");
            string CreateCustomerPath = "";

            var restClient = new RestClient(HostUrl);
            var request = new RestRequest(Method.POST);
            request.Resource = CreateCustomerPath;
            request.RequestFormat = DataFormat.Json;
            request.AddBody(createCustomerRequest);
            var response = restClient.Execute<CreateCustomerRequestV1>(request);
            return response;
        }
    }
}
