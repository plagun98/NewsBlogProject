using NewsBlog.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NewsBlog.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var currentUserEmail = System.Web.HttpContext.Current?.User?.Identity?.Name;
            if (currentUserEmail != null)
            {
                var currentUser = _context.Users.FirstOrDefault(x => x.Email.Equals(currentUserEmail));
                if (currentUser != null && currentUser.Roles.Any(x => x.RoleId.Equals("1")))
                    return View("Index");
            }            
            return RedirectToAction("NoAccess");
        }

        public ActionResult NoAccess()
        {
            return View("NoAccess");
        }

        public async Task<JsonResult> GetArticlesByFilter(string filter)
        {
            if (filter == "all")
            {
                var articles = (from ar in _context.Articles orderby ar.DateCreate descending select new { ar.ID, ar.Name, ar.isAprove }).ToListAsync();
                var a = Json(await articles, JsonRequestBehavior.AllowGet);
                a.MaxJsonLength = int.MaxValue;
                return a;
            }
            else if (filter == "today")
            {
                var articles = (from ar in _context.Articles orderby ar.DateCreate descending select new { ar.ID, ar.Name, ar.isAprove, ar.DateCreate }).Where(x => x.DateCreate.Day == DateTime.Now.Day).ToListAsync();
                var a = Json(await articles, JsonRequestBehavior.AllowGet);
                a.MaxJsonLength = int.MaxValue;
                return a;
            }
            else if (filter == "weekly")
            {
                var articles = (from ar in _context.Articles orderby ar.DateCreate descending select new { ar.ID, ar.Name, ar.isAprove, ar.DateCreate }).Where(x => x.DateCreate.Month == DateTime.Now.Month).ToListAsync();
                var a = Json(await articles, JsonRequestBehavior.AllowGet);
                a.MaxJsonLength = int.MaxValue;
                return a;
            }
            else if (filter == "notallowed")
            {
                var articles = (from ar in _context.Articles orderby ar.DateCreate descending select new { ar.ID, ar.Name, ar.isAprove }).Where(x => x.isAprove == false).ToListAsync();
                var a = Json(await articles, JsonRequestBehavior.AllowGet);
                a.MaxJsonLength = int.MaxValue;
                return a;
            }
            else
            {
                return new JsonResult { };
            };
        }

        public async Task<JsonResult> GetArticles()
        {
            var articles = (from article in _context.Articles
                            orderby article.DateCreate descending
                            select new
                            {
                                article.ID,
                                article.Name,
                                article.Description,
                                article.Categories,
                                article.isAprove,
                                article.isHotArticle,
                                article.isAdminChoose
                            }).ToListAsync();
            var a = Json(await articles, JsonRequestBehavior.AllowGet);
            a.MaxJsonLength = int.MaxValue;
            return a;
        }
        public async Task<JsonResult> GetCategories()
        {
            var categories = (from cat in _context.Categories select cat).ToListAsync();
            var jsonCat = Json(await categories, JsonRequestBehavior.AllowGet);
            jsonCat.MaxJsonLength = int.MaxValue;
            return jsonCat;
        }

        public ActionResult isAproved([Bind(Include = "isAprove")] int id)
        {
            Article article = _context.Articles.Find(id);
            if (article.isAprove == false)
            {
                article.isAprove = true;
                _context.Entry(article).Property(r => r.isAprove).IsModified = true;
                _context.SaveChanges();
            }
            else
            {
                article.isAprove = false;
                _context.Entry(article).Property(r => r.isAprove).IsModified = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}