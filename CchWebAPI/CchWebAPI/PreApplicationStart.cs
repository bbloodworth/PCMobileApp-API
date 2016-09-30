using CchWebAPI.PComm.Models;
using ClearCost.Data;
using Dapper;

namespace CchWebAPI {
    public class PreApplicationStart {
        public static void Start() {
            SqlMapper.SetTypeMap(typeof(HealthPlanSummary), new ColumnAttributeTypeMapper<HealthPlanSummary>());
        }
    }
}
