using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BizContents.Controllers
{
    public class ContentsController : Controller
    {
        private CCH_CustomerDemoNCCTEntities db = new CCH_CustomerDemoNCCTEntities();

        // GET: Contents
        public ActionResult Index()
        {
            var contents = db.Contents
                .Include(c => c.Survey)
                .Include(c => c.ContentType)
                .Include(c => c.ContentTranslations);

            return View(contents.ToList().OrderByDescending(c => c.ContentID));
        }

        // GET: Contents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        // GET: Contents/Create
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> contentTypes = db.ContentTypes.ToList()
                .Select(c => new SelectListItem
                {
                    Text = c.ContentTypeDesc,
                    Value = c.ContentTypeID.ToString()
                });
            ViewBag.ContentTypeID = contentTypes;

            ViewBag.ContentID = new SelectList(db.Surveys, "SurveyID", "SurveyID");
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContentID,ContentTypeID,ContentTitle,ContentDesc,ContentSourceDesc,ContentPointsCount,ContentDurationSecondsCount,ContentCaptionText,IntroContentInd,CreateDate,ContentURL,ContentPhoneNum")] Content content)
        {
            content.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Contents.Add(content);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContentID = new SelectList(db.Surveys, "SurveyID", "SurveyID", content.ContentID);
            return View(content);
        }

        // GET: Contents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            IEnumerable<SelectListItem> contentTypes = db.ContentTypes.ToList()
                .Select(c => new SelectListItem
                {
                    Text = c.ContentTypeDesc,
                    Value = c.ContentTypeID.ToString()
                });
            ViewBag.ContentTypeID = contentTypes;

            ViewBag.ContentID = new SelectList(db.Surveys, "SurveyID", "SurveyID", content.ContentID);
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContentID,ContentTypeID,ContentTitle,ContentDesc,ContentSourceDesc,ContentPointsCount,ContentDurationSecondsCount,ContentCaptionText,IntroContentInd,CreateDate,ContentURL,ContentPhoneNum,ContentImageFileName,ContentFileLocationDesc,CreateDate")] Content content)
        {
            if (ModelState.IsValid)
            {
                db.Entry(content).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContentID = new SelectList(db.Surveys, "SurveyID", "SurveyID", content.ContentID);
            return View(content);
        }

        // GET: Contents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Content content = db.Contents.Find(id);
            db.Contents.Remove(content);
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
