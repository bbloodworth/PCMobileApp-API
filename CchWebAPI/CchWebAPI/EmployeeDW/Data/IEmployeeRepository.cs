using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CchWebAPI.EmployeeDW.Data {
    public interface IEmployeeRepository {
        void Initialize(string connectionString);
        Task<Employee> GetEmployeeByKeyAsync(int employeeKey);
        Task<Employee> GetEmployeeByCchIdAsync(int cchId);
    }

    public class EmployeeRepository : IEmployeeRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<Employee> GetEmployeeByKeyAsync(int employeeKey) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeeContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.EmployeeKey == employeeKey);
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeeContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CCHID == cchId && p.CurrentRecordInd.Value);
            }

            return employee;
        }
    }
}