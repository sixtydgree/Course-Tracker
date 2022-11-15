using CourseTracker.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermViewPage : ContentPage
    {
        Term selectedTerm;
        public TermViewPage(Term selectedTerm)
        {
            InitializeComponent();

            
                this.selectedTerm = selectedTerm;
            Title = selectedTerm.TermName;
            

        }


        // set item source for the course list view---------------------------------------------------------------------------------
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                selectedTerm = conn.Table<Term>().Where(t => t.TermId == selectedTerm.TermId).FirstOrDefault();
            }

            startDate.Text = "Start Date: " + selectedTerm.StartDate.ToString("M/dd/yyyy");
            endDate.Text = "End Date: " + selectedTerm.EndDate.ToString("M/dd/yyyy");
            status.Text = "Status: " + selectedTerm.Status;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                ObservableCollection<Course> courses = new ObservableCollection<Course>(conn.Table<Course>().ToList());
                courseList.ItemsSource = courses.Where(c => c.TermId == selectedTerm.TermId).ToList();
            }

        }


        //set add course behavior---------------------------------------------------------------------------------------------------------------
        private void addCourseBtn_Clicked(object sender, EventArgs e)
        {
            ObservableCollection<Course> courses;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                courses = new ObservableCollection<Course>(conn.Table<Course>().Where(c => c.TermId == selectedTerm.TermId));

            }

            if (courses.Count == 6)
            {
                DisplayAlert("Course maximum reached.", "Each term may only have six (6) courses.", "Ok");

            }
            else
            {
                Navigation.PushAsync(new AddCoursePage(selectedTerm.TermId));
            }


        }


        private void updateBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditTermPage(selectedTerm.TermId));
        }


        // set delete behavior and ensure intention-----------------------------------------------------------------------------
        private async void deleteBtn_Clicked(object sender, EventArgs e)
        {

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                conn.CreateTable<Course>();
                conn.CreateTable<Notifications>();
                var courses = conn.Table<Course>().ToList();
                List<Course> coursesInTerm = courses.Where(c => c.TermId == selectedTerm.TermId).ToList();

                if (coursesInTerm.Any())
                {
                    var choice = await DisplayAlert("Caution!", "Are you sure you want to delete this term and its courses?", "Yes", "No");
                    if (choice)
                    {
                        
                        int rows = conn.Delete(selectedTerm);
                        int cRows = 0;

                        foreach (Course c in coursesInTerm)
                        {
                            var notifications = conn.Table<Notifications>().Where(n => n.CourseId == c.CourseId || n.PerformanceName == c.PerformanceName || n.ObjectiveName == c.ObjectiveName);
                            if (notifications.Any())
                            {
                                foreach(Notifications n in notifications)
                                {
                                    n.CancelNotification(n.NotificationId);
                                }
                            }
                            cRows++;
                            conn.Delete(c);
                        }

                        if (rows > 0 && cRows > 0)
                        {
                            await DisplayAlert("Deleted!", "This term and its courses have been deleted.", "Ok");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure!", "The term and its courses could not be deleted.", "Ok");
                        }
                    }
                }
                else
                {
                    var choice2 = await DisplayAlert("Caution!", "Are you sure you want to delete this term?", "Yes", "No");
                    if (choice2)
                    {
                        int rows = conn.Delete(selectedTerm);

                        if (rows > 0)
                        {
                            await DisplayAlert("Deleted!", "Term successfully deleted.", "Ok");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Failure!", "The term could not be deleted.", "Ok");
                        }
                    }

                }



            }


        }


        //set behavior for selected courses---------------------------------------------------------------------------------------
        private void courseList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedCourse = courseList.SelectedItem as Course;

            if (selectedCourse != null)
            {
                Navigation.PushAsync(new CourseViewPage(selectedCourse));
            }
        }
    }
}