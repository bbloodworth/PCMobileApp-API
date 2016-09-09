using System.Web;
using System.Web.Http;

namespace CchWebAPI.Extensions {
    public static class ApiControllerExt {
        public static string Host(this ApiController controller) {
        if (controller.Request.Properties.ContainsKey("MS_HttpContext"))
            return ((HttpContextWrapper)controller.Request.Properties["MS_HttpContext"]).Request.Url.Host;

        return string.Empty;
    }
}
}