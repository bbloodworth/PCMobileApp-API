using System.Collections.Generic;
using ClearCost.Data;
using System.Data;
using Dapper;
using CchWebAPI.Areas.Animation.Models;
using System;

namespace CchWebAPI.Services
{
    public class EmployeeServices
    {
        public MemberDetail GetKeyEmployeeInfo(int employerId, int cchId)
        {
            //var keyEmployeeInfo = DataGateway.Query(
            //    "GetKeyEmployeeInfoByCchId", 
            //    new
            //    {
            //        CCHID = cchId
            //    },
            //    DataSource.Employer,
            //    employerId
            //);
            //return keyEmployeeInfo;

            MemberDetail keyEmployeeInfo = null;

            using (var connection = ConnectionFactory.Get(DataSource.Employer, employerId))
            {
                try
                {
                    connection.Open();
                    var multiple = connection.QueryMultiple(
                        "GetKeyEmployeeInfoByCchId",
                        new
                        {
                            CCHID = cchId
                        },
                        commandType: CommandType.StoredProcedure);

                    keyEmployeeInfo = multiple.Read<MemberDetail>().AsList().ToArray()[0];
                    keyEmployeeInfo.Dependents.AddRange(multiple.Read<DependentDetail>());
                }
                catch (Exception ex)
                {
                    DataGateway.LogError(ex);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return keyEmployeeInfo;
        }
    }
}
