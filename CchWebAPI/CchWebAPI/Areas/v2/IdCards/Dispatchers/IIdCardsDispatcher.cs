using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearCost.Data.Security;
using CchWebAPI.Areas.v2.IdCards.Data;
using CchWebAPI.Areas.Animation.Models;
using ClearCost.Security.JWT;
using CchWebAPI.Support;
using Newtonsoft.Json;

namespace CchWebAPI.Areas.v2.IdCards.Dispatchers {
    public interface IIdCardsDispatcher {
        Task<List<IdCard>> ExecuteAsync(int cchId, Employer employer);
    }

    public class IdCardsDispatcher : IIdCardsDispatcher {
        private IIdCardsRepository _repository;
        public IdCardsDispatcher(IIdCardsRepository repository) {
            _repository = repository;
        }

        public async Task<List<IdCard>> ExecuteAsync(int cchId, Employer employer) {
            if (cchId < 1)
                throw new InvalidOperationException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetIdCardsByCchIdAsync(cchId);

            var cardBaseAddress = "CardBaseAddress".GetConfigurationValue();
            var timeout = "TimeoutInMinutes".GetConfigurationValue();

            result.ForEach(r => {

                //This whole section may have to be rethought depending on new data structures. 
                //May require an interface break/ v2 in media as well.
                var cardToken = new CardToken() {
                    EmployerId = employer.Id,
                    CardDetail = JsonConvert.DeserializeObject<CardDetail>(r.Detail),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt16(timeout))
                };

                cardToken.CardDetail.CardTypeFileName = r.CardType.FileName;
                cardToken.CardDetail.CardTypeId = r.TypeId;
                cardToken.CardDetail.CardViewModeId = r.ViewModeId;

                var jwt = JwtService.EncryptPayload(JsonConvert.SerializeObject(cardToken));

                r.Url = string.Format("{0}/?tkn={1}|{2}", cardBaseAddress, employer.Id, jwt);
            });

            return result;
        }
    }
}
