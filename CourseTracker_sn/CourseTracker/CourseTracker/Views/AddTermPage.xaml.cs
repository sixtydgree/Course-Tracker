using CourseTracker.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CourseTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTermPage : ContentPage
    {
        public AddTermPage()
        {
            InitializeComponent();

            statusPicker.SelectedIndex = 0;
        }

        private void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (IsTermNameNull() && IsEndDateGreater())
            {
                Term term = new Term()
                {
                    TermName = termTitle.Text,
                    StartDate = startDate.Date,
                    EndDate = endDate.Date,
                    Status = statusPicker.SelectedItem.ToString()

                };

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Term>();
                    int rows = conn.Insert(term);
                    if (rows > 0)
                    {
                        DisplayAlert("Saved!", "Term saved successfully.", "Ok");
                        Navigation.PopAsync();
                    }
                    else
                    {
                        DisplayAlert("Failed!", "Something went wrong.", "Ok");
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
            if (startDate.Date < endDate.Date)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "End date must occur after Start date.", "Ok");
                startDate.BackgroundColor = Color.Coral;
                endDate.BackgroundColor = Color.Coral;
                return false;
            }

        }

        public bool IsTermNameNull()
        {
            if (termTitle.Text != null)
            {
                return true;
            }
            else
            {
                DisplayAlert("Alert!", "Term must have a name.", "Ok");
                termTitle.BackgroundColor = Color.Coral;
                return false;
            }
        }
    }
}