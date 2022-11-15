using CourseTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Xamarin.Essentials;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseViewPage : ContentPage
    {
        Course selectedCourse;

        public bool courseNotify;
        public bool perfNotify;
        public bool objNotify;
        public CourseViewPage(Course selectedCourse)
        {
            InitializeComponent();

            this.selectedCourse = selectedCourse;


            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using(SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                selectedCourse = conn.Table<Course>().Where(c => c.CourseId == selectedCourse.CourseId).FirstOrDefault();
            }

            //Set values for views---------------------------------------------------------------------------------------
            Title = selectedCourse.CourseName;
            startDate.Text = selectedCourse.StartDate.ToString("M/dd/yyyy");
            endDate.Text = selectedCourse.EndDate.ToString("M/dd/yyyy");
            status.Text = selectedCourse.Status;
            notes.Text = selectedCourse.CourseNotes;
            instructorName.Text = selectedCourse.InstructorName;
            instructorEmail.Text = selectedCourse.InstructorEmail;
            instructorPhone.Text = selectedCourse.InstructorPhone;
            objectiveName.Text = selectedCourse.ObjectiveName;
            objectiveStart.Text = selectedCourse.ObjectiveStart.ToString("M/dd/yyyy");
            objectiveEnd.Text = selectedCourse.ObjectiveDue.ToString("M/dd/yyyy");
            performanceName.Text = selectedCourse.PerformanceName;
            performanceStart.Text = selectedCourse.PerformanceStart.ToString("M/dd/yyyy");
            performanceEnd.Text = selectedCourse.PerformanceDue.ToString("M/dd/yyyy");
            courseNotification.IsChecked = selectedCourse.CourseNotification;
            performanceNotification.IsChecked = selectedCourse.PerformanceNotification;
            objectiveNotification.IsChecked = selectedCourse.ObjectiveNotification;
            courseNotify = selectedCourse.CourseNotification;
            perfNotify = selectedCourse.PerformanceNotification;
            objNotify = selectedCourse.ObjectiveNotification;
        }

        //OnDisappearing to automatically save notes and notification preferance-------------------------------------------------------
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            selectedCourse.CourseNotes = notes.Text;
            selectedCourse.CourseNotification = courseNotify;
            selectedCourse.ObjectiveNotification = objNotify;
            selectedCourse.PerformanceNotification = perfNotify;

            SetCourseNotifications();
            SetPerformanceNotifications();
            SetObjectiveNotifications();


            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                conn.Update(selectedCourse);

            }
        }

        //Set notification behavior---------------------------------------------------------------------------------------
        private void SetObjectiveNotifications()
        {
            Notifications startNotification = new Notifications()
            {
                ObjectiveName = selectedCourse.ObjectiveName
            };
            Notifications endNotification = new Notifications()
            {
                ObjectiveName = selectedCourse.ObjectiveName
            };

            if (objNotify)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var objN = conn.Table<Notifications>().Where(n => n.ObjectiveName == selectedCourse.ObjectiveName);
                    if(objN.Count() == 0)
                    {
                        int rows = conn.Insert(startNotification);
                        int rows2 = conn.Insert(endNotification);

                        if (rows > 0 && rows2 > 0)
                        {
                            startNotification.SetAssessmentStartNotification(selectedCourse.ObjectiveName, selectedCourse.ObjectiveStart.ToString(), selectedCourse.ObjectiveStart);
                            endNotification.SetAssessmentEndNotification(selectedCourse.ObjectiveName, selectedCourse.ObjectiveDue.ToString(), selectedCourse.ObjectiveDue);
                        }
                    }
                    

                }
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var startN = conn.Table<Notifications>().Where(c => c.ObjectiveName == selectedCourse.ObjectiveName).ToList();

                    if (startN.Any())
                    {
                        foreach (Notifications n in startN)
                        {

                            startNotification.CancelNotification(n.NotificationId);
                            endNotification.CancelNotification(n.NotificationId);

                            conn.Delete(n);
                        }
                    }
                }
            }
        }

        private void SetPerformanceNotifications()
        {
            Notifications startNotification = new Notifications()
            {
                PerformanceName = selectedCourse.PerformanceName
            };
            Notifications endNotification = new Notifications()
            {
                PerformanceName = selectedCourse.PerformanceName
            };

            if (perfNotify)
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var perfN = conn.Table<Notifications>().Where(n => n.PerformanceName == selectedCourse.PerformanceName);
                    if(perfN.Count() == 0)
                    {
                        int rows = conn.Insert(startNotification);
                        int rows2 = conn.Insert(endNotification);

                        if (rows > 0 && rows2 > 0)
                        {
                            startNotification.SetAssessmentStartNotification(selectedCourse.PerformanceName, selectedCourse.PerformanceStart.ToString(), selectedCourse.PerformanceStart);
                            endNotification.SetAssessmentEndNotification(selectedCourse.PerformanceName, selectedCourse.PerformanceDue.ToString(), selectedCourse.PerformanceDue);
                        }
                    }
                    

                }
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var startN = conn.Table<Notifications>().Where(c => c.PerformanceName == selectedCourse.PerformanceName).ToList();

                    if (startN.Any())
                    {
                        foreach (Notifications n in startN)
                        {

                            startNotification.CancelNotification(n.NotificationId);
                            endNotification.CancelNotification(n.NotificationId);

                            conn.Delete(n);
                        }
                    }
                }
            }
        }

        private void SetCourseNotifications()
        {
            Notifications startNotification = new Notifications()
            {
                CourseId = selectedCourse.CourseId
            };
            Notifications endNotification = new Notifications()
            {
                CourseId = selectedCourse.CourseId
            };

            if (courseNotify)
            {
                
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var courseN = conn.Table<Notifications>().Where(n => n.CourseId == selectedCourse.CourseId);

                    if(courseN.Count() == 0)
                    {
                        int rows = conn.Insert(startNotification);
                        int rows2 = conn.Insert(endNotification);

                        if (rows > 0 && rows2 > 0)
                        {
                            startNotification.SetCourseStartNotification(selectedCourse.CourseName, selectedCourse.StartDate.ToString(), selectedCourse.StartDate);
                            endNotification.SetCourseEndNotification(selectedCourse.CourseName, selectedCourse.EndDate.ToString(), selectedCourse.EndDate);
                        }
                    }
                    
                }
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var startN = conn.Table<Notifications>().Where(c => c.CourseId == selectedCourse.CourseId).ToList();

                    if (startN.Any())
                    {
                        foreach (Notifications n in startN)
                        {
                           
                            startNotification.CancelNotification(n.NotificationId);
                            endNotification.CancelNotification(n.NotificationId);

                            conn.Delete(n);
                        }
                    }
                    

                }

            }
        }

        private void updateBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditCoursePage(selectedCourse.CourseId));
        }


        //Set delete behavior and ensure intention___________________________________________________________________\\
        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Delete?", "Are you sure you would like to delete this course?", "Yes", "No"))
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Notifications>();
                    var notifications = conn.Table<Notifications>().Where(n => n.CourseId == selectedCourse.CourseId || n.PerformanceName == selectedCourse.PerformanceName || n.ObjectiveName == selectedCourse.ObjectiveName);
                    if (notifications.Any())
                    {
                        foreach (Notifications n in notifications)
                        {
                            n.CancelNotification(n.NotificationId);
                        }
                    }
                }

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Course>();
                    int rows = conn.Delete(selectedCourse);

                    if (rows > 0)
                    {
                        await DisplayAlert("Deleted!", "The course was successfully deleted.", "Ok");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Failure!", "The course did not delete.", "Ok");
                    }
                }
            }


        }


        //set notification bools------------------------------------------------------------------------------------------------------
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


        // Create share behavior-----------------------------------------------------------------------------
        private async void shareNotesBtn_Clicked(object sender, EventArgs e)
        {
           await ShareNotes();
        }

        private async Task ShareNotes()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = notes.Text,
                Title = $"Notes for {selectedCourse.CourseName}"
            });
        }
    }
}