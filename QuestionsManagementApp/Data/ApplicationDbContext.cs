using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestionsManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestionsManagementApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<QuestionsTypes> QuestionTyes { get; set; }
        public DbSet<QuestionsDepartments> Departments { get; set; } 
        public DbSet<Attachment> QuestionAttachments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //For Adding Types of Question in Hard Coded Form
            builder.Entity<QuestionsTypes>().HasData(
                new QuestionsTypes
                {
                    Id = 1,
                    Name = "Reassign Case",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 2,
                    Name = "Possible Reopen",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 3,
                    Name = "Letter Update",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 4,
                    Name = "Police Report",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 5,
                    Name = "Vacation Override",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 6,
                    Name = "Peer to Peer Request",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 7,
                    Name = "Case Approved in Error",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 8,
                    Name = "Case Denied in Error",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 9,
                    Name = "Other",
                    Department = 1
                }, new QuestionsTypes
                {
                    Id = 10,
                    Name = "Override Over Cost Limit",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 11,
                    Name = "Peer to Peer Request",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 12,
                    Name = "Request for Rework",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 13,
                    Name = "Approval Date Error",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 14,
                    Name = "Lost Medication",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 15,
                    Name = "Vacation Override",
                    Department = 2
                }, new QuestionsTypes
                {
                    Id = 16,
                    Name = "J-Code Update",
                    Department = 3
                }, new QuestionsTypes
                {
                    Id = 17,
                    Name = "S-Code Update",
                    Department = 3
                }, new QuestionsTypes
                {
                    Id = 18,
                    Name = "Nebulizer Solution",
                    Department = 3
                }, new QuestionsTypes
                {
                    Id = 19,
                    Name = "Medical Procedure",
                    Department = 3
                }, new QuestionsTypes
                {
                    Id = 20,
                    Name = "Other",
                    Department = 3
                });
            //For Adding Roles of the Users in Hard coded Form.
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Supervisor",
                    NormalizedName = "Supervisor"
                }, new IdentityRole
                {
                    Id = "2",
                    Name = "Manager",
                    NormalizedName = "Manager"
                }, new IdentityRole
                {
                    Id = "3",
                    Name = "Technician",
                    NormalizedName = "Technician"
                });
            //For Adding Question Departments
            builder.Entity<QuestionsDepartments>().HasData(
                new QuestionsDepartments
                {
                    Id = 1,
                    DepartmentName = "Operations Questions"
                }, new QuestionsDepartments
                {
                    Id = 2,
                    DepartmentName = "Customer Service/Escalations Questions"
                }, new QuestionsDepartments
                {
                    Id = 3,
                    DepartmentName = "Medical/Pega Questions"
                });
        }
    }
}
