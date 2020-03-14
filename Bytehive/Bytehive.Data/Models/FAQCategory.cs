using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    [Table("faq_category")]
    public class FAQCategory
    {
        [Key]
        public Guid Id { get; set; }

        public string NameEN { get; set; }

        public string NameBG { get; set; }

        public int MyProperty { get; set; }

        public virtual ICollection<FAQ> FAQs { get; set; }

    }
}
