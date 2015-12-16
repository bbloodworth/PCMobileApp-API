using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BizContents.Controllers
{
    public class CampaignContentsController : Controller
    {
        private CCH_CustomerDemoNCCTEntities db = new CCH_CustomerDemoNCCTEntities();

        // GET: CampaignContents
        public ActionResult Index()
        {
            var campaignContents = db.CampaignContents
                .Include(c => c.Campaign)
                .Include(c => c.Content)
                .Include(c => c.Content.ContentTranslations);
            return View(campaignContents.ToList().OrderByDescending(c => c.CampaignID));
        }

        // GET: CampaignContents/Details/5
        public ActionResult Details(int? campaignId, int? contentId)
        {
            if (campaignId == null || contentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignContent campaignContent =
                db.CampaignContents.FirstOrDefault(c => c.CampaignID == campaignId && c.ContentID == contentId);

            if (campaignContent == null)
            {
                return HttpNotFound();
            }
            return View(campaignContent);
        }

        // GET: CampaignContents/Create
        public ActionResult Create()
        {
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc");
            ViewBag.ContentID = new SelectList(db.ContentTranslations, "ContentID", "ContentTitle");
            return View();
        }

        // POST: CampaignContents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CampaignID,ContentID,CreateDate,ActivationDate,ExpirationDate,UserContentInd")] CampaignContent campaignContent)
        {
            campaignContent.CreateDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.CampaignContents.Add(campaignContent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignContent.CampaignID);
            ViewBag.ContentID = new SelectList(db.ContentTranslations, "ContentID", "ContentTitle", campaignContent.ContentID);
            return View(campaignContent);
        }

        // GET: CampaignContents/Edit/5
        public ActionResult Edit(int? campaignId, int? contentId)
        {
            if (campaignId == null || contentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignContent campaignContent =
                db.CampaignContents.FirstOrDefault(c => c.CampaignID == campaignId && c.ContentID == contentId);

            if (campaignContent == null)
            {
                return HttpNotFound();
            }
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignContent.CampaignID);
            ViewBag.ContentID = new SelectList(db.ContentTranslations, "ContentID", "ContentTitle", campaignContent.ContentID);
            return View(campaignContent);
        }

        // POST: CampaignContents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CampaignID,ContentID,CreateDate,ActivationDate,ExpirationDate,UserContentInd")] CampaignContent campaignContent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaignContent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CampaignID = new SelectList(db.Campaigns, "CampaignID", "CampaignDesc", campaignContent.CampaignID);
            ViewBag.ContentID = new SelectList(db.ContentTranslations, "ContentID", "ContentTitle", campaignContent.ContentID);
            return View(campaignContent);
        }

        // GET: CampaignContents/Delete/5
        public ActionResult Delete(int? campaignId, int? contentId)
        {
            if (campaignId == null || contentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignContent campaignContent =
                db.CampaignContents.FirstOrDefault(c => c.CampaignID == campaignId && c.ContentID == contentId);

            if (campaignContent == null)
            {
                return HttpNotFound();
            }
            return View(campaignContent);
        }

        // POST: CampaignContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? campaignId, int? contentId)
        {
            CampaignContent campaignContent =
                db.CampaignContents.FirstOrDefault(c => c.CampaignID == campaignId && c.ContentID == contentId);

            db.CampaignContents.Remove(campaignContent);
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
