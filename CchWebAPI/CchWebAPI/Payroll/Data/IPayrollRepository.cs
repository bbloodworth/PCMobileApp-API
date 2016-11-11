﻿using CchWebAPI.Payroll.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.Payroll.Data {
    public interface IPayrollRepository {
        void Initialize(string connectionString);
        Task<List<DatePaid>> GetDatesPaidAsync(int cchId);
        Task<List<PaycheckDetails>> GetPaycheckAsync(string documentId);
    }

    public class PayrollRepository : IPayrollRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<DatePaid>> GetDatesPaidAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            List<DatePaid> datesPaid = new List<DatePaid>();

            using (var ctx = new PayrollContext(_connectionString)) {
                datesPaid = await ctx.Payrolls
                    .Join(
                        ctx.Employees,
                        payroll => payroll.EmployeeKey,
                        employee => employee.EmployeeKey,
                        (payroll, employee) => new {
                            Payroll = payroll,
                            Employee = employee
                        })
                    .Join(
                        ctx.Dates,
                        payroll => payroll.Payroll.PayDateKey,
                        date => date.DateKey,
                        (payroll, date) => new {
                            Payroll = payroll.Payroll,
                            Employee = payroll.Employee,
                            Date = date
                        })
                    .Where(
                        p => p.Employee.Cchid.Equals(cchId)
                    )
                    .Select(
                        p => new DatePaid {
                            CchId = p.Employee.Cchid,
                            PaycheckDate = p.Date.FullDate.Value,
                            DocumentId = p.Payroll.DocumentId
                        })
                    .Distinct()
                    .ToListAsync();
            }

            return datesPaid;
        }

        public async Task<List<PaycheckDetails>> GetPaycheckAsync(string documentId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            List<PaycheckDetails> payCheckDetails = new List<PaycheckDetails>();

            using (var ctx = new PayrollContext(_connectionString)) {
                payCheckDetails = await ctx.Payrolls
                    .Join(
                        ctx.Dates,
                        payroll => payroll.PayDateKey,
                        date => date.DateKey,
                        (payroll, date) => new {
                            Payroll = payroll,
                            Date = date
                        })
                    .Join(
                        ctx.Employees,
                        payroll => payroll.Payroll.EmployeeKey,
                        employee => employee.EmployeeKey,
                        (payroll, employee) => new {
                            Payroll = payroll.Payroll,
                            Date = payroll.Date,
                            Employee = employee
                        })
                    .Join(
                        ctx.DeliveryMethods,
                        payroll => payroll.Payroll.DeliveryMethodKey,
                        deliveryMethod => deliveryMethod.DeliveryMethodKey,
                        (payroll, deliveryMethod) => new {
                            Payroll = payroll.Payroll,
                            Date = payroll.Date,
                            Employee = payroll.Employee,
                            DeliveryMethod = deliveryMethod
                        })
                    .Join(
                        ctx.ContributionTypes,
                        payroll => payroll.Payroll.ContributionTypeKey,
                        contributionType => contributionType.ContributionTypeKey,
                        (payroll, contributionType) => new {
                            Payroll = payroll.Payroll,
                            Date = payroll.Date,
                            Employee = payroll.Employee,
                            DeliveryMethod = payroll.DeliveryMethod,
                            ContributionType = contributionType
                        })
                    .Join(
                        ctx.PayrollMetrics,
                        payroll => payroll.Payroll.PayrollMetricKey,
                        payrollMetrics => payrollMetrics.PayrollMetricKey,
                        (payroll, payrollMetric) => new {
                            Payroll = payroll.Payroll,
                            Date = payroll.Date,
                            Employee = payroll.Employee,
                            DeliveryMethod = payroll.DeliveryMethod,
                            ContributionType = payroll.ContributionType,
                            PayrollMetric = payrollMetric
                        })
                    .Where(
                        p => p.Payroll.DocumentId == documentId
                    )
                    .Select(
                        p => new PaycheckDetails {
                            CchId = p.Employee.Cchid,
                            FirstName = p.Employee.EmployeeFirstName,
                            LastName = p.Employee.EmployeeLastName,
                            PrimaryWorkLocationCode = p.Employee.PrimaryWorkLocationCode,
                            FederalTaxElectionCode = p.Employee.FederalTaxElectionCode,
                            StateOfWorkElectionCode = p.Employee.StateOfWorkElectionCode,
                            StateOfResidenceElectionCode = p.Employee.StateOfResidenceElectionCode,
                            PayDate = p.Date.FullDate,
                            DocumentId = p.Payroll.DocumentId,
                            DeliveryMethodCode = p.DeliveryMethod.DeliveryMethodCode,
                            PayrollCategoryName = p.PayrollMetric.PayrollCategoryName,
                            PayrollMetricName = p.PayrollMetric.PayrollMetricName,
                            PreTaxInd = p.PayrollMetric.PreTaxInd,
                            ContributionTypeCode = p.ContributionType.ContributionTypeCode,
                            PayrollMetricRate = p.Payroll.PayrollMetricRate,
                            PerPeriodQty = p.Payroll.PerPeriodQty,
                            PerPeriodAmt = p.Payroll.PerPeriodAmt,
                            YearToDateQuantity = p.Payroll.YtdQty,
                            YearToDateAmount = p.Payroll.YtdAmt
                        }
                    )
                    .ToListAsync();
            }

            return payCheckDetails;
        }
    }
}