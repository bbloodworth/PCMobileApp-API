using ClearCost.Data.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CchWebAPI.Services {
    public static class PlatformDataCache {

        private static List<Employer> _employers;
        public static List<Employer> Employers {
            get {
                if(_employers == null) {
                    LoadEmployers();
                }

                return _employers;
            }
        }

        //Can be used to expire the cache on demand
        public static void LoadEmployers() {
            using (var ptx = new PlatformContext()) {
                _employers = ptx.Employers.ToList();
            }
        }
    }
}
