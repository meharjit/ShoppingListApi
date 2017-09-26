using System.Web;
using System.Web.Mvc;

namespace api.shoppingapp.harj.it
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
