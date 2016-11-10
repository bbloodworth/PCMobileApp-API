﻿using CchWebAPI.PaidTimeOff.Models;
using CchWebAPI.PaidTimeOff.Data;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CchWebAPI.PaidTimeOff.Dispatchers {
    public interface IPaidTimeOffDispatcher {
        Task<List<PaidTimeOffTable>> GetPaidTimeOffTable();
    }

    public class PaidTimeOffDispatcher : IPaidTimeOffDispatcher {
        private IPaidTimeOffRepository _repository;

        public PaidTimeOffDispatcher(IPaidTimeOffRepository repository) {
            _repository = repository;
        }

        public async Task<List<PaidTimeOffTable>> GetPaidTimeOffTable() {
            var result = await _repository.GetPaidTimeOffDetailsByCchId(1);
            
            return result;
        }

    }
}