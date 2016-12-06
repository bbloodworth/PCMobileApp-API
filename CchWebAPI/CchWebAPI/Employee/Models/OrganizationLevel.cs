using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Employee.Models {
    public class OrganizationLevel {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeName { get; set; }
        public OrganizationLevel() { }
        public OrganizationLevel(string name, string code, string codeName) {
            Name = name;
            Code = code;
            CodeName = codeName;
        }
    }
}