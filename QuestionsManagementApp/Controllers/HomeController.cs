using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestionsManagementApp.Data;
using QuestionsManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public int UserDepartmentReturner()
        {
            return context.ApplicationUsers.Where(a => a.Email == User.Identity.Name).FirstOrDefault().UserDepartment;
        }

        public IActionResult Index()
        {
            return View();
        }



        [Authorize(Roles ="Supervisor, Manager")]
        public IActionResult AllQuestionList(string Qtype, string Qstatus, string email)
        {
            ViewBag.types = new SelectList(context.QuestionTyes.Where(a => a.Department ==
            UserDepartmentReturner()).ToList(), "Id", "Name", Convert.ToInt32(Qtype));
            List<SelectListItem> ObjList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Answered", Value = "Answered" },
                new SelectListItem { Text = "Not Answered", Value = "none" }
            };

            if (Qtype == null && Qstatus == null && email == null)  
            {
                var listofQuestions = context.Questions.Where(a => a.Department == UserDepartmentReturner())
                    .OrderByDescending(a => a.CreatedDate).ToList();
                listofQuestions.ForEach(a => a.QuestionType = context.QuestionTyes.Find(a.Type));
                listofQuestions.ForEach(a => a.QuestionDepartment = context.Departments.Find(a.Department));
                ViewBag.Status = ObjList;
                return View(listofQuestions);
            }
            else
            {
                var listofQuestions = context.Questions.Where(a => a.Department == UserDepartmentReturner())
                    .OrderByDescending(a => a.CreatedDate).ToList();
                listofQuestions.ForEach(a => a.QuestionType = context.QuestionTyes.Find(a.Type));
                listofQuestions.ForEach(a => a.QuestionDepartment = context.Departments.Find(a.Department));

                if (Qtype != null)
                {
                    listofQuestions = listofQuestions.Where(a => a.Type == Convert.ToInt32(Qtype)).ToList();
                }
                if (Qstatus != null)
                {
                    if (Qstatus == "Answered")
                    {
                        listofQuestions = listofQuestions.Where(a => a.Answeredby != "none").ToList();
                    }
                    else
                    {
                        listofQuestions = listofQuestions.Where(a => a.Answeredby == Qstatus).ToList();
                    }
                    ObjList.Where(a => a.Value == Qstatus).FirstOrDefault().Selected = true;
                }
                if (email != null) 
                {
                    listofQuestions = listofQuestions.Where(a => a.Askedby == email).ToList();
                }
                ViewBag.Status = ObjList;
                ViewBag.email = email;
                return View(listofQuestions);
            }
        }

        [Authorize(Roles ="Technician")]
        public IActionResult MyQuestions()
        {
            var user = User.Identity.Name;
            var listofQuestions = context.Questions.Where(a => a.Askedby == user)
                .OrderByDescending(a => a.CreatedDate).ToList();
            listofQuestions.ForEach(a => a.QuestionType = context.QuestionTyes.Find(a.Type));
            return View(listofQuestions);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.types = new SelectList(context.QuestionTyes.ToList(), "Id", "Name", null, "ForeignDepartment.DepartmentName");
            ViewBag.departments = new SelectList(context.Departments.ToList(), "Id", "DepartmentName", UserDepartmentReturner());
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Questions questionDetails)
        {
            bool AttachmentCheck = true;
            questionDetails.Id = Guid.NewGuid().ToString();            
            questionDetails.CreatedDate = DateTime.Now;
            if (questionDetails.Answeredby == null)
            {
                questionDetails.Answeredby = "none";
                AttachmentCheck = false;
            }
            questionDetails.AnswertoQuestion = "";
            context.Questions.Add(questionDetails);
            context.SaveChanges();

            //For Saving Attachment
            if (AttachmentCheck)
            {
                var QuestionAttachment = new Attachment
                {
                    AttachmentData = questionDetails.Answeredby,
                    QuestionId = questionDetails.Id
                };
                context.QuestionAttachments.Add(QuestionAttachment);
                context.SaveChanges();

                //Now Changing Question Data.....
                var questiontoChange = context.Questions.Where(a => a.Id == questionDetails.Id).FirstOrDefault();
                questiontoChange.Answeredby = "none";
                context.Entry(questiontoChange).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            if (User.IsInRole("Technician"))
            {
                return RedirectToAction("MyQuestions");
            }
            else
            {
                return RedirectToAction("AllQuestionList");
            }
        }

        [Authorize(Roles ="Supervisor, Manager")]
        [HttpGet]
        public IActionResult AddAnswer(string id)
        {
            var question = context.Questions.Where(a => a.Id == id).FirstOrDefault();
            ViewBag.departments = new SelectList(context.Departments.ToList(), "Id", "DepartmentName", question.Department);
            ViewBag.types = new SelectList(context.QuestionTyes.ToList(), "Id", "Name",
                question.Department, "ForeignDepartment.DepartmentName");
            return View(question);
        }

        [Authorize(Roles = "Supervisor, Manager")]
        [HttpPost]
        public IActionResult AddAnswer(Questions question)
        {
            context.Entry(question).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("AllQuestionList");
        }


        [Authorize]
        public IActionResult SeeAnswer(string id)
        {
            var answer = context.Questions.Where(a => a.Id == id).FirstOrDefault();
            answer.QuestionType = context.QuestionTyes.Where(a => a.Id == answer.Type).FirstOrDefault();
            return View(answer);
        }

        [Authorize(Roles ="Supervisor, Manager")]
        public IActionResult Dashboard()
        {
            return View();
        }


        public IActionResult DashboardData(string dateTime)
        {
            int ReassignCase = 0, PossibleReopen = 0, LetterUpdate = 0, PoliceReport = 0, VaccationOvveride = 0, PeertoPeerRequest = 0,
            CasedApprovedinError = 0, CaseDeniedinError = 0, Other = 0;
            var questions = new List<Questions>();
            if (dateTime == null)
            {
                questions = context.Questions.Where(a => a.Department == UserDepartmentReturner()).ToList();
            }
            else
            {
                questions = context.Questions.Where(a => a.Department == UserDepartmentReturner()
                && a.CreatedDate.Value.Month == Convert.ToDateTime(dateTime).Month
                && a.CreatedDate.Value.Year == Convert.ToDateTime(dateTime).Year).ToList();
            }
            /////////First Department Catagories////////////
            ReassignCase = questions.Where(a => a.Type == 1).Count();
            PossibleReopen = questions.Where(a => a.Type == 2).Count();
            LetterUpdate = questions.Where(a => a.Type == 3).Count();
            PoliceReport = questions.Where(a => a.Type == 4).Count();
            VaccationOvveride = questions.Where(a => a.Type == 5).Count();
            PeertoPeerRequest = questions.Where(a => a.Type == 6).Count();
            CasedApprovedinError = questions.Where(a => a.Type == 7).Count();
            CaseDeniedinError = questions.Where(a => a.Type == 8).Count();
            Other = questions.Where(a => a.Type == 9).Count();
            ///////////Second department catogries////////////////////
            var OverideOVerCostlimit = questions.Where(a => a.Type == 10).Count();
            var PeertoPeer11 = context.Questions.Where(a => a.Type == 11).Count();
            var RequestforRework = context.Questions.Where(a => a.Type == 12).Count();
            var ApprovalDateError = context.Questions.Where(a => a.Type == 13).Count();
            var LostMedication = context.Questions.Where(a => a.Type == 14).Count();
            var VacationOverride15 = context.Questions.Where(a => a.Type == 15).Count();
            ///////////Third department Catagories///////////////////
            var JCodeUpdate = context.Questions.Where(a => a.Type == 16).Count();
            var SCodeupdate = context.Questions.Where(a => a.Type == 17).Count();
            var NebulizerSolution = context.Questions.Where(A => A.Type == 18).Count();
            var MedicalProcedure = context.Questions.Where(A => A.Type == 19).Count();
            var other20 = context.Questions.Where(A => A.Type == 20).Count();
            //////////////////End of All Types//////////////
            var totalquestions = questions.Count();
            var submittedToday = questions.Where(a => Convert.ToDateTime(a.CreatedDate).ToShortDateString() == DateTime.Today.ToShortDateString()).Count();
            var Answered = questions.Where(a => a.Answeredby != "none").Count();
            var UnAnswered = questions.Where(a => a.Answeredby == "none").Count();
            var deaprtmentname = context.Departments.Find(UserDepartmentReturner()).DepartmentName;
            if (UserDepartmentReturner() == 1)
            {
                var QuestionStatusChartResult = string.Format("{0}::{1}::{2}::{3}::{4}::{5}::{6}::{7}::{8}::{9}::{10}::{11}::{12}::{13}",
                    ReassignCase, PossibleReopen, LetterUpdate, PoliceReport, VaccationOvveride, PeertoPeerRequest, CasedApprovedinError,
                    CaseDeniedinError, Other, totalquestions, submittedToday, Answered, UnAnswered, deaprtmentname);
                return Ok(QuestionStatusChartResult);
            }
            else if (UserDepartmentReturner()==2)
            {
                var QuestionStatusChartResult = string.Format("{0}::{1}::{2}::{3}::{4}::{5}::{6}::{7}::{8}::{9}::{10}",
                    OverideOVerCostlimit, PeertoPeer11, RequestforRework, ApprovalDateError, LostMedication, 
                    VacationOverride15, totalquestions, submittedToday, Answered, UnAnswered, deaprtmentname);
                return Ok(QuestionStatusChartResult);
            }
            else
            {
                var QuestionStatusChartResult = string.Format("{0}::{1}::{2}::{3}::{4}::{5}::{6}::{7}::{8}::{9}",
                    JCodeUpdate, SCodeupdate, NebulizerSolution, MedicalProcedure, other20,
                    totalquestions, submittedToday, Answered, UnAnswered, deaprtmentname);
                return Ok(QuestionStatusChartResult);
            }
        }

        [HttpPost]
        public IActionResult DeleteQuestion(string QId)
        {
            var questionForDelete = context.Questions.Where(a => a.Id == QId).FirstOrDefault();
            context.Questions.Remove(questionForDelete);
            context.SaveChanges();
            return RedirectToAction("AllQuestionList");
        }


        
        public IActionResult PrintReport(string Qtype, string Qstatus, string email)
        {
            var questions = new List<Questions>();
            if (Qtype == null && Qstatus == null && email == null)
            {
                questions = context.Questions.Where(a => a.Department == UserDepartmentReturner())
                    .OrderBy(a => a.CaseNumber).ToList();
            }
            else
            {
                questions = context.Questions.Where(a => a.Department == UserDepartmentReturner())
                    .OrderBy(a => a.CaseNumber).ToList();

                if (Qtype != null)
                {
                    questions = questions.Where(a => a.Type == Convert.ToInt32(Qtype)).ToList();
                }
                if (Qstatus != null)
                {
                    if (Qstatus == "Answered")
                    {
                        questions = questions.Where(a => a.Answeredby != "none").ToList();
                    }
                    else
                    {
                        questions = questions.Where(a => a.Answeredby == Qstatus).ToList();
                    }
                }
                if (email != null)
                {
                    questions = questions.Where(a => a.Askedby == email).ToList();
                }
            }

            var departmentname = context.Departments.Find(UserDepartmentReturner());
            questions.ForEach(a => a.QuestionType = context.QuestionTyes.Find(a.Type));
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(departmentname + "_Questions_List");
            ws.Cells["A2"].Value = "Case No";
            ws.Cells["B2"].Value = "Member Name";
            ws.Cells["C2"].Value = "Date Created";
            ws.Cells["D2"].Value = "Type";
            ws.Cells["E2"].Value = "Urgent";
            ws.Cells["F2"].Value = "Description";
            ws.Cells["G2"].Value = "Asked by";
            ws.Cells["H2"].Value = "Answered by";
            ws.Cells["I2"].Value = "Answer of Question";

            int rowstart = 3;
            foreach (var item in questions)
            {
                ws.Cells[string.Format("A{0}", rowstart)].Value = item.CaseNumber;
                ws.Cells[string.Format("B{0}", rowstart)].Value = item.MemberName;
                ws.Cells[string.Format("C{0}", rowstart)].Style.Numberformat.Format = "dd-mm-yyyy";
                ws.Cells[string.Format("C{0}", rowstart)].Value = item.CreatedDate;
                ws.Cells[string.Format("D{0}", rowstart)].Value = item.QuestionType.Name;
                ws.Cells[string.Format("E{0}", rowstart)].Value = item.Urgent;
                ws.Cells[string.Format("F{0}", rowstart)].Value = item.Description;
                ws.Cells[string.Format("G{0}", rowstart)].Value = item.Askedby;
                ws.Cells[string.Format("H{0}", rowstart)].Value = item.Answeredby;
                ws.Cells[string.Format("I{0}", rowstart)].Value = item.AnswertoQuestion;
                rowstart++;
            }

            ws.Cells["A:I"].AutoFitColumns();
            ws.Cells["A2:I2"].Style.Font.Bold = true;
            ws.Cells["A2:I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["A2:I2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            ws.Cells["A2:I2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Red);
            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public IActionResult HelpView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AttachmentReturner(string questionid)
        {
            if (context.QuestionAttachments.Where(a => a.QuestionId == questionid).FirstOrDefault() != null)
            {
                var image = context.QuestionAttachments.Where(a => a.QuestionId == questionid).FirstOrDefault().AttachmentData;
                return Ok(image);
            }
            else
            {
                return Ok("No Attachment Found");
            }
        }
    }
}
