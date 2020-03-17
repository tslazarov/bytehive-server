using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bytehive.Models
{
    public class FAQ
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
