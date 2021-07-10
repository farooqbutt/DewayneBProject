using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Models
{
    public class Questions
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string CaseNumber { get; set; }
        [Required]
        public string MemberName { get; set; }
        [Required]
        public string Urgent { get; set; }
        [Required,MaxLength(250)]
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Askedby { get; set; }

        [Required]
        [MaxLength(500)]
        public string AnswertoQuestion { get; set; }
        public string Answeredby { get; set; }

        [Required]
        [ForeignKey("QuestionType")]
        public int Type { get; set; }
        public QuestionsTypes QuestionType { get; set; }

        [Required]
        [ForeignKey("QuestionDepartment")]
        public int Department { get; set; }
        public QuestionsDepartments QuestionDepartment { get; set; } 
    }
}
