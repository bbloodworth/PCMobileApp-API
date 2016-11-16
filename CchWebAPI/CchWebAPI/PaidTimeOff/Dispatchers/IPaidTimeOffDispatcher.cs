using CchWebAPI.PaidTimeOff.Models;
using CchWebAPI.PaidTimeOff.Data;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CchWebAPI.PaidTimeOff.Dispatchers {
    public interface IPaidTimeOffDispatcher {
        Task<List<PaidTimeOffDetail>> GetPaidTimeOffTable(Employer employer, int cchid);
    }

    public class PaidTimeOffDispatcher : IPaidTimeOffDispatcher {
        private IPaidTimeOffRepository _repository;

        public PaidTimeOffDispatcher(IPaidTimeOffRepository repository) {
            _repository = repository;
        }

        public async Task<List<PaidTimeOffDetail>> GetPaidTimeOffTable(Employer employer, int cchid) {
            string employerConnectionString =
                DataWarehouse.GetEmployerConnectionString(employer.Id).Equals(string.Empty) ?
                employer.ConnectionString :
                DataWarehouse.GetEmployerConnectionString(employer.Id);

            _repository.Initialize(employerConnectionString);
            
            var result = await _repository.GetPaidTimeOffDetailsByCchIdAsync(cchid);
            
            return result;
        }

    }
}