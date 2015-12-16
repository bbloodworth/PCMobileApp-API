using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Models
{
    public class Handshake
    {
        public String AuthHash;
        public EmployeeInfoData EmployeeInfo;
        public struct EmployeeInfoData
        {
            public String LastName;
            public String FirstName;
            public String Address1;
            public String Address2;
            public String City;
            public String State;
            public String ZipCode;
            public Double Latitude;
            public Double Longitude;
            public String Insurer;
            public String RXProvider;
            public String HealthPlanType;
            public String MedicalPlanType;
            public String RXPlanType;
            public String PropertyCode;
        }
    }

    public class HandshakeMobile
    {
        public String AuthHash;
        public EmployeeInfoData EmployeeInfo;
        public struct EmployeeInfoData
        {
            public String LastName;
            public String FirstName;
            public String Address1;
            public String Address2;
            public String City;
            public String State;
            public String ZipCode;
            public Double Latitude;
            public Double Longitude;
            public String Insurer;
            public String RXProvider;
            public String HealthPlanType;
            public String MedicalPlanType;
            public String RXPlanType;
        }
        public String EmployerName;
    }
}