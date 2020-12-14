using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace doan_1.Models
{
    public class Bill
    {
        [Key]
        public int MaBill { get; set; }
        [Required(ErrorMessage = "Enter the issued date.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }
        public float TongHoaDon { get; set; }
        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
    }
}