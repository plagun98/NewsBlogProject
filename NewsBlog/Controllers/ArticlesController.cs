using LibraryAngular.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NewsBlog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NewsBlog.Controllers
{
    public class ArticlesController : Controller
    {
        private ApplicationDbContext db;

        public ArticlesController()
        {
           db = new ApplicationDbContext();
        }

        public ActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            ViewBag.User = User.Identity.Name;

            var articles = db.Articles.ToList().OrderByDescending(p => p.ID).ToPagedList(pageNumber, pageSize);
            return View("Index", articles);
        }


        public async Task<JsonResult> GetUserInfoByArticle(int id)
        {
            var user = (from a in db.Articles
                        join u in db.Users on a.UserID equals u.Id
                        select new
                        {
                            a.ID,
                            u.FirstName,
                            u.LastName
                        }).Where(x => x.ID == id).ToListAsync();

            var json = Json(await user, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public async Task<JsonResult> GetPublishedArticlesBySearch(string term)
        {
            var articles = (from article in db.Articles
                            join u in db.Users on article.UserID equals u.Id
                            where article.Name.Contains(term) ||
                            article.Description.Contains(term) ||
                            article.Content.Contains(term)
                        orderby article.ID descending
                        select article).Where(x=>x.isAprove==true).ToListAsync();
            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public async Task<JsonResult> GetCategories()
        {
            var categories = (from c in db.Categories select new
            {
                c.ID,
                c.CategoryName
            }).ToListAsync();
            return Json(await categories, JsonRequestBehavior.AllowGet); ;
        }

            public async Task<JsonResult> GetPublishedArticlesByCategory(string category)
        {
            var articles = (from a in db.Articles
                           join u in db.Users on a.UserID equals u.Id
                           where a.isAprove == true
                           orderby a.ViewCount descending
                           where a.Categories.CategoryName == category && a.isAprove==true
                           orderby a.ID 
                           descending
                           select new {
                            a.ID,
                            a.Name,
                            a.isAprove,
                            a.ViewCount,
                            a.CommentsCount,
                            a.DateCreate,
                            a.Description,
                            a.Categories.CategoryName,
                            u.FirstName,
                            u.LastName,
                            u.Id
            }).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }

        public async Task<JsonResult> GetPublishedAdminChoosedArticles()
        {
            var articles = (from a in db.Articles
                            join u in db.Users on a.UserID equals u.Id
                            where a.isAprove == true
                            orderby a.ViewCount descending
                            where a.isAdminChoose == true && a.isAprove == true
                            orderby a.ID
                            descending
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove,
                                a.ViewCount,
                                a.CommentsCount,
                                a.DateCreate,
                                a.Description,
                                a.Categories.CategoryName,
                                u.FirstName,
                                u.LastName,
                                u.Id
                            }).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public async Task<JsonResult> GetPublishedArticlesByCommentsCount()
        {
            var articles = (from a in db.Articles
                            join u in db.Users on a.UserID equals u.Id
                            where a.isAprove == true
                            orderby a.ViewCount descending
                            where a.isAprove == true
                            orderby a.CommentsCount
                            descending
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove,
                                a.ViewCount,
                                a.CommentsCount,
                                a.DateCreate,
                                a.Description,
                                a.Categories.CategoryName,
                                u.FirstName,
                                u.LastName,
                                u.Id
                            }).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }


        public async Task<JsonResult> GetPopularArticlesByCategory(string category)
        {
            var articles = (from a in db.Articles
                            join u in db.Users on a.UserID equals u.Id
                            where a.isAprove == true
                            orderby a.ViewCount descending
                            where a.Categories.CategoryName == category && a.isAprove == true
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove,
                                a.ViewCount,
                                a.CommentsCount,
                                a.DateCreate,
                                a.Description,
                                a.Categories.CategoryName,
                                u.FirstName,
                                u.LastName,
                                u.Id
                            }).Take(4).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }


        public async Task<JsonResult> GetPublishedHotArticles()
        {
            var articles = (from a in db.Articles
                            join u in db.Users on a.UserID equals u.Id
                            where a.isAprove == true
                            orderby a.ViewCount descending
                            where a.isHotArticle==true && a.isAprove == true
                            orderby a.ID
                            descending
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove,
                                a.ViewCount,
                                a.CommentsCount,
                                a.DateCreate,
                                a.Description,
                                a.Categories.CategoryName,
                                u.FirstName,
                                u.LastName,
                                u.Id
                            }).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;

        }

      
        public async Task<JsonResult> GetTop3PerWeekArticles()
        {
 
            var now = DateTime.Now;

            int days = now.DayOfWeek - DayOfWeek.Sunday;
            DateTime weekStart = now.AddDays(-days);
            DateTime weekEnd = weekStart.AddDays(7);

            var articles = (from a in db.Articles
                                where a.DateCreate <= weekEnd && a.DateCreate >= weekStart
                                orderby a.ViewCount descending
                                select new { a.ID, a.Name, a.ViewCount, a.CommentsCount, a.isAprove }).Where(x => x.isAprove == true).Take(3).ToListAsync();
                var json = Json(await articles, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;

        }

        public async Task<JsonResult> GetPublishedArticles()
        {
            var articles = (from a in db.Articles join u in db.Users on a.UserID equals u.Id
                            where a.isAprove == true
                            orderby a.DateCreate descending
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove,
                                a.ViewCount,
                                a.CommentsCount,
                                a.DateCreate,
                                a.Description,
                                a.Categories.CategoryName,
                                a.isHotArticle,
                                a.isAdminChoose,
                                u.FirstName,
                                u.LastName,
                                u.Id
                            }).ToListAsync();
            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }


        [Authorize]
        public async Task<JsonResult> GetArticlesByUser()
        {
            string userId = User.Identity.GetUserId();
            var articles = (from a in db.Articles
                       where a.UserID == userId && a.isAprove == true
                       orderby a.ViewCount descending
                       select new {
                           a.ID,
                           a.Name,
                           a.isAprove,
                           a.ViewCount,
                           a.CommentsCount,
                       }).ToListAsync();

            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [Authorize]
        public async Task<JsonResult> GetUnpublishedArticles()
        {
            string userId = User.Identity.GetUserId();

            var articles = (from a in db.Articles
                            where a.UserID == userId && a.isAprove == false orderby a.DateCreate
                            select new
                            {
                                a.ID,
                                a.Name,
                                a.isAprove
                            }).ToListAsync();
               
            var json = Json(await articles, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }


        [AllowAnonymous]
        public FileResult GetCoverImage(int id)
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

        [AllowAnonymous]
        public FileResult GetImageTop(int id)
        {
            var result = db.Articles.FirstOrDefault(a => a.ID == id);
            if (result != null && result.CoverPath != null && result.CoverType != null)
            {
                return File(result.CoverPath, result.CoverType);
            }
            else
            {
                return null;
            }
        }

        private List<Categories> GetOptions()
        {
            return db.Categories.ToList();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        public ActionResult Create()
        {
            Article article = new Article();
            ViewBag.Categories = GetOptions();
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Content, DateCreate, Cover, CoverType, Description, CategoriesID")] Article article, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                    article.DateCreate = DateTime.Now;
                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var currentUser = manager.FindById(User.Identity.GetUserId());
                    article.UserID = User.Identity.GetUserId();
                    db.Articles.Add(article);
                    db.SaveChanges();

                    Directory.CreateDirectory(Server.MapPath("~/App_Data/covers/" + article.ID));

                if (file != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        article.CoverType = file.ContentType;
                    }
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(Server.MapPath("~/App_Data/covers/" + article.ID), fileName);
                    file.SaveAs(path);

                    article.CoverPath = Globals.resolveVirtual(path);
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["message"] = "Стаття буде доступна після пітдвердження адміном";

                return RedirectToAction("Index","Manage");
            }
            return View(article);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            ViewBag.Categories = GetOptions();
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriesID = new SelectList(db.Categories, "ID", "CategoryName", article.CategoriesID);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID, Name, CategoryName, Content, CategoriesID, Description, CoverPath, CoverType,  isAprove")] Article article, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                        var path = Path.Combine(Server.MapPath("~/App_Data/covers/" + article.ID), file.FileName);
                        article.CoverPath = path;
                        article.CoverType = file.ContentType;
                        
                    file.SaveAs(path);
                }
               
                db.Entry(article).State = EntityState.Modified;

                if (file != null)
                {
                    db.Entry(article).Property(r => r.CoverPath).IsModified = true;
                    db.Entry(article).Property(r => r.CoverType).IsModified = true;
                }
                else
                {
                    db.Entry(article).Property(r => r.CoverPath).IsModified = false;
                    db.Entry(article).Property(r => r.CoverType).IsModified = false;
                }
                db.Entry(article).Property(r => r.UserID).IsModified = false;
                db.Entry(article).Property(r => r.DateCreate).IsModified = false;
                db.Entry(article).Property(r => r.isAprove).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index","Admin");
            }
            return View(article);
        }

        public ActionResult isAproved([Bind(Include = "isAprove")] int id)
        {
            Article article = db.Articles.Find(id);
            article.isAprove = true;
            db.Entry(article).Property(r => r.isAprove).IsModified = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult setHotArticle([Bind(Include = "isHotArticle")] int id)
        {
            Article article = db.Articles.Find(id);
            if (article.isHotArticle == false)
            {
                article.isHotArticle = true;
                db.Entry(article).Property(r => r.isHotArticle).IsModified = true;
                db.SaveChanges();
            }
            else
            {
                article.isHotArticle = false;
                db.Entry(article).Property(r => r.isHotArticle).IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index","Admin");
        }

        public ActionResult setAdminChoosedArticle([Bind(Include = "isAdminChoose")] int id)
        {
            Article article = db.Articles.Find(id);
            if (article.isAdminChoose == false)
            {
                article.isAdminChoose = true;
                db.Entry(article).Property(r => r.isAdminChoose).IsModified = true;
                db.SaveChanges();
            }
            else
            {
                article.isAdminChoose = false;
                db.Entry(article).Property(r => r.isAdminChoose).IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index","Admin");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Article article = db.Articles.Find(id);
                db.Articles.Remove(article);
                db.SaveChanges();
                return Json(new { Success = true });
            }
            catch
            {//TODO: Log error				
                return Json(new { Success = false });
            }
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
