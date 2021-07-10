using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Models
{
    public class QuestionsTypes
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("ForeignDepartment")]
        public int Department { get; set; }
        public QuestionsDepartments ForeignDepartment { get; set; }
    }
}
