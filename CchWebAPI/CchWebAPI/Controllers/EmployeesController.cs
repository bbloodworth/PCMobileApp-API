using CchWebAPI.Employees.Dispatchers;
using CchWebAPI.EmployeeDW.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    //[RoutePrefix("v2")]
    public class EmployeesController : ApiController {
        IEmployeesDispatcher _dispatcher;
        IEmployeeDispatcher _dispatcherDW;
        public EmployeesController(IEmployeesDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public EmployeesController(IEmployeeDispatcher dispatcher) {
            _dispatcherDW = dispatcher;
        }
        public EmployeesController() { }

        private void InitDispatcher() {
            _dispatcher = new EmployeesDispatcher(new Employees.Data.EmployeesRepository());
        }
        private void InitDispatcherDW() {
            _dispatcherDW = new EmployeeDispatcher(new EmployeeDW.Data.EmployeeRepository());
        }

        [HttpGet]
        //[Route("employees/{employerId}/{cchId}")]
        public async Task<ApiResult<Employee>> GetEmployee(int employerId, int cchId) {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            Employee employee = new Employee();

            // Hack: Until we move over to the data warehouse completely, this hack exists to 
            // negotiate between the two repositories.
            if (string.IsNullOrWhiteSpace(DataWarehouse.GetEmployerConnectionString(employerId))) {
                if (_dispatcher == null) {
                    InitDispatcher();
                }

                var result = await _dispatcher.ExecuteAsync(cchId, employer);
                employee.MapProperties(result);
            }
            else {
                if (_dispatcherDW == null) {
                    InitDispatcherDW();
                }

                var employeeResult = await _dispatcherDW.ExecuteAsync(cchId, employer);
                employee.MapProperties(employeeResult);
            }

            return ApiResult<Employee>.ValidResult(employee, string.Empty);
        }

    }

    public class Employee {
        #region properties
        public int CchId { get; set; }
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
            OrganizationLevels = new List<OrganizationLevel>();
            PrimaryWorkLocation = new WorkLocation();
            Job = new Job();
        }
        public Employee(Employees.Data.Employee employee) {
            OrganizationLevels = new List<OrganizationLevel>();
            PrimaryWorkLocation = new WorkLocation();
            Job = new Job();

            if (employee != null) {
                MapProperties(employee);
            }
        }
        public Employee(EmployeeDW.Data.Employee employee) {
            OrganizationLevels = new List<OrganizationLevel>();
            PrimaryWorkLocation = new WorkLocation();
            Job = new Job();

            if (employee != null) {
                MapProperties(employee);
            }
        }
        #endregion
        #region methods
        public void MapProperties(Employees.Data.Employee employee) {
            if (employee == null)
                return;

            CchId = employee.MemberId;
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
        public void MapProperties(EmployeeDW.Data.Employee employee) {
            if (employee == null)
                return;

            CchId = employee.CCHID;
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
        #endregion
    }
    public class OrganizationLevel {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeName { get; set; }
        public OrganizationLevel() { }
        public OrganizationLevel(string name, string code, string codeName) {
            Name = name;
            Code = code;
            CodeName = codeName;
        }
    }
    public class WorkLocation {
        public string Code { get; set; }
        public string Name { get; set; }

        public WorkLocation() { }
        public WorkLocation(string code, string name) {
            Code = code;
            Name = name;
        }
    }

    public class Job {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public Job() { }
        public Job(string code, string name, string title) {
            Code = code;
            Name = name;
            Title = title;
        }
    }
}