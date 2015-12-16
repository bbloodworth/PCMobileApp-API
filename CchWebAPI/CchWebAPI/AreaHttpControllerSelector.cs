using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CchWebAPI
{
    public class AreaHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;
        private new const String ControllerSuffix = "Controller";
        private const String AreaRouteVariableName = "area";
        private Dictionary<String, Type> _apiControllerTypes;

        private Dictionary<String, Type> ApiControllerTypes
        {
            get { return _apiControllerTypes ?? (_apiControllerTypes = GetControllerTypes()); }
        }

        public AreaHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        private static Dictionary<String, Type> GetControllerTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract
                                                         && t.Name.EndsWith(ControllerSuffix)
                                                         && typeof (IHttpController).IsAssignableFrom(t)))
                .ToDictionary(t => t.FullName, t => t);
            return types;
        }

        public override HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
        {
            return GetApiController(request) ?? base.SelectController(request);
        }

        private static String GetAreaName(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            if (!data.Values.ContainsKey(AreaRouteVariableName)) return null;
            return data.Values[AreaRouteVariableName].ToString().ToLower();
        }

        private Type GetControllerTypeByArea(String areaName, String controllerName)
        {
            var areaNameToFind = String.Format(".{0}.", areaName.ToLower());
            var controllerNameToFind = String.Format(".{0}{1}", controllerName, ControllerSuffix);
            return ApiControllerTypes.Where(t => t.Key.ToLower().Contains(areaNameToFind)
                                                 &&
                                                 t.Key.EndsWith(controllerNameToFind, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Value).FirstOrDefault();
        }

        private HttpControllerDescriptor GetApiController(HttpRequestMessage request)
        {
            var controllerName = base.GetControllerName(request);
            var areaName = GetAreaName(request);
            if (String.IsNullOrEmpty(areaName)) return null;
            var type = GetControllerTypeByArea(areaName, controllerName);
            if (type == null) return null;
            return new HttpControllerDescriptor(_configuration, controllerName, type);
        }
    }
}