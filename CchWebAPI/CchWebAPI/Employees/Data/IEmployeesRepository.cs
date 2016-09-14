using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CchWebAPI.Employees.Data {
    public interface IEmployeesRepository {
        void Initialize(string connectionString);
        Task<Employee> GetEmployeeAsync(int cchId);
    }

    public class EmployeesRepository : IEmployeesRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<Employee> GetEmployeeAsync(int cchId) {
            Contract.Requires<InvalidOperationException>(!string.IsNullOrEmpty(_connectionString), "Failed to initialize repository.");

            Employee employee = null;

            using (var ctx = new EmployeesContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.MemberId == cchId);
            }

            return employee;
        }
    }
}