using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BizContents.Controllers
{
    public class ContentTranslationsController : Controller
    {
        private CCH_CustomerDemoNCCTEntities db = new CCH_CustomerDemoNCCTEntities();

        // GET: ContentTranslations
        public ActionResult Index()
        {
            var contentTranslations = db.ContentTranslations
                .Include(c => c.Content)
                .Include(c => c.Locale);
            return View(contentTranslations.ToList().OrderByDescending(c => c.ContentID));
        }

        // GET: ContentTranslations/Details/5
        public ActionResult Details(int? contentId, int? localeId)
        {
            if (contentId == null || localeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentTranslation contentTranslation = 
                db.ContentTranslations.FirstOrDefault(c => c.ContentID == contentId && c.LocaleID == localeId);

            if (contentTranslation == null)
            {
                return HttpNotFound();
            }
            return View(contentTranslation);
        }

        // GET: ContentTranslations/Create
        public ActionResult Create()
        {
            ViewBag.ContentID = new SelectList(db.Contents, "ContentID", "ContentID");
            ViewBag.LocaleID = new SelectList(db.Locales, "LocaleID", "LocaleDesc");
            return View();
        }

        // POST: ContentTranslations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContentID,LocaleID,ContentTitle,ContentCaptionText,ContentDesc")] ContentTranslation contentTranslation)
        {
            contentTranslation.CreateDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.ContentTranslations.Add(contentTranslation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContentID = new SelectList(db.Contents, "ContentID", "ContentName", contentTranslation.ContentID);
            ViewBag.LocaleID = new SelectList(db.Locales, "LocaleID", "LocaleDesc", contentTranslation.LocaleID);
            return View(contentTranslation);
        }

        // GET: ContentTranslations/Edit/5
        public ActionResult Edit(int? contentId, int? localeId)
        {
            if (contentId == null || localeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentTranslation contentTranslation =
                db.ContentTranslations.FirstOrDefault(c => c.ContentID == contentId && c.LocaleID == localeId);

            if (contentTranslation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContentID = new SelectList(db.Contents, "ContentID", "ContentName", contentTranslation.ContentID);
            ViewBag.LocaleID = new SelectList(db.Locales, "LocaleID", "LocaleDesc", contentTranslation.LocaleID);
            return View(contentTranslation);
        }

        // POST: ContentTranslations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContentID,LocaleID,ContentTitle,ContentCaptionText,ContentDesc,CreateDate")] ContentTranslation contentTranslation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentTranslation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContentID = new SelectList(db.Contents, "ContentID", "ContentName", contentTranslation.ContentID);
            ViewBag.LocaleID = new SelectList(db.Locales, "LocaleID", "LocaleDesc", contentTranslation.LocaleID);
            return View(contentTranslation);
        }

        // GET: ContentTranslations/Delete/5
        public ActionResult Delete(int? contentId, int? localeId)
        {
            if (contentId == null || localeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentTranslation contentTranslation =
                db.ContentTranslations.FirstOrDefault(c => c.ContentID == contentId && c.LocaleID == localeId);

            if (contentTranslation == null)
            {
                return HttpNotFound();
            }
            return View(contentTranslation);
        }

        // POST: ContentTranslations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? contentId, int? localeId)
        {
            ContentTranslation contentTranslation =
                db.ContentTranslations.FirstOrDefault(c => c.ContentID == contentId && c.LocaleID == localeId);

            db.ContentTranslations.Remove(contentTranslation);
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
