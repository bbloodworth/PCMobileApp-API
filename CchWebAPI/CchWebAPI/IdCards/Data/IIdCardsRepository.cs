using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.IdCards.Data {
    public interface IIdCardsRepository {
        void Initialize(string connectionString);
       Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId);
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

        public async Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            using(var itx = new IdCardsContext(_connectionString)) {
                //This is not the final production code. It's illustrative at this point pending 
                //the final data structures.

                var dependentsQuery = @"SELECT CCHID, RelationshipCode
                    FROM Enrollments
                    WHERE SubscriberMedicalID IN
                    (SELECT SubscriberMedicalID 
	                    FROM Enrollments 
	                    WHERE CCHID = @cchid)";

                var parm = new SqlParameter("@cchid", cchId);
                //Load records for any dependents
                var enrollments = await itx.Database.SqlQuery<Enrollment>(dependentsQuery, parm).ToListAsync();

                var employee = enrollments.FirstOrDefault(e => e.RelationshipCode.Equals("20") 
                    || string.IsNullOrEmpty(e.RelationshipCode));

                if (employee == null)
                    throw new InvalidOperationException("No member found");

                List<IdCard> results;
                //if dependent only get dependent cards
                if(!employee.CchId.Equals(cchId))
                    results = await itx.IdCards
                        .Include(p => p.CardType)
                        .Where(id => id.CchId.Equals(cchId)).ToListAsync();
                else //get employee and dependents
                    results = await itx.IdCards
                        .Include(p => p.CardType)
                        .Where(id => 
                            enrollments.Select(e => e.CchId).ToList()
                                .Contains(id.CchId)).ToListAsync();

                results.ForEach(r => {
                    r.MemberId = cchId;
                });

                return results;
            }
        }
        
    }
}