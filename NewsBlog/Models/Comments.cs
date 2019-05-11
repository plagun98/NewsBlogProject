using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace NewsBlog.Models
{
    public class Comments
    {
        [Key]
        public int CommentId { get; set; }
        public int ParentId { get; set; }
        [AllowHtml]
        public string CommentBody { get; set; }
        [DataType(DataType.DateTime)]
        public System.DateTime CommentTime { get; set; }
        public virtual int ArticleId { get; set; }
        public virtual string UserId { get; set; }
    }
}