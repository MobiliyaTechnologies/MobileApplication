using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSU_PORTABLE.Models
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        public string QuestionDesc { get; set; }
        public List<AnswerModel> Answers { get; set; }
    }
}
