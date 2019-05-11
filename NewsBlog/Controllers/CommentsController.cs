using Microsoft.AspNet.Identity;
using NewsBlog.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NewsBlog.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public async Task<ActionResult> Index()
        {
            return View(await db.Comments.ToListAsync());
        }

        public FileResult GetArticleImage(int id)
        {
            var cover = db.Articles.FirstOrDefault(p => p.ID == id);
            if (cover.CoverType != null)
            {
                return File(cover.CoverPath, cover.CoverType);
            }
            else
            {
                return null;
            }
        }

        public async Task<JsonResult> GetRecentComments()
        {
            var comments = (from c in db.Comments join u in db.Users on c.UserId equals u.Id join a in db.Articles on c.ArticleId equals a.ID orderby c.CommentTime descending select new {
                c.CommentBody,
                c.CommentTime,
                a.ID,
                a.Name,
                u.FirstName,
                u.LastName
            }).Take(5).ToListAsync();
            var comm =  Json(await comments, JsonRequestBehavior.AllowGet);
            comm.MaxJsonLength = int.MaxValue;
            return comm;
        }

        [AllowAnonymous]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Any, Duration = 60)]
        public FileContentResult GetUserImage(string id)
        {
            var cover = db.Users.FirstOrDefault(p => p.Id == id);
            if (cover.Cover != null && cover.CoverType != null)
            {
                return File(cover.Cover, cover.CoverType);
            }
            else
            {
                string imageFile = System.Web.HttpContext.Current.Server.MapPath("~/Content/user.png");
                var srcImage = Image.FromFile(imageFile);
                var stream = new MemoryStream();
                srcImage.Save(stream, ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }
        }



        [HttpGet]
        public async Task<JsonResult> CommentWall(int id)
        {
            var comments = (from c in db.Comments join u in db.Users on c.UserId equals u.Id
                            where c.ArticleId == id orderby c.CommentTime descending
                            select new
                            {   
                                c.CommentId,
                                u.FirstName,
                                u.LastName,
                                c.CommentBody,
                                u.Cover,
                                u.Id,
                                c.CommentTime,
                                c.ParentId,
                            }).ToListAsync();
            var comm = Json(await comments, JsonRequestBehavior.AllowGet);
            comm.MaxJsonLength = int.MaxValue;
            return comm;
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddComment(Comments comment)
        {
            if (ModelState.IsValid)
            {
                Article article = db.Articles.Find(comment.ArticleId);
                article.CommentsCount += 1;
                comment.CommentTime = DateTime.Now;
                db.Entry(article).State = EntityState.Modified;
                comment.UserId = User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return Json(new { });
        }


        // GET: Comments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = await db.Comments.FindAsync(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CommentBody,CommentTime,ArticleId,UserId")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comments);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(comments);
        }

        // GET: Comments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = await db.Comments.FindAsync(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CommentBody,CommentTime,ArticleId,UserId")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comments).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(comments);
        }

        // GET: Comments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = await db.Comments.FindAsync(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Comments comments = await db.Comments.FindAsync(id);
            db.Comments.Remove(comments);
            await db.SaveChangesAsync();
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
