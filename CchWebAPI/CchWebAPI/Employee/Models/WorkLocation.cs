using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Employee.Models {
    public class WorkLocation {
        public string Code { get; set; }
        public string Name { get; set; }

        public WorkLocation() { }
        public WorkLocation(string code, string name) {
            Code = code;
            Name = name;
        }
    }
}