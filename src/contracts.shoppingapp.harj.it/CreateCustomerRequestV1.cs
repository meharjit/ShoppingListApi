namespace contracts.shoppingapp.harj.it
{
    public class CustomerBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
    }

    public class CreateCustomerRequestV1 : CustomerBase
    {
        public string Password { get; set; }
    }

    public class LoginCustomerRequestV1
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCustomerResponseV1 : CustomerBase
    {
    }

    public class GetCustomerResponseV1 : CustomerBase { }

    public class Customer : CustomerBase
    {
    }

    public class ListBase
    {
        public string Name { get; set; }
    }

    public class CreateListRequestV1 : ListBase
    {
    }
    public class GetListResponseV1 : CreateListRequestV1
    {
    }
    public class AddItemToListRequestV1
    {

    }
}