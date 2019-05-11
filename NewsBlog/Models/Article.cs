using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NewsBlog.Models
{
    public class Article
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Назва новини не може бути пуста.")]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Назва новини повинна бути більше 3х символів.")]       
        public string Name { get; set; }

        [Required(ErrorMessage = "Опис новини не може бути пустий.")]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Опис новини повинен бути більше 3х символів.")]
        public string Description { get; set; }

        [AllowHtml]
        [UIHint("tinymce_full_compressed")]
        public string Content { get; set; }
        public System.DateTime DateCreate { get; set; }
        public string CoverType { get; set; }
        public int ViewCount { get; set; }

        [Required(ErrorMessage = "Будь-ласка виберіть категорію.")]        
        public int CategoriesID { get; set; }

        public string UserID { get; set; }
        public bool isAprove { get; set; }
        public int CommentsCount { get; set; }
        public string CoverPath { get; set; }
        public bool isAdminChoose { get; set; }
        public bool isHotArticle { get; set; }

        public virtual Categories Categories { get; set; }
    }
}