using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CourseTracker.Models;
using System.Collections.ObjectModel;
using CourseTracker.Views;

namespace CourseTracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Term>();
                ObservableCollection<Term> terms = new ObservableCollection<Term>(conn.Table<Term>().ToList());
                termListView.ItemsSource = terms;

            }

        }

        private void addBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddTermPage());

        }

        private void termListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedTerm = termListView.SelectedItem as Term;

            if (selectedTerm != null)
            {
                Navigation.PushAsync(new TermViewPage(selectedTerm));
            }
        }
    }
}
