using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Core.Mappings {
    public static class AutoMapperExtensions {
        public static void RegisterConfigurations(this Type[] types) {
            var automapperProfiles = types
                .Where(p => p.IsSubclassOf(typeof(Profile)))
                .Select(Activator.CreateInstance)
                .OfType<Profile>()
                .ToList();

            automapperProfiles.ForEach(Mapper.Configuration.AddProfile);
        }

        public static void RegisterConfigurations() {
            Mapper.Initialize(cfg => {
                cfg.AddProfile<MemberCardProfile>();
            });
        }
    }
}
