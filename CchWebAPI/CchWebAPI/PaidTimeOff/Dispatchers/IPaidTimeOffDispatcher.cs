using CchWebAPI.PaidTimeOff.Models;
using CchWebAPI.PaidTimeOff.Data;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CchWebAPI.PaidTimeOff.Dispatchers {
    public interface IPaidTimeOffDispatcher {
        Task<List<PaidTimeOffDetail>> GetPaidTimeOffAsync(Employer employer, int cchid);
    }

    public class PaidTimeOffDispatcher : IPaidTimeOffDispatcher {
        private IPaidTimeOffRepository _repository;

        public PaidTimeOffDispatcher(IPaidTimeOffRepository repository) {
            _repository = repository;
        }

        public async Task<List<PaidTimeOffDetail>> GetPaidTimeOffAsync(Employer employer, int cchid) {
            _repository.Initialize(employer.ConnectionString);
            
            var result = await _repository.GetPaidTimeOffDetailsByCchIdAsync(cchid);
            
            return result;
        }

    }
}