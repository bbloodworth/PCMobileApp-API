using System.Reflection;
using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

using CchWebAPI.Controllers;
using CchWebAPI.Filters;
using CchWebAPI.IdCards.Data;
using CchWebAPI.IdCards.Dispatchers;
using CchWebAPI.Employees.Data;
using CchWebAPI.Employees.Dispatchers;
using CchWebAPI.BenefitContributions.Data;
using CchWebAPI.BenefitContributions.Dispatchers;
using System.Web.Mvc;

namespace CchWebAPI {
    public class AutofacConfig {
        public static void Register(HttpConfiguration config) {
            var builder = new ContainerBuilder();
            //Repositories
            builder.RegisterType<IdCardsRepository>().As<IIdCardsRepository>();
            builder.RegisterType<EmployeesRepository>().As<IEmployeesRepository>();
            builder.RegisterType<ContributionsRepository>().As<IContributionsRepository>();

            //Dispatchers
            builder.RegisterType<IdCardsDispatcher>().As<IIdCardsDispatcher>()
                .UsingConstructor(typeof(IIdCardsRepository));
            builder.RegisterType<EmployeesDispatcher>().As<IEmployeesDispatcher>()
                .UsingConstructor(typeof(IEmployeesRepository));
            builder.RegisterType<ContributionsDispatcher>().As<IContributionsDispatcher>()
                .UsingConstructor(typeof(IContributionsRepository));

            //Controllers
            builder.RegisterType<IdCardsController>()
                .UsingConstructor(typeof(IIdCardsDispatcher))
                .InstancePerRequest();
            builder.RegisterType<EmployeesController>()
                .UsingConstructor(typeof(IEmployeesDispatcher))
                .InstancePerRequest();
            builder.RegisterType<ContributionsController>()
                .UsingConstructor(typeof(IContributionsDispatcher))
                .InstancePerRequest();

            //Filters
            builder.RegisterWebApiFilterProvider(config);

            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<IdCardsController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<EmployeesController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<ContributionsController>().InstancePerRequest();

            var container = builder.Build();

            //WebApi
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}