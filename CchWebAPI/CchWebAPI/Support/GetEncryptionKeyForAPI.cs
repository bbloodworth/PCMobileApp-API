using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Support
{
    [System.ComponentModel.DesignerCategory("")]
    public class GetEncryptionKeyForAPI : DataBase
    {
        public GetEncryptionKeyForAPI(String APIKey)
            : base("GetEncryptionKeyForAPI")
        {
            this.Parameters.New("APIKey", System.Data.SqlDbType.NVarChar,36,APIKey);
            this.GetFrontEndData();
        }
    }
}