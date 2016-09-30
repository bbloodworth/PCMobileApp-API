using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Core.Entities {
    public class MemberCard {
        #region Properties
        public int CchId { get; set; }
        public int CardTypeId { get; set; }
        public int LocaleId { get; set; }
        public int CardViewModeId { get; set; }
        public string CardMemberDataText { get; set; }
        public virtual ICollection<CchUser> VisibleByUsers { get; set; }
        #endregion

    }
}
