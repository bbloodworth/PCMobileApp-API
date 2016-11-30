using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Employee.Models {
    public class Job {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public Job() { }
        public Job(string code, string name, string title) {
            Code = code;
            Name = name;
            Title = title;
        }
    }
}