using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Collections.ObjectModel;
using CourseTracker.Models;

namespace CourseTracker
{
    public partial class App : Application
    {

        public static string DatabaseLocation = string.Empty;
        public App()
        {
            InitializeComponent();

            

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
            DatabaseLocation = databaseLocation;

            //the following code is for evaluation purposes. if deleted the app starts fresh and without any terms or courses added.
            ObservableCollection<Term> terms;
            using (SQLiteConnection conn = new SQLiteConnection(DatabaseLocation))
            {
                conn.CreateTable<Term>();
                terms = new ObservableCollection<Term>(conn.Table<Term>());

                if(terms.Count == 0)
                {
                    Term term = new Term()
                    {
                        TermName = "Term 1",
                        StartDate = DateTime.Parse("7/1/2022 12:00:00 AM"),
                        EndDate = DateTime.Parse("12/31/2022 11:59:59 PM"),
                        Status = "Started"
                    };
                    
                    conn.Insert(term);

                    Course course = new Course()
                    {
                        TermId = term.TermId,
                        CourseName = "C971",
                        StartDate = DateTime.Parse("7/1/2022 12:00:00 AM"),
                        EndDate = DateTime.Parse("7/30/2022 12:00:00 AM"),
                        Status = "Not Started",
                        InstructorName = "Paul Johnson",
                        InstructorPhone = "7022838174",
                        InstructorEmail = "pjoh308@wgu.edu",
                        PerformanceName = "Mobile Application Build",
                        PerformanceStart = DateTime.Parse("7/1/2022 12:00:00 AM"),
                        PerformanceDue = DateTime.Parse("7/10/2022 12:00:00 AM"),
                        ObjectiveName = "Mobile Application Test?",
                        ObjectiveStart = DateTime.Parse("7/11/2022 12:00:00 AM"),
                        ObjectiveDue = DateTime.Parse("7/30/2022 12:00:00 AM"),
                        CourseNotification = false,
                        PerformanceNotification = false,
                        ObjectiveNotification = false
                    };
                    Course course2 = new Course()
                    {
                        TermId = term.TermId,
                        CourseName = "Update me!",
                        StartDate = DateTime.Parse("8/1/2022 12:00:00 AM"),
                        EndDate = DateTime.Parse("8/31/2022 12:00:00 AM"),
                        Status = "Not Started",
                        InstructorName = "This would be a name.",
                        InstructorEmail = "email@email.com",
                        InstructorPhone = "5555555555",
                        PerformanceName = "Performance Assessment",
                        PerformanceStart = DateTime.Parse("8/1/2022 12:00:00 AM"),
                        PerformanceDue = DateTime.Parse("8/10/2022 12:00:00 AM"),
                        ObjectiveName = "Objective Assessment",
                        ObjectiveStart = DateTime.Parse("8/11/2022 12:00:00 AM"),
                        ObjectiveDue = DateTime.Parse("8/31/2022 12:00:00 AM"),
                        CourseNotification = false,
                        PerformanceNotification = false,
                        ObjectiveNotification = false
                    };

                    conn.CreateTable<Course>();
                    conn.Insert(course);
                    conn.Insert(course2);
                }
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
