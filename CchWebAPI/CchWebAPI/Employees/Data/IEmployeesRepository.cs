using System;
using System.Data.Entity;
using System.Threading.Tasks;

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
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeesContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.MemberId == cchId);
            }

            return employee;
        }
    }
}