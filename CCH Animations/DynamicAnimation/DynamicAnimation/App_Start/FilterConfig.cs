using System.Web;
using System.Web.Mvc;
using DynamicAnimation.Common;

namespace DynamicAnimation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionFilter());
        }
    }
}
