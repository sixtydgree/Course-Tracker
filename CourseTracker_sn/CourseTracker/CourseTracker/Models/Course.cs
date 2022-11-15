using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CourseTracker.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }


        public int TermId { get; set; }

        [MaxLength(100)]
        public string CourseName { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }


        public string Status { get; set; }

        [MaxLength(1000)]
        public string CourseNotes { get; set; }

        [MaxLength(100)]
        public string InstructorName { get; set; }

        [MaxLength(250)]
        public string InstructorEmail { get; set; }


        public string InstructorPhone { get; set; }

        [MaxLength(100)]
        public string ObjectiveName { get; set; }


        public DateTime ObjectiveStart { get; set; }


        public DateTime ObjectiveDue { get; set; }

        [MaxLength(100)]
        public string PerformanceName { get; set; }


        public DateTime PerformanceStart { get; set; }


        public DateTime PerformanceDue { get; set; }

        public bool CourseNotification { get; set; }

        public bool PerformanceNotification { get; set; }

        public bool ObjectiveNotification { get; set; }
    }
}
