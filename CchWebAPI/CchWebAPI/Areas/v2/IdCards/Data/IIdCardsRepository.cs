using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using ClearCost.Data;

namespace CchWebAPI.Areas.v2.IdCards.Data {
    public interface IIdCardsRepository {
        void Initialize(string connectionString);
       Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId);
    }

    public class IdCardsRepository : IIdCardsRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            using(var itx = new IdCardsContext(_connectionString)) {
                var mockResults = new List<IdCard>();
                // Mock data until actual data structures are finalized.  IdCard interface likely to 
                //change then too.
                mockResults.AddRange(GenerateMockCards());

                return mockResults;

                //This is not the final production code. It's illustrative at this point pending 
                //the final data structures.
                var results = await itx.IdCards.Where(id => id.CchId.Equals(cchId) 
                    || id.MemberId.Equals(cchId)).ToListAsync();

                return results;
            }
        }

        private List<IdCard> GenerateMockCards() {
            return new List<IdCard>() {
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 1,
                    ViewModeId = 1,
                    FileName = "Med_Cigna_Caesars",
                    Detail = @"{'EffectiveDate': 'Coverage Effective Date','EffectiveDateValue': '01-01-2015','MemberName': 'Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 1,
                    ViewModeId = 2,
                    FileName = "Med_Cigna_Caesars",
                    Detail = @"{'EffectiveDate': 'Coverage Effective Date','EffectiveDateValue': '01-01-2015','MemberName': 'Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 1,
                    ViewModeId = 3,
                    FileName = "Med_Cigna_Caesars",
                    Detail = @"{'EffectiveDate': 'Coverage Effective Date','EffectiveDateValue': '01-01-2015','MemberName': 'Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 1,
                    ViewModeId = 4,
                    FileName = "Med_Cigna_Caesars",
                    Detail = @"{'EffectiveDate': 'Coverage Effective Date','EffectiveDateValue': '01-01-2015','MemberName': 'Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 2,
                    ViewModeId = 1,
                    FileName = "RX_Generic_Caesars",
                    Detail = @"{'RxBin': 'RxBin','RxBinValue': '003858','RxPCN': 'RxPCN','RxPCNValue': 'A4','RxGrp': 'RxGrp','RxGrpValue': '123456' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 2,
                    ViewModeId = 2,
                    FileName = "RX_Generic_Caesars",
                    Detail = @"{'RxBin': 'RxBin','RxBinValue': '003858','RxPCN': 'RxPCN','RxPCNValue': 'A4','RxGrp': 'RxGrp','RxGrpValue': '123456' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 2,
                    ViewModeId = 3,
                    FileName = "RX_Generic_Caesars",
                    Detail = @"{'RxBin': 'RxBin','RxBinValue': '003858','RxPCN': 'RxPCN','RxPCNValue': 'A4','RxGrp': 'RxGrp','RxGrpValue': '123456' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 2,
                    ViewModeId = 4,
                    FileName = "RX_Generic_Caesars",
                    Detail = @"{'RxBin': 'RxBin','RxBinValue': '003858','RxPCN': 'RxPCN','RxPCNValue': 'A4','RxGrp': 'RxGrp','RxGrpValue': '123456' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 3,
                    ViewModeId = 1,
                    FileName = "Vision_EyeMed_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'Select Plan','MemberName': 'Member Name','MemberNameValue': 'Jim Jones'}"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 3,
                    ViewModeId = 2,
                    FileName = "Vision_EyeMed_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'Select Plan','MemberName': 'Member Name','MemberNameValue': 'Jim Jones'}"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 3,
                    ViewModeId = 3,
                    FileName = "Vision_EyeMed_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'Select Plan','MemberName': 'Member Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 3,
                    ViewModeId = 4,
                    FileName = "Vision_EyeMed_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'Select Plan','MemberName': 'Member Name','MemberNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 4,
                    ViewModeId = 1,
                    FileName = "Dental_Generic_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'PDP Network','EmployeeName': 'Employee Name','EmployeeNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 4,
                    ViewModeId = 2,
                    FileName = "Dental_Generic_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'PDP Network','EmployeeName': 'Employee Name','EmployeeNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 4,
                    ViewModeId = 3,
                    FileName = "Dental_Generic_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'PDP Network','EmployeeName': 'Employee Name','EmployeeNameValue': 'Jim Jones' }"
                },
                new IdCard() { CchId = 63880,
                    MemberId = 63880,
                    TypeId = 4,
                    ViewModeId = 4,
                    FileName = "Dental_Generic_Caesars",
                    Detail = @"{'PlanName': 'Plan Name','PlanNameValue': 'PDP Network','EmployeeName': 'Employee Name','EmployeeNameValue': 'Jim Jones' }"
                }
            };
        }
    }


    internal class IdCardsContext : ClearCostContext<IdCardsContext> {
        public IdCardsContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new IdCard.IdCardConfiguration());
        }

        public DbSet<IdCard> IdCards { get; set; }
    }
}