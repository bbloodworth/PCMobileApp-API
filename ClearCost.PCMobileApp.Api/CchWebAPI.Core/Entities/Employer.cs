﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Core.Entities {
    public class Employer {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
