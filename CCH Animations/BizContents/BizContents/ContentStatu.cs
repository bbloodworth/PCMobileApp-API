//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BizContents
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContentStatu
    {
        public ContentStatu()
        {
            this.UserContents = new HashSet<UserContent>();
            this.ContentTypeStates = new HashSet<ContentTypeState>();
        }
    
        public int ContentStatusID { get; set; }
        public string ContentStatusDesc { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual ICollection<UserContent> UserContents { get; set; }
        public virtual ICollection<ContentTypeState> ContentTypeStates { get; set; }
    }
}