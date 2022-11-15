using CourseTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Collections.ObjectModel;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTermPage : ContentPage
    {
        int termId;
        Term termToEdit;
        public EditTermPage(int termId)
        {
            InitializeComponent();

            this.termId = termId;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                ObservableCollection<Term> terms = new ObservableCollection<Term>(conn.Table<Term>().Where(t => t.TermId == this.termId).ToList());
                termToEdit = terms.First();

            }

            termNameEntry.Text = termToEdit.TermName;
            startDatePicker.Date = termToEdit.StartDate;
            endDatePicker.Date = termToEdit.EndDate;
            statusPicker.SelectedItem = termToEdit.Status;

        }

        private void saveBtn_Clicked(object sender, EventArgs e)
        {


            if (IsTermNameNull() && IsEndDateGreater() && StatusPicked())
            {
                termToEdit.TermName = termNameEntry.Text;
                termToEdit.StartDate = startDatePicker.Date;
                termToEdit.EndDate = endDatePicker.Date;
                termToEdit.Status = statusPicker.SelectedItem.ToString();


                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Term>();
                    int rows = conn.Update(termToEdit);

                    if (rows > 0)
                    {
                        DisplayAlert("Updated!", "The term was successfully updated.", "Ok");
                        Navigation.PopAsync();
                    }
                    else
                    {
                        DisplayAlert("Failed!", "The term was unable to update.", "Ok");
                    }
                }
            }


        }

        private void cancelBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public bool IsEndDateGreater()
        {
            if (startDatePicker.Date < endDatePicker.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "End date must occur after Start date.", "Ok");
                startDatePicker.BackgroundColor = Color.Coral;
                endDatePicker.BackgroundColor = Color.Coral;
                return false;
            }

        }

        public bool IsTermNameNull()
        {
            if (termNameEntry.Text != null)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "Term must have a name.", "Ok");
                termNameEntry.BackgroundColor = Color.Coral;
                return false;
            }
        }

        private bool StatusPicked()
        {
            if(statusPicker.SelectedIndex == -1)
            {
                return false;
            }
            else { return true; }
        }

        private void addCourseBtn_Clicked(object sender, EventArgs e)
        {
            ObservableCollection<Course> courses;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Course>();
                courses = new ObservableCollection<Course>(conn.Table<Course>().Where(c => c.TermId == this.termId));

            }

            if (courses.Count == 6)
            {
                DisplayAlert("Course maximum reached.", "Each term may only have six (6) courses.", "Ok");

            }
            else
            {
                Navigation.PushAsync(new AddCoursePage(this.termId));
            }

        }
    }
}