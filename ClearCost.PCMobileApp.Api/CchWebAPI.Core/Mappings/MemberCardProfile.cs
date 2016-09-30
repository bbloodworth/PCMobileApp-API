using AutoMapper;
using CchWebAPI.Core.Entities;
using CchWebAPI.Core.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Core.Mappings {
    public class MemberCardProfile : Profile {
        protected override void Configure() {
            base.Configure();

            Mapper.CreateMap<Entities.MemberCard, DataTransferObjects.MemberCard>();
        }
    }
}
