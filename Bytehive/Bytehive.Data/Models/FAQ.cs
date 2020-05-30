using Bytehive.Data.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bytehive.Data.Models
{
    [Table("faq")]
    public class FAQ : IIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        public string QuestionEN { get; set; }

        public string QuestionBG { get; set; }

        public string AnswerEN { get; set; }

        public string AnswerBG { get; set; }

        public FAQCategory Category { get; set; }

        public Guid CategoryId { get; set; }
    }
}
