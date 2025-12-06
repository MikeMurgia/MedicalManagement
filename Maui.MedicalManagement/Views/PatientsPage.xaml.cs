using Maui.MedicalManagement.ViewModels;
using Library.MedicalManagement.Services;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    public partial class PatientsPage : ContentPage
    {
        public ObservableCollection<PatientViewModel> Patients { get; set; }
        public PatientViewModel? SelectedPatient { get; set; }

        public PatientsPage()
        {
            InitializeComponent();
            Patients = new ObservableCollection<PatientViewModel>();
            BindingContext = this;
            LoadPatients();
        }

        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            LoadPatients();
        }

        private void LoadPatients()
        {
            try
            {
                Patients.Clear();

                PatientServiceProxy.Current.Refresh();

                foreach (var patient in PatientServiceProxy.Current.patients)
                {
                    if (patient != null)
                    {
                        Patients.Add(new PatientViewModel(patient));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading patients: {ex.Message}");
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientDetail?patientId=0");
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            var searchTerm = SearchEntry.Text?.Trim().ToUpper() ?? string.Empty;

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadPatients();
                return;
            }

            Patients.Clear();

            var filteredPatients = PatientServiceProxy.Current.patients
                .Where(p => p != null &&
                    ((p.Name?.ToUpper().Contains(searchTerm) ?? false) ||
                     (p.Address?.ToUpper().Contains(searchTerm) ?? false)));

            foreach (var patient in filteredPatients)
            {
                if (patient != null)
                {
                    Patients.Add(new PatientViewModel(patient));
                }
            }
        }

        private void ClearSearchClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadPatients();
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadPatients();
        }

        private async void EditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is PatientViewModel patient)
            {
                var patientId = patient.Model?.Id ?? 0;
                await Shell.Current.GoToAsync($"PatientDetail?patientId={patientId}");
            }
        }

        private async void DeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is PatientViewModel patient)
            {
                var confirm = await DisplayAlert("Confirm Delete",
                    $"Are you sure you want to delete {patient.Model?.Name}?",
                    "Yes", "No");

                if (confirm)
                {
                    try
                    {
                        var patientId = patient.Model?.Id ?? 0;
                        if (patientId > 0)
                        {
                            PatientServiceProxy.Current.Delete(patientId);
                            Patients.Remove(patient);
                            await DisplayAlert("Success", "Patient deleted successfully", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete patient: {ex.Message}", "OK");
                    }
                }
            }
        }
    }
}
