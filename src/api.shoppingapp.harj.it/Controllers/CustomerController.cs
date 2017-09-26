using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using api.shoppingapp.harj.it.Domain.DataStores;
using api.shoppingapp.harj.it.Domain.Repositories;
using api.shoppingapp.harj.it.Domain.Validators;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ICreateCustomerRequestValidatorV1 _createCustomerRequestValidatorV1;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController() : this(new CreateCustomerRequestValidatorV1(), new CustomerRepository(new FileCustomerDataStore("e:\\db\\")))
        {
        }

        public CustomerController(ICreateCustomerRequestValidatorV1 createCustomerRequestValidatorV1, ICustomerRepository customerRepository)
        {
            _createCustomerRequestValidatorV1 = createCustomerRequestValidatorV1;
            _customerRepository = customerRepository;
        }

        [HttpPost]
        [HttpPut]
        [Route("customers")]
        public Task<HttpResponseMessage> UpsertRecord([FromBody]CreateCustomerRequestV1 customerObject)
        {
            var validationErrors = _createCustomerRequestValidatorV1.GetValidationErrors(customerObject);
            if (validationErrors.Any())
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
            try
            {
                _customerRepository.SaveCustomer(customerObject, customerObject.Password);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));
            }
            catch (ArgumentException)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
        }

        [HttpGet]
        [Route("customers/{customerGuid}")]
        public Task<HttpResponseMessage> GetCustomer([FromUri]string customerGuid)
        {
            var customer = _customerRepository.GetCustomer(customerGuid);
            if (customer == null)
            {

                return Task.FromResult(new HttpResponseMessage()
                {StatusCode = HttpStatusCode.NotFound
                });
            }
            return Task.FromResult(new HttpResponseMessage()
            {
                Content = new ObjectContent(typeof(GetCustomerResponseV1),
                    new GetCustomerResponseV1() {Email = customer.Email, Guid = customer.Guid, Name = customer.Name},
                    new JsonMediaTypeFormatter())
            });
        }

        [HttpPost]
        [Route("authenticate")]
        public Task<HttpResponseMessage> AuthenticateCustomer([FromBody]LoginCustomerRequestV1 loginCustomerRequest)
        {
            var customer = _customerRepository.Login(loginCustomerRequest.Email, loginCustomerRequest.Password);
            if (customer == null)
            {
                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized
                });
            }
            return Task.FromResult(new HttpResponseMessage()
            {
                Content = new ObjectContent(typeof(LoginCustomerResponseV1),
                    new LoginCustomerResponseV1() {Email = customer.Email, Guid = customer.Guid, Name = customer.Name},
                    new JsonMediaTypeFormatter())
            });
        }
    }
}
