using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BizContents.Controllers
{
    public class EnrollmentsController : Controller
    {
        private CCH_CustomerDemoNCCTEntities db = new CCH_CustomerDemoNCCTEntities();

        // GET: Enrollments
        public ActionResult Index()
        {
            return View(db.Enrollments.ToList());
        }

        // GET: Enrollments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CCHID,EmployeeID,MemberMedicalID,MemberRXID,SubscriberMedicalID,SubscriberRXID,LastName,FirstName,Middle,DateOfBirth,Gender,RelationshipCode,Address1,Address2,City,State,Zipcode,Email,Longitude,Latitude,UnRegisteredID,OptInIncentiveProgram,Insurer,RXProvider,HealthPlanType,MedicalPlanType,RXPlanType,Phone,UserID,OptInEmailAlerts,OptInTextMsgAlerts,MobilePhone,OptInPriceConcierge,LastUpdated,UpdateNotes,MemberSSN,SubscriberSSN,Referral_CCHID,MemberClassification,Subscriber_Email_Home,Subscriber_Email_Work,Member_Email_Home,Member_Email_Work,Subscriber_Phone_Home,Subscriber_Phone_Mobile,Member_Phone_Home,Member_Phone_Mobile,Member_Status,Interested_In_CCH,Contract_Prefix,Contract_Suffix,DateAdded,Subscriber_Coverage_Effective_Dt,Subscriber_Coverage_Termination_Dt,Notes,PropertyCode,PCP_Name,PCP_NPI,HearCCH,TandCIndicator,Source_Phone,Email_Phone,DateRegistered_Phone,AlternatePlanSource,MemberSSN_Full,PartialGeocode,RegistrationType")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CCHID,EmployeeID,MemberMedicalID,MemberRXID,SubscriberMedicalID,SubscriberRXID,LastName,FirstName,Middle,DateOfBirth,Gender,RelationshipCode,Address1,Address2,City,State,Zipcode,Email,Longitude,Latitude,UnRegisteredID,OptInIncentiveProgram,Insurer,RXProvider,HealthPlanType,MedicalPlanType,RXPlanType,Phone,UserID,OptInEmailAlerts,OptInTextMsgAlerts,MobilePhone,OptInPriceConcierge,LastUpdated,UpdateNotes,MemberSSN,SubscriberSSN,Referral_CCHID,MemberClassification,Subscriber_Email_Home,Subscriber_Email_Work,Member_Email_Home,Member_Email_Work,Subscriber_Phone_Home,Subscriber_Phone_Mobile,Member_Phone_Home,Member_Phone_Mobile,Member_Status,Interested_In_CCH,Contract_Prefix,Contract_Suffix,DateAdded,Subscriber_Coverage_Effective_Dt,Subscriber_Coverage_Termination_Dt,Notes,PropertyCode,PCP_Name,PCP_NPI,HearCCH,TandCIndicator,Source_Phone,Email_Phone,DateRegistered_Phone,AlternatePlanSource,MemberSSN_Full,PartialGeocode,RegistrationType")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
