using CourseTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCoursePage : ContentPage
    {

        Course course;
        int courseId;
        public bool courseNotify;
        public bool perfNotify;
        public bool objNotify;
        public EditCoursePage(int courseId)
        {
            InitializeComponent();

            this.courseId = courseId;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();



            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                var courses = conn.Table<Course>().Where(c => c.CourseId == courseId);
                course = courses.FirstOrDefault();
            }
            courseNameEntry.Text = course.CourseName;
            courseStartDate.Date = course.StartDate;
            courseEndDate.Date = course.EndDate;
            status.SelectedItem = course.Status;
            instructorName.Text = course.InstructorName;
            instructorPhone.Text = course.InstructorPhone;
            instructorEmail.Text = course.InstructorEmail;
            performanceName.Text = course.PerformanceName;
            performanceStart.Date = course.PerformanceStart;
            performanceEnd.Date = course.PerformanceDue;
            objectiveName.Text = course.ObjectiveName;
            objectiveStart.Date = course.ObjectiveStart;
            objectiveEnd.Date = course.ObjectiveDue;
            courseNotification.IsChecked = course.CourseNotification;
            performanceNotification.IsChecked = course.PerformanceNotification;
            objectiveNotification.IsChecked = course.ObjectiveNotification;
            courseNotify = course.CourseNotification;
            perfNotify = course.PerformanceNotification;
            objNotify = course.ObjectiveNotification;
        }


        // Save behavior----------------------------------------------------------------------------------------------------------------
        private void saveBtn_Clicked(object sender, EventArgs e)
        {
            if(IsUniqueAssessmentName(objectiveName.Text) && IsUniqueAssessmentName(performanceName.Text) && IsCourseEndLater() && IsPerformanceEndLater() && IsObjectiveEndLater() && AreAllEntriesFilled() && IsPhoneNumber() && IsEmail() && IsInsideTerm() && IsInsideCourse())
            {
                course.CourseName = courseNameEntry.Text;
                course.StartDate = courseStartDate.Date;
                course.EndDate = courseEndDate.Date;
                course.Status = status.SelectedItem.ToString();
                course.InstructorName = instructorName.Text;
                course.InstructorPhone = instructorPhone.Text;
                course.InstructorEmail = instructorEmail.Text;
                course.PerformanceName = performanceName.Text;
                course.PerformanceStart = performanceStart.Date;
                course.PerformanceDue = performanceEnd.Date;
                course.ObjectiveName = objectiveName.Text;
                course.ObjectiveStart = objectiveStart.Date;
                course.ObjectiveDue = objectiveEnd.Date;
                course.CourseNotification = courseNotify;
                course.PerformanceNotification = perfNotify;
                course.ObjectiveNotification = objNotify;

                //set objective assessment notifications----------------------------------------------------------------
                Notifications oStart = new Notifications()
                {
                    ObjectiveName = course.ObjectiveName
                };
                Notifications oEnd = new Notifications()
                {
                    ObjectiveName = course.ObjectiveName
                };

                if (objNotify)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.CreateTable<Notifications>();
                        var notifications = conn.Table<Notifications>().Where(n => n.ObjectiveName == course.ObjectiveName).ToList();
                        if (notifications.Count == 0)
                        {
                            int rows = conn.Insert(oStart);
                            int rows2 = conn.Insert(oEnd);

                            if (rows > 0 && rows2 > 0)
                            {
                                oStart.SetAssessmentStartNotification(course.ObjectiveName, course.ObjectiveStart.ToString(), course.ObjectiveStart);
                                oEnd.SetAssessmentEndNotification(course.ObjectiveName, course.ObjectiveDue.ToString(), course.ObjectiveDue);
                            }
                        }
                        

                    }
                }


                //set performance objective notifications---------------------------------------------------------------
                Notifications pStart = new Notifications()
                {
                    PerformanceName = course.PerformanceName
                };
                Notifications pEnd = new Notifications()
                {
                    PerformanceName = course.PerformanceName
                };

                if (perfNotify)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.CreateTable<Notifications>();
                        var perfNotifications = conn.Table<Notifications>().Where(n => n.PerformanceName == course.PerformanceName);
                        if(perfNotifications.Count() == 0)
                        {
                            int rows = conn.Insert(pStart);
                            int rows2 = conn.Insert(pEnd);

                            if (rows > 0 && rows2 > 0)
                            {
                                pStart.SetAssessmentStartNotification(course.PerformanceName, course.PerformanceStart.ToString(), course.PerformanceStart);
                                pEnd.SetAssessmentEndNotification(course.PerformanceName, course.PerformanceDue.ToString(), course.PerformanceDue);
                            }
                        }
                        

                    }
                }

                //set course notifications------------------------------------------------------------------------------------
                Notifications startNotification = new Notifications()
                {
                    CourseId = course.CourseId
                };
                Notifications endNotification = new Notifications()
                {
                    CourseId = course.CourseId
                };

                if (courseNotify)
                {

                    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.CreateTable<Notifications>();
                        var courseNotifications = conn.Table<Notifications>().Where(n => n.CourseId == course.CourseId);
                        if(courseNotifications.Count() == 0)
                        {
                            int rows = conn.Insert(startNotification);
                            int rows2 = conn.Insert(endNotification);

                            if (rows > 0 && rows2 > 0)
                            {
                                startNotification.SetCourseStartNotification(course.CourseName, course.StartDate.ToString(), course.StartDate);
                                endNotification.SetCourseEndNotification(course.CourseName, course.EndDate.ToString(), course.EndDate);
                            }
                        }
                        
                    }
                }


                //save course edit------------------------------------------------------------------------------------------------------------
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Course>();
                    int rows = conn.Update(course);

                    if(rows > 0)
                    {
                        DisplayAlert("Updated!", "Course successfully updated.", "Ok");
                        Navigation.PopAsync();
                    }
                    else
                    {
                        DisplayAlert("Failed!", "Course did not update.", "Ok");
                    }
                }
            }
            
        }

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            
                Navigation.PopAsync();
        }

        //Error control to ensure all fields are filled out properly----------------------------------------------------------------

        private bool IsInsideTerm()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                var terms = conn.Table<Term>().Where(t => t.TermId == course.TermId);
                Term term = terms.FirstOrDefault();
                if (courseStartDate.Date < term.StartDate || courseEndDate.Date > term.EndDate)
                {
                    DisplayAlert("Outside of term", "The course cannot start or end outside the term start and end dates.", "Ok");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool IsInsideCourse()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                if (performanceStart.Date < courseStartDate.Date || performanceEnd.Date > courseEndDate.Date || objectiveStart.Date < courseStartDate.Date || objectiveEnd.Date > courseEndDate.Date)
                {
                    DisplayAlert("Outside of course", "Assessments cannot start or end outside of the course start and end.", "Ok");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private bool IsCourseEndLater()
        {
            if (courseEndDate.Date > courseStartDate.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "The course end date must be later than the start date.", "Ok");
                courseStartDate.BackgroundColor = Color.Coral;
                courseEndDate.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool IsPerformanceEndLater()
        {
            if (performanceEnd.Date > performanceStart.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "The performance end date must be later than its start date.", "Ok");
                performanceEnd.BackgroundColor = Color.Coral;
                performanceStart.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool IsObjectiveEndLater()
        {
            if (objectiveEnd.Date > objectiveStart.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "The objective end date must be later than its start date.", "Ok");
                objectiveStart.BackgroundColor = Color.Coral;
                objectiveEnd.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool AreAllEntriesFilled()
        {
            if (courseNameEntry.Text != null && instructorName != null && instructorPhone != null && instructorEmail != null && performanceName != null && objectiveName != null)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "All fields must be filled.", "Ok");
                courseNameEntry.BackgroundColor = Color.Coral;
                instructorName.BackgroundColor = Color.Coral;
                instructorEmail.BackgroundColor = Color.Coral;
                instructorPhone.BackgroundColor = Color.Coral;
                performanceName.BackgroundColor = Color.Coral;
                objectiveName.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool IsPhoneNumber()
        {
            int charCount = 0;
            foreach (char n in instructorPhone.Text)
            {
                if (int.TryParse(n.ToString(), out int value))
                {
                    charCount++;

                }
            }
            if (charCount > 0 && instructorPhone.Text.Length == 10)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "Instructor phone must be ten digits without dashes.", "Ok");
                instructorPhone.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool IsEmail()
        {
            if (new EmailAddressAttribute().IsValid(instructorEmail.Text))
            {
                return true;
            }
            else
            {
                DisplayAlert("Invalid Email", "Email must be valid.", "Ok");
                instructorEmail.BackgroundColor = Color.Coral;
                return false;
            }
        }

        // Set notifications--------------------------------------------------------------------
        private void CheckedChanged(object sender, CheckedChangedEventArgs e)
        {


            if (courseNotification.IsChecked)
            {
                courseNotify = true;

            }
            else
            {
                courseNotify = false;
            }



            if (performanceNotification.IsChecked)
            {
                perfNotify = true;
            }
            else
            {
                perfNotify = false;
            }

            if (objectiveNotification.IsChecked)
            {
                objNotify = true;
            }
            else
            {
                objNotify = false;
            }


        }

        private bool IsUniqueAssessmentName(string assessmentName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                var assessment = conn.Table<Course>().Where(c => c.CourseId != course.CourseId && (c.ObjectiveName == assessmentName || c.PerformanceName == assessmentName));
                if (assessment.Count() == 0)
                {
                    return true;
                }
                else
                {
                    DisplayAlert("Alert!", "Assessments must have unique names.", "Ok");
                    return false;
                }

            }
        }
    }
}