//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace BizContents
{
    using System;
    using System.Collections.Generic;
    
    public partial class CampaignMember
    {
        public int CampaignID { get; set; }
        public int CCHID { get; set; }
        public Nullable<decimal> Savings { get; set; }
        public Nullable<double> Score { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<decimal> YourCostSavingsAmt { get; set; }
    
        public virtual Campaign Campaign { get; set; }
    }
}