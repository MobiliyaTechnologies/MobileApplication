using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM_PORTABLE.Models
{
    public class FeedbackModel
    {

        public int RoomId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string FeedbackDesc { get; set; }
    }
}
