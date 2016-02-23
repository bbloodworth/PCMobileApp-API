//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BizContents
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContentTranslation
    {
        public int ContentID { get; set; }
        public int LocaleID { get; set; }
        public string ContentTitle { get; set; }
        public string ContentCaptionText { get; set; }
        [AllowHtml]
        public string ContentDesc { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Content Content { get; set; }
        public virtual Locale Locale { get; set; }
    }
}