using System;
using System.Data.Entity.ModelConfiguration;
using Newtonsoft.Json;

//MUST REMAIN BACKWARD COMPATIBLE
namespace CchWebAPI.IdCard.Models {
    public class CardDetail {
        public bool Expired { get; set; }
        public bool Invalid { get; set; }
        public string CardTypeFileName { get; set; }
        public int CardTypeId { get; set; }
        public int CardViewModeId { get; set; }
        public string MemberName { get; set; }
        public string MemberNameValue { get; set; }
        public string MemberMedicalId { get; set; }
        public string MemberMedicalIdValue { get; set; }
        public string MemberId { get; set; }
        public string MemberIdValue { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameValue { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeIdValue { get; set; }
        public string EffectiveDate { get; set; }
        public string EffectiveDateValue { get; set; }
        public string InNetworkCoinsurance { get; set; }
        public string InNetworkCoinsuranceValue { get; set; }
        public string OutNetworkCoinsurance { get; set; }
        public string OutNetworkCoinsuranceValue { get; set; }
        public string NetworkDesignationValue { get; set; }
        public string GroupDesignation { get; set; }
        public string GroupDesignationValue { get; set; }
        public string GroupName { get; set; }
        public string GroupNameValue { get; set; }
        public string GroupNumber { get; set; }
        public string GroupNumberValue { get; set; }
        public string GroupId { get; set; }
        public string GroupIdValue { get; set; }
        public string RxBin { get; set; }
        public string RxBinValue { get; set; }
        public string RxPcn { get; set; }
        public string RxPcnValue { get; set; }
        public string RxGrp { get; set; }
        public string RxGrpValue { get; set; }
        public string RxId { get; set; }
        public string RxIdValue { get; set; }
        public string PlanName { get; set; }
        public string PlanNameValue { get; set; }
        public string OnPlan { get; set; }
        public string OnPlanValue { get; set; }
        public string PlanType { get; set; }
        public string PlanTypeValue { get; set; }
        public string CardIssuedDateValue { get; set; }
        public string CoverageType { get; set; }
        public string CoverageTypeValue { get; set; }
        public string PayorIdValue { get; set; }
        public string BenefitTypeCode { get; set; }
        public string PayerName { get; set; }
        public int EnrolledCCHID { get; set; }
        public int SubscribedCCHID { get; set; }
        public string IndividualDeductibleAmt { get; set; }
        public string FamilyDeductibleAmt { get; set; }
        public string ExamCopayAmt { get; set; }
        public string MaterialsCopayAmt { get; set; }
        public string GenderCode { get; set; }
        public string BirthDate { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberMiddleName { get; set; }
        public string MemberLastName { get; set; }
        public string DateOfBirthMM { get; set; }
        public string DateOfBirthYY { get; set; }
    }
}