using BanPhimCanhCach.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BanPhimCanhCach.ExtendMethods
{
    public static class HtmlHelperExtend
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string action, string controller)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeController = (string)routeData.Values["controller"];

            var isActive = string.Equals(controller, routeController, StringComparison.InvariantCultureIgnoreCase)
                           && string.Equals(action, routeAction, StringComparison.InvariantCultureIgnoreCase);

            return isActive ? "active" : null;
        }

        public static string OrderStatusElement(this IHtmlHelper htmlHelper, string status)
        {
            string elementClass;
            if(status == OrderStatus.Delivered) {
                elementClass = "success";
            }
            else if(status == OrderStatus.Cancelled)
            {
                elementClass = "danger";
            }
            else
            {
                elementClass = "warning";
            }
            return $"<p><i class=\"fas fa-circle text-{elementClass}\"></i> {status}</p>";
        }
    }
    
}

