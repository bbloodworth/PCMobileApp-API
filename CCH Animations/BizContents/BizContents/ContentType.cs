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
    
    public partial class ContentType
    {
        public ContentType()
        {
            this.Contents = new HashSet<Content>();
            this.ContentTypeStates = new HashSet<ContentTypeState>();
        }
    
        public int ContentTypeID { get; set; }
        public string ContentTypeDesc { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual ICollection<Content> Contents { get; set; }
        public virtual ICollection<ContentTypeState> ContentTypeStates { get; set; }
    }
}
