using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Employee.Models {
    public class Employee {
        #region properties
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string PreferredFirstName { get; set; }
        public string LastName { get; set; }
        public Job Job { get; set; }
        public string RecognitionTitle { get; set; }
        public string EmployeeId { get; set; }
        public string MemberMedicalId { get; set; }
        public string MemberRxId { get; set; }
        public string SubscriberMedicalId { get; set; }
        public string SubscriberRxId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string RelationshipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string HealthPlanType { get; set; }
        public string MedicalPlanType { get; set; }
        public string RxPlanType { get; set; }
        public DateTime? SeniorityDate { get; set; }
        public DateTime? LastHireDate { get; set; }
        public string SupervisorFullName { get; set; }
        public string EmploymentTypeDescription { get; set; }
        public string EmploymentHoursDescription { get; set; }
        public List<OrganizationLevel> OrganizationLevels { get; set; }
        public WorkLocation PrimaryWorkLocation { get; set; }
        #endregion
        #region constructors
        public Employee() {
            Initialize();
        }
        public Employee(Data.V2.Employee employee) {
            Initialize();

            if (employee != null) {
                Merge(employee);
            }
        }
        public Employee(Data.V1.Employee employee) {
            Initialize();

            if (employee != null) {
                Merge(employee);
            }
        }
        #endregion
        #region methods
        private void Initialize() {
            OrganizationLevels = new List<OrganizationLevel>();
            PrimaryWorkLocation = new WorkLocation();
            Job = new Job();
        }
        public void Merge(Data.V1.Employee employee) {
            if (employee == null)
                return;

            MemberId = employee.CchId;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            EmployeeId = employee.EmployeeID;
            MemberMedicalId = employee.MemberMedicalID;
            MemberRxId = employee.MemberRXID;
            SubscriberMedicalId = employee.SubscriberMedicalID;
            SubscriberRxId = employee.SubscriberRXID;
            DateOfBirth = employee.DateOfBirth;
            Gender = employee.Gender;
            RelationshipCode = employee.RelationshipCode;
            City = employee.City;
            State = employee.State;
            ZipCode = employee.ZipCode;
            Email = employee.Email;
            Longitude = employee.Longitude;
            Latitude = employee.Latitude;
            HealthPlanType = employee.HealthPlanType;
            MedicalPlanType = employee.MedicalPlanType;
            RxPlanType = employee.RXPlanType;
        }
        public void Merge(Data.V2.Employee employee) {
            if (employee == null)
                return;

            MemberId = employee.CchId;
            FirstName = employee.EmployeeFirstName;
            PreferredFirstName = employee.EmployeePreferredFirstName;
            LastName = employee.EmployeeLastName;
            Job.Title = employee.JobTitleName;
            EmployeeId = employee.SourceEmployeeNum;
            RecognitionTitle = employee.RecognitionTitleName;

            OrganizationLevels.Add(new OrganizationLevel(employee.OrgLevel1Name,
                employee.OrgLevel1ValueCode, employee.OrgLevel1ValueName));
            OrganizationLevels.Add(new OrganizationLevel(employee.OrgLevel2Name,
                employee.OrgLevel2ValueCode, employee.OrgLevel2ValueName));
            OrganizationLevels.Add(new OrganizationLevel(employee.OrgLevel3Name,
                employee.OrgLevel3ValueCode, employee.OrgLevel3ValueName));
            OrganizationLevels.Add(new OrganizationLevel(employee.OrgLevel4Name,
                employee.OrgLevel4ValueCode, employee.OrgLevel4ValueName));
            OrganizationLevels.Add(new OrganizationLevel(employee.OrgLevel5Name,
                employee.OrgLevel5ValueCode, employee.OrgLevel5ValueName));
            PrimaryWorkLocation.Code = employee.PrimaryWorkLocationCode;
            PrimaryWorkLocation.Name = employee.PrimaryWorkLocationName;
            SeniorityDate = employee.SeniorityDate.Value;
            LastHireDate = employee.LastHireDate;
            Job.Code = employee.JobCode;
            Job.Name = employee.JobName;
            Job.Title = employee.JobTitleName;
            SupervisorFullName = employee.SupervisorFullName;
            EmploymentTypeDescription = employee.EmploymentTypeDesc;
            EmploymentHoursDescription = employee.EmploymentHoursDesc;
        }

        public void Merge(Data.V2.Member member)
        {
            if (member == null)
                return;

            Gender = member.GenderCode;
            City = member.CityName;
            State = member.StateCode;
            ZipCode = member.PostalCode;
        }

        public void Merge(BenefitMedicalPlan benefit)
        {
            if (benefit == null)
                return;
            MemberMedicalId = benefit.MemberPlanId.ToString();
            MedicalPlanType = benefit.PlanType;
            SubscriberMedicalId = benefit.SubscriberPlanId.ToString();

        }
        #endregion
    }
}
