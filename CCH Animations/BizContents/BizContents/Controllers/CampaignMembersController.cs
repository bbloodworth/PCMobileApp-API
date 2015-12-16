using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BizContents.Controllers
{
    public class CampaignMembersController : Controller
    {
        private CCH_CustomerDemoNCCTEntities db = new CCH_CustomerDemoNCCTEntities();

        // GET: CampaignMembers
        public ActionResult Index()
        {
            var campaignMembers = db.CampaignMembers.Include(c => c.Campaign);
            return View(campaignMembers.ToList().OrderByDescending(c => c.CampaignID));
        }

        // GET: CampaignMembers/Details/5
        public ActionResult Details(int? campaignId, int? cchId)
        {
            if (campaignId == null || cchId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMember campaignMember =
                db.CampaignMembers.FirstOrDefault(c => c.CampaignID == campaignId && c.CCHID == cchId);

            if (campaignMember == null)
            {
                return HttpNotFound();
            }
            return View(campaignMember);
        }

        // GET: CampaignMembers/Create
        public ActionResult Create()
        {
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc");
            return View();
        }

        // POST: CampaignMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CampaignID,CCHID,Savings,Score,CreateDate,YourCostSavingsAmt")] CampaignMember campaignMember)
        {
            if (ModelState.IsValid)
            {
                campaignMember.CreateDate = DateTime.Now;

                db.CampaignMembers.Add(campaignMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignMember.CampaignID);
            return View(campaignMember);
        }

        // GET: CampaignMembers/Edit/5
        public ActionResult Edit(int? campaignId, int? cchId)
        {
            if (campaignId == null || cchId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMember campaignMember =
                db.CampaignMembers.FirstOrDefault(c => c.CampaignID == campaignId && c.CCHID == cchId);

            if (campaignMember == null)
            {
                return HttpNotFound();
            }
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignMember.CampaignID);
            return View(campaignMember);
        }

        // POST: CampaignMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CampaignID,CCHID,CreateDate,Savings,Score,YourCostSavingsAmt")] CampaignMember campaignMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaignMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignMember.CampaignID);
            return View(campaignMember);
        }

        // GET: CampaignMembers/Delete/5
        public ActionResult Delete(int? campaignId, int? cchId)
        {
            if (campaignId == null || cchId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignMember campaignMember =
                db.CampaignMembers.FirstOrDefault(c => c.CampaignID == campaignId && c.CCHID == cchId);

            if (campaignMember == null)
            {
                return HttpNotFound();
            }
            return View(campaignMember);
        }

        // POST: CampaignMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? campaignId, int? cchId)
        {
            CampaignMember campaignMember =
                db.CampaignMembers.FirstOrDefault(c => c.CampaignID == campaignId && c.CCHID == cchId);

            db.CampaignMembers.Remove(campaignMember);
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
