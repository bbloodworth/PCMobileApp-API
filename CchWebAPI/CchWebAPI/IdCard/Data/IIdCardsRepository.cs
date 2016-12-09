using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ClearCost.Platform;

using CchWebAPI.IdCard.Models;

namespace CchWebAPI.IdCard.Data {
    public interface IIdCardsRepository {
        void Initialize(string connectionString);
        Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId);
        Task<List<IdCard>> GetIdCardsByCchIdAsyncV2(int cchId, Employer employer);
    }

    public class IdCardsRepository : IIdCardsRepository {
        private string _connectionString = string.Empty;

        private class Enrollment {
            public int CchId { get; set; }
            public string RelationshipCode { get; set; }
        }

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<IdCard>> GetIdCardsByCchIdAsyncV2(int cchId, Employer employer) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            using (var ctx = new IdCardsContext(_connectionString)) {                
                List<CardDetail> cardDetails = await ctx.Database.SqlQuery<CardDetail>("SELECT * FROM vwIdCard WHERE SubscribedCCHID = @Id", new SqlParameter("@Id", cchId)).ToListAsync();

                List<IdCard> results = new List<IdCard>();

                List<CardViewMode> viewModes = await ctx.CardViewModes.ToListAsync();

                // for each view mode create an id card object 
                // todo dynamically build file name
                foreach(CardDetail cardDetail in cardDetails) {
                    foreach(CardViewMode viewMode in viewModes) {
                        cardDetail.CardTypeId = 1;
                        cardDetail.CardViewModeId = viewMode.CardViewModeId;

                        int cardTypeId = 1;

                        switch (cardDetail.BenefitTypeCode) {
                            case "MED":
                                cardTypeId = 1;
                                break;
                            case "RX":
                                cardTypeId = 2;
                                break;
                            case "DEN":
                                cardTypeId = 3;
                                break;
                            case "VIS":
                                cardTypeId = 4;
                                break;

                        }


                        IdCard card = new IdCard {
                            LocaleId = 1,
                            TypeId = cardTypeId,
                            MemberId = cchId,
                            DetailText = JsonConvert.SerializeObject(cardDetail),
                            ViewModeId = viewMode.CardViewModeId,
                        };

                        IdCardType cardType = new IdCardType {
                            FileName = (string.Format("{0}_{1}_{2}", cardDetail.BenefitTypeCode, cardDetail.PayerName, employer.Name).Replace(" ", String.Empty)),
                            Id = 1,
                            Translation = cardDetail.PayerName
                        };

                        card.CardType = cardType;
                        results.Add(card);

                    }
                }


                return results;
            }
        }

         public async Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            using(var itx = new IdCardsContext(_connectionString)) {
                //This is not the final production code. It's illustrative at this point pending 
                //the final data structures.

                var enrollmentsQuery = @"SELECT CCHID, RelationshipCode
                    FROM Enrollments
                    WHERE SubscriberMedicalID IN
                    (SELECT SubscriberMedicalID 
	                    FROM Enrollments 
	                    WHERE CCHID = @cchid)";

                var parm = new SqlParameter("@cchid", cchId);
                //Load records for any dependents
                var familyEnrollments = await itx.Database.SqlQuery<Enrollment>(enrollmentsQuery, parm).ToListAsync();
                var familyEnrollmentIds = familyEnrollments.Select(e => e.CchId);

                var employee = familyEnrollments.FirstOrDefault(e => e.RelationshipCode.Equals("20") 
                    || string.IsNullOrEmpty(e.RelationshipCode));

                if (employee == null)
                    throw new InvalidOperationException("No member found");

                List<IdCard> results;
                //if dependent only get dependent cards
                if(!employee.CchId.Equals(cchId))
                    results = await itx.IdCards
                        .Include(p => p.CardType)
                        .Where(id => id.MemberId.Equals(cchId)
                            && id.LocaleId.Equals(1) //Only support English at this time
                            && !string.IsNullOrEmpty(id.DetailText)).ToListAsync();
                else //get employee and dependents
                    results = await itx.IdCards
                        .Include(p => p.CardType)
                        .Where(id =>id.LocaleId.Equals(1)
                            && !string.IsNullOrEmpty(id.DetailText)
                            && familyEnrollmentIds.Contains(id.MemberId)).ToListAsync();

                var cardTypeIds = results.Select(r => r.CardType.Id).Distinct();

                var translations = await itx.IdCardTypeTranslations
                    .Where(t => t.LocaleId.Equals(1) && cardTypeIds.Contains(t.Id)).ToListAsync();

                results.ForEach(r => {
                    r.CardType.Translation = translations.FirstOrDefault(t => t.Id.Equals(r.CardType.Id)).CardTypeName;
                    r.RequestContextMemberId = cchId;
                });

                return results;
            }
        }
        
    }
}
 