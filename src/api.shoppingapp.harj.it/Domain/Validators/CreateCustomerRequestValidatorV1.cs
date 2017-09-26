using System;
using System.Collections.Generic;
using contracts.shoppingapp.harj.it;

namespace api.shoppingapp.harj.it.Domain.Validators
{
    public class CreateCustomerRequestValidatorV1 : ICreateCustomerRequestValidatorV1
    {
        public List<string> GetValidationErrors(CreateCustomerRequestV1 customerRequest)
        {
            List<string> validationErrors = new List<string>();
            if (IsNameValid(customerRequest.Name))
            {
                validationErrors.Add("CreateCustomerRequestV1.Name");
            }
            if (IsEmailValid(customerRequest.Email))
            {
                validationErrors.Add("CreateCustomerRequestV1.Email");
            }
            return validationErrors;
        }

        private static bool IsNameValid(string name)
        {
            return string.IsNullOrEmpty(name);
        }

        private static bool IsEmailValid(string email)
        {
            return email == null || !email.Contains("@");
        }
    }
}