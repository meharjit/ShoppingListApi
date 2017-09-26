using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace api.shoppingapp.harj.it
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //http://localhost:12989/api/customer/12345

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            ////add and update store
            //config.Routes.MapHttpRoute("Store", "api/store/{id}", new { id = RouteParameter.Optional, controller = "Store" });
            ////add update customer
            //config.Routes.MapHttpRoute("Customer", "api/customer/{id}", new { id = RouteParameter.Optional, controller = "Customer"});
            ////add update list container
            //config.Routes.MapHttpRoute("List", "api/customer/{custid}/list/{id}", new { id = RouteParameter.Optional, controller = "List"});
            ////get list in store order
            //config.Routes.MapHttpRoute("StoreList", "api/customer/{custid}/list/{listid}/store/{id}", new { id = RouteParameter.Optional, controller = "StoreList"});
            ////add item to list, delete, patch as updated
            //config.Routes.MapHttpRoute("Item", "api/customer/{custid}/List/{listid}/item/{id}", new { id = RouteParameter.Optional, controller = "Item"});
        }
    }
}
