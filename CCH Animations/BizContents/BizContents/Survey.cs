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
    
    public partial class Survey
    {
        public int SurveyID { get; set; }
        public Nullable<int> SurveyPassCount { get; set; }
        public Nullable<bool> EmbeddedSurveyInd { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Content Content { get; set; }
    }
}