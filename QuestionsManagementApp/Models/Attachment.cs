using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        public string AttachmentData { get; set; }

        [ForeignKey("QuestionIdFoerign")]
        public string QuestionId { get; set; }
        public Questions QuestionIdFoerign { get; set; }
    }
}
