using CourseTracker.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCoursePage : ContentPage
    {
        public int termId;
        Course course;
        public bool courseNotify = false;
        public bool perfNotify = false;
        public bool objNotify = false;

        public AddCoursePage(int termId)
        {
            InitializeComponent();

            this.termId = termId;
            statusPicker.Title = "Select status";
        }


        // save behavior---------------------------------------------------------------------------------------------------------------
        private void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (IsUniqueAssessmentName(performanceName.Text) && IsUniqueAssessmentName(objectiveName.Text)  && IsCourseEndLater() && IsPerformanceEndLater() && IsObjectiveEndLater() && AreAllEntriesFilled() && IsPhoneNumber() && IsEmail() && IsInsideCourse() && IsInsideTerm())
            {
                course = new Course()
                {
                    TermId = termId,
                    CourseName = courseNameEntry.Text,
                    StartDate = startDatePicker.Date,
                    EndDate = endDatePicker.Date,
                    Status = statusPicker.SelectedItem.ToString(),
                    InstructorName = instructorName.Text,
                    InstructorEmail = instructorEmail.Text,
                    InstructorPhone = instructorPhone.Text,
                    PerformanceName = performanceName.Text,
                    PerformanceStart = performanceStart.Date,
                    PerformanceDue = performanceEnd.Date,
                    ObjectiveName = objectiveName.Text,
                    ObjectiveStart = objectiveStart.Date,
                    ObjectiveDue = objectiveEnd.Date,
                    CourseNotification = courseNotify,
                    PerformanceNotification = perfNotify,
                    ObjectiveNotification = objNotify

                };


                //set notifications----------------------------------------------------------------
                SetObjectiveNotification();
                SetPerformanceNotification();
                SetCourseNotification();

                

               

                //save course----------------------------------------------------------------------------------------------------------------
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Course>();
                    int rows = conn.Insert(course);
                    if (rows > 0)
                    {
                        DisplayAlert("Saved!", "Course successfully saved.", "Ok");
                        Navigation.PopAsync();
                    }
                    else
                    {
                        DisplayAlert("Failed!", "The course failed to save.", "Ok");
                    }
                }
            }
        }

        private void SetCourseNotification()
        {
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
                    int rows = conn.Insert(startNotification);
                    int rows2 = conn.Insert(endNotification);

                    if (rows > 0 && rows2 > 0)
                    {
                        startNotification.SetCourseStartNotification(course.CourseName, course.StartDate.ToString(), course.StartDate);
                        endNotification.SetCourseEndNotification(course.CourseName, course.EndDate.ToString(), course.EndDate);
                    }
                    else { return; }
                }
            }
        }

        private void SetPerformanceNotification()
        {
            //set performance assessment notifications---------------------------------------------------------------
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


        //set notifications--------------------------------------------------------------------------------------------------------------
        private void SetObjectiveNotification()
        {
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

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }


        //Error control to ensure all fields are filled out properly--------------------------------------------------------------------

        private bool IsInsideTerm()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                var terms = conn.Table<Term>().Where(t => t.TermId == termId);
                Term term = terms.FirstOrDefault();
                if(startDatePicker.Date < term.StartDate || endDatePicker.Date > term.EndDate)
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
                if(performanceStart.Date < startDatePicker.Date || performanceEnd.Date > endDatePicker.Date || objectiveStart.Date < startDatePicker.Date || objectiveEnd.Date > endDatePicker.Date)
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
            if (endDatePicker.Date > startDatePicker.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "The course end date must be later than the start date.", "Ok");
                startDatePicker.BackgroundColor = Color.Coral;
                endDatePicker.BackgroundColor = Color.Coral;
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
            if (courseNameEntry.Text != null && instructorName != null && instructorPhone != null && instructorEmail != null && performanceName != null && objectiveName != null && statusPicker.SelectedIndex != -1)
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
            if(new EmailAddressAttribute().IsValid(instructorEmail.Text))
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


        // Set notifications----------------------------------------------------------------------------
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
                var assessment = conn.Table<Course>().Where(c => c.ObjectiveName == assessmentName || c.PerformanceName == assessmentName);
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