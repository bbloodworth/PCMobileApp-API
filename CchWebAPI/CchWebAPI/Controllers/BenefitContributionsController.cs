﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CchWebAPI.BenefitContributions.Models;
using CchWebAPI.BenefitContributions.Dispatchers;
using ClearCost.Platform;
using ClearCost.Net;
using System.Threading.Tasks;


namespace CchWebAPI.Controllers
{
    public class BenefitContributionsController : ApiController
    {
        IBenefitContributionsDispatcher _dispatcher;

        public BenefitContributionsController(IBenefitContributionsDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public BenefitContributionsController() { }

        [HttpGet]
        public async Task<ApiResult<BenefitContribution>> Get (string categoryCode)
        {
            
            var result = await _dispatcher.GetContributionsByCchIdAsync(
                Request.CCHID(),
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())),
                categoryCode);

            return ApiResult<BenefitContribution>.ValidResult(result, string.Empty);
        }

    }
}