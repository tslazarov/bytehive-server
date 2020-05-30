using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Models.Faq
{
    public class FaqEditModel
    {
        public string QuestionEN { get; set; }

        public string QuestionBG { get; set; }

        public string AnswerEN { get; set; }

        public string AnswerBG { get; set; }

        public Guid CategoryId { get; set; }
    }
}
