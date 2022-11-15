using System;
using System.Collections.Generic;
using System.Text;
using Plugin.LocalNotifications;
using SQLite;


namespace CourseTracker.Models
{
    public class Notifications
    {
        [PrimaryKey, AutoIncrement]
        public int NotificationId { get; set; }
        public int CourseId { get; set; }

        public string PerformanceName { get; set; }
        
        public string ObjectiveName { get; set; }

        public void SetCourseStartNotification(string courseName, string startDate, DateTime date)
        {
            CrossLocalNotifications.Current.Show("Next Course", $"{courseName} begins tomorrow ({startDate}).", NotificationId, date.AddHours(-12));


        }

        public void SetCourseEndNotification(string courseName, string endDate, DateTime date)
        {
            CrossLocalNotifications.Current.Show("Course Ending", $"{courseName} will end tomorrow {endDate}.", NotificationId, date.AddHours(-12));
        }

        public void CancelNotification(int notificationId)
        {
            CrossLocalNotifications.Current.Cancel(notificationId);
        }

        public void SetAssessmentStartNotification(string assessmentName, string startDate, DateTime date)
        {
            CrossLocalNotifications.Current.Show("Assessment beginning", $"{assessmentName} begins tomorrow{startDate}.", NotificationId, date.AddHours(-12));
        }


        public void SetAssessmentEndNotification(string assessmentName, string endDate, DateTime date)
        {
            CrossLocalNotifications.Current.Show("Assessment due", $"{assessmentName} is due tomorrow ({endDate})", NotificationId, date.AddHours(-12));
        }

    }
}
