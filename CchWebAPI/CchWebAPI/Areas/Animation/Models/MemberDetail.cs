using ClearCost.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Areas.Animation.Models
{
    // First resultset from stored proc
    public class MemberDetail
    {
        public MemberDetail()
        {
            Dependents = new List<DependentDetail>();
        }

        #region properties
        [Column(Name = "EmployeeID")]
        public string Id { get; set; }

        [Column(Name = "SubscriberMedicalID")]
        public string SubscriberMedicalId { get; set; }

        [Column(Name = "SubscriberRXID")]
        public string SubscriberRxId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        [Column(Name = "Zipcode")]
        public string ZipCode { get; set; }

        public string Email { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Column(Name = "Adult")]
        public bool IsAdult { get; set; }

        public char Gender { get; set; }

        public string Insurer { get; set; }

        [Column(Name = "RXProvider")]
        public string RxProvider { get; set; }

        public string Phone { get; set; }

        public string HealthPlanType { get; set; }

        public string MedicalPlanType { get; set; }

        [Column(Name = "RXPlanType")]
        public string RxPlanType { get; set; }

        [Column(Name = "Parent")]
        public bool IsParent { get; set; }

        public bool OptInIncentiveProgram { get; set; }

        public bool OptInEmailAlerts { get; set; }

        [Column(Name = "OptInTextMsgAlerts")]
        public bool OptInTextMessageAlerts { get; set; }

        public string MobilePhone { get; set; }

        public bool OptInPriceConcierge { get; set; }

        //This one is result set 3 from proc
        public decimal LostSavings { get; set; }

        //Result set 2 from Proc
        public List<DependentDetail> Dependents { get; set; }
        #endregion
    }

    // Second resultset from stored proc
    public class DependentDetail
    {
        [Column(Name = "EmployeeID")]
        public string MemberId { get; set; }

        [Column(Name = "SubscriberMedicalID")]
        public string SubscriberMedicalId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Column(Name = "Adult")]
        public bool IsAdult { get; set; }

        public int Age { get; set; }

        public int CCHID { get; set; }

        public bool ShowAccessQuestions { get; set; }

        public string RelationshipText { get; set; }

        [Column(Name = "DepToUserGranted")]
        public bool DependentToMemberPastCareAcessGranted { get; set; }

        [Column(Name = "UserToDepGranted")]
        public bool MemberToDependentPastCareAccessGranted { get; set; }
    }
}
