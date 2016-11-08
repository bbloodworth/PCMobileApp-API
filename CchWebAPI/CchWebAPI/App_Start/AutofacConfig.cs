﻿using System.Reflection;
using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

using CchWebAPI.Controllers;
using CchWebAPI.Filters;
using CchWebAPI.IdCard.Data;
using CchWebAPI.IdCard.Dispatchers;
using CchWebAPI.Employees.Data;
using CchWebAPI.Employees.Dispatchers;
using CchWebAPI.BenefitContribution.Data;
using CchWebAPI.BenefitContribution.Dispatchers;
using System.Web.Mvc;
using CchWebAPI.Employee.Data;
using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Payroll.Data;
using CchWebAPI.Payroll.Dispatchers;
using CchWebAPI.MedicalPlan.Data;
using CchWebAPI.MedicalPlan.Dispatchers;

namespace CchWebAPI {
    public class AutofacConfig {
        public static void Register(HttpConfiguration config) {
            var builder = new ContainerBuilder();
            //Repositories
            builder.RegisterType<IdCardsRepository>().As<IIdCardsRepository>();
            builder.RegisterType<EmployeesRepository>().As<IEmployeesRepository>();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>();
            builder.RegisterType<PayrollRepository>().As<IPayrollRepository>();
            builder.RegisterType<ContributionsRepository>().As<IContributionsRepository>();
            builder.RegisterType<MedicalPlanRepository>().As<IMedicalPlanRepository>();

            //Dispatchers
            builder.RegisterType<IdCardsDispatcher>().As<IIdCardsDispatcher>()
                .UsingConstructor(typeof(IIdCardsRepository));
            builder.RegisterType<EmployeesDispatcher>().As<IEmployeesDispatcher>()
                .UsingConstructor(typeof(IEmployeesRepository));
            builder.RegisterType<EmployeeDispatcher>().As<IEmployeeDispatcher>()
                .UsingConstructor(typeof(IEmployeeRepository));
            builder.RegisterType<PayrollDispatcher>().As<IPayrollDispatcher>()
                .UsingConstructor(typeof(IPayrollRepository));
            builder.RegisterType<ContributionsDispatcher>().As<IContributionsDispatcher>()
                .UsingConstructor(typeof(IContributionsRepository));
            builder.RegisterType<MedicalPlanDispatcher>().As<IMedicalPlanDispatcher>()
                .UsingConstructor(typeof(IMedicalPlanRepository));

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
            builder.RegisterType<ContributionsController>()
                .UsingConstructor(typeof(IContributionsDispatcher))
                .InstancePerRequest();
            builder.RegisterType<MedicalPlansController>()
                .UsingConstructor(typeof(IMedicalPlanDispatcher))
                .InstancePerRequest();

            //Filters
            builder.RegisterWebApiFilterProvider(config);

            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<IdCardsController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<EmployeesController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<ContributionsController>().InstancePerRequest();
            builder.Register(c => new V2AuthenticatedAuthorizationFilter())
                .AsWebApiAuthorizationFilterFor<MedicalPlansController>().InstancePerRequest();

            var container = builder.Build();

            //WebApi
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}