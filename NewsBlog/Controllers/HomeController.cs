using NewsBlog.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NewsBlog.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public static int Counter { get; set; }

        [HttpGet]
        public ActionResult AjaxMostPopular()
        {
            var model = (from a in db.Articles orderby a.ViewCount descending select a).Where(x=>x.isAprove==true).Take(3);
            Counter = 2;
            return PartialView("_AjaxMostPopular", model);
        }

        [HttpPost]
        [ActionName("AjaxMostPopular")]
        public ActionResult PartialGridPost(int count = 3)
        {
            count = count + Counter;
            var model = (from a in db.Articles orderby a.ViewCount descending select a).Where(x=>x.isAprove==true).Take(count);
            Counter += 2;
            return PartialView("_AjaxMostPopular", model);
        }

        [AllowAnonymous]
        [OutputCache(Duration = 180)]
        public FileContentResult GetUserImage(string id)
        {
            var cover = db.Users.FirstOrDefault(p => p.Id == id);
            if (cover.Cover != null && cover.CoverType!=null)
            {
                var image = Image.FromStream(new MemoryStream(cover.Cover));
                image = ResizeImage(image, 150, CalculateHeight(image));
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), cover.CoverType);
                }
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

        private int CalculateHeight(Image image)
        {
            return (int)image.Height * 150 / image.Width;
        }

        private Image ResizeImage(Image originalImage,
                     /* note changed names */
                     int canvasWidth, int canvasHeight)
        {
            Image image = originalImage;
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                             100L);

            return thumbnail;
        }

        [AllowAnonymous]
        public ActionResult news([Bind(Include = "ViewCount")] int? id)
        {
            Article article = db.Articles.Find(id);
            
            if (article != null)
            {
                article.ViewCount += 1;
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return View("article", article);
            }
            else
            {
                return null;
            }
        }

        public ActionResult Index()
        {
            ViewBag.S = TempData["message"] as string;
            return View();
        }
    }
    }