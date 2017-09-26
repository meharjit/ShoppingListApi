using System.Collections.Generic;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.Validators
{
    public interface ICreateCustomerRequestValidatorV1
    {
        List<string> GetValidationErrors(CreateCustomerRequestV1 customerRequest);
    }
}