using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace doan_1.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public string NoiDungBL { get; set; }
        [ForeignKey("Book")]
        public int BookID { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
    }
}