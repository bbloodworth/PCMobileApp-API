using System.Reflection;
using System.Web.Http;
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
using CchWebAPI.EmployeeDW.Data;
using CchWebAPI.EmployeeDW.Dispatchers;
using CchWebAPI.PaidTimeOff.Data;
using CchWebAPI.PaidTimeOff.Dispatchers;
using CchWebAPI.Payrolls.Data;
using CchWebAPI.Payrolls.Dispatchers;

namespace CchWebAPI {
    public class AutofacConfig {
        public static void Register(HttpConfiguration config) {
            var builder = new ContainerBuilder();
            //Repositories
            builder.RegisterType<IdCardsRepository>().As<IIdCardsRepository>();
            builder.RegisterType<EmployeesRepository>().As<IEmployeesRepository>();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>();
            builder.RegisterType<PayrollRepository>().As<IPayrollRepository>();
            builder.RegisterType<BenefitContributionsRepository>().As<IBenefitContributionsRepository>();
            builder.RegisterType<PaidTimeOffRepository>().As<IPaidTimeOffRepository>();

            //Dispatchers
            builder.RegisterType<IdCardsDispatcher>().As<IIdCardsDispatcher>()
                .UsingConstructor(typeof(IIdCardsRepository));
            builder.RegisterType<EmployeesDispatcher>().As<IEmployeesDispatcher>()
                .UsingConstructor(typeof(IEmployeesRepository));
            builder.RegisterType<EmployeeDispatcher>().As<IEmployeeDispatcher>()
                .UsingConstructor(typeof(IEmployeeRepository));
            builder.RegisterType<PayrollDispatcher>().As<IPayrollDispatcher>()
                .UsingConstructor(typeof(IPayrollRepository));
            builder.RegisterType<ContributionsDispatcher>().As<IBenefitContributionsDispatcher>()
                .UsingConstructor(typeof(IBenefitContributionsRepository));
            builder.RegisterType<PaidTimeOffDispatcher>().As<IPaidTimeOffDispatcher>()
                .UsingConstructor(typeof(IPaidTimeOffRepository));

            //Controllers
            builder.RegisterType<IdCardsController>()
                .UsingConstructor(typeof(IIdCardsDispatcher))
                .InstancePerRequest();
            builder.RegisterType<EmployeesController>()
                .UsingConstructor(typeof(IEmployeesDispatcher))
                .InstancePerRequest();
            builder.RegisterType<EmployeesController>()
                .UsingConstructor(typeof(IEmployeeDispatcher))
                .InstancePerRequest();
            builder.RegisterType<PayrollController>()
                .UsingConstructor(typeof(IPayrollDispatcher))
                .InstancePerRequest();
            builder.RegisterType<BenefitContributionsController>()
                .UsingConstructor(typeof(IBenefitContributionsDispatcher))
                .InstancePerRequest();
            builder.RegisterType<PaidTimeOffController>()
                .UsingConstructor(typeof(IPaidTimeOffDispatcher))
                .InstancePerRequest();

            //Filters
            builder.RegisterWebApiFilterProvider(config);

            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<IdCardsController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<EmployeesController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<BenefitContributionsController>().InstancePerRequest();

            var container = builder.Build();

            //WebApi
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}