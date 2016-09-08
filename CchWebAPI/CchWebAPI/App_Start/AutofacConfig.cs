using System.Reflection;
using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

using CchWebAPI.Controllers;
using CchWebAPI.IdCards.Data;
using CchWebAPI.IdCards.Dispatchers;

namespace CchWebAPI {
    public class AutofacConfig {
        public static void Register(HttpConfiguration config) {
            var builder = new ContainerBuilder();

            //Repositories
            builder.RegisterType<IdCardsRepository>().As<IIdCardsRepository>();

            //Dispatchers
            builder.RegisterType<IdCardsDispatcher>().As<IIdCardsDispatcher>()
                .UsingConstructor(typeof(IIdCardsRepository));

            //Controllers
            builder.RegisterType<IdCardsController>()
                .UsingConstructor(typeof(IIdCardsDispatcher))
                .InstancePerRequest();

            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}