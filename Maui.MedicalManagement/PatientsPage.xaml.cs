using Maui.MedicalManagement.ViewModels;
using Library.MedicalManagement.Services;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    public partial class PatientsPage : ContentPage
    {
        private MainViewModel viewModel;

        public PatientsPage()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            BindingContext = viewModel;
        }

        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            viewModel?.Refresh();
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientDetail?patientId=0");
        }

        //use wordpress as model->move a lot of this into viewmodel
        private void SearchClicked(object sender, EventArgs e)
        {
            var searchEntry = this.FindByName<Entry>("SearchEntry");
            if (searchEntry != null)
            {
                viewModel.Query = searchEntry.Text;
                viewModel?.Search();
            }
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            viewModel?.Refresh();
        }

        private async void ExportClicked(object sender, EventArgs e)
        {
            try
            {
                viewModel?.Export();
                await DisplayAlert("Success", "Data exported successfully to C:\\temp\\data.json", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }

        private async void ImportClicked(object sender, EventArgs e)
        {
            try
            {
                viewModel?.Import();
                viewModel?.Refresh();
                await DisplayAlert("Success", "Data imported successfully", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
            }
        }

        private async void EditClicked(object sender, EventArgs e)
        {
            var selectedId = viewModel?.SelectedPatient?.Model?.Id ?? 0;
            if (selectedId > 0)
            {
                await Shell.Current.GoToAsync($"PatientDetail?patientId={selectedId}");
            }
            else
            {
                await DisplayAlert("Info", "Please select a patient to edit", "OK");
            }
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (viewModel?.SelectedPatient == null)
            {
                await DisplayAlert("Error", "Please select a patient to delete", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {viewModel.SelectedPatient.Model?.Name}?",
                "Yes", "No");

            if (confirm)
            {
                viewModel?.Delete();
                viewModel?.Refresh();
            }
        }

        private void InlineEditClicked(object sender, EventArgs e)
        {
            viewModel?.Refresh();
        }

        private async void InlineAddClicked(object sender, EventArgs e)
        {
            try
            {
                if (viewModel != null)
                {
                    bool result = await viewModel.AddInlinePatient();
                    if (result)
                    {
                        await DisplayAlert("Success", "Patient added successfully", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to add patient", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add patient: {ex.Message}", "OK");
            }
        }

        private void ExpandCardClicked(object sender, EventArgs e)
        {
            viewModel?.ExpandCard();
        }
    }
}