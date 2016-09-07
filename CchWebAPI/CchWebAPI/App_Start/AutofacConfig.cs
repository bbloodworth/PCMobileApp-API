using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

using CchWebAPI.Areas.v2.Controllers;
using CchWebAPI.Areas.v2.IdCards.Data;
using CchWebAPI.Areas.v2.IdCards.Dispatchers;

namespace CchWebAPI {
    public class AutofacConfig {
        public static void ConfigureContainer(HttpConfiguration config) {
            var builder = new ContainerBuilder();

            //Data Providers
            builder.RegisterType<IdCardsRepository>().As<IIdCardsRepository>();

            //Dispatchers
            builder.RegisterType<IdCardsDispatcher>().As<IIdCardsDispatcher>()
                .UsingConstructor(typeof(IIdCardsRepository));

            //Controllers
            builder.RegisterType<IdCardsController>().UsingConstructor(typeof(IIdCardsDispatcher));

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}