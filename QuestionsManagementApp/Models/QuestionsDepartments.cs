using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Models
{
    public class QuestionsDepartments
    {
        [Key]
        public int Id { get; set; }
        public string DepartmentName { get; set; }
    }
}
