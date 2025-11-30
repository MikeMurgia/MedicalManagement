using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Library.MedicalManagement.Services;
using Maui.MedicalManagement.ViewModels;

namespace Maui.MedicalManagement
{
    [QueryProperty(nameof(PatientId), "patientId")]
    [QueryProperty(nameof(PatientId), "PatientId")]
    public partial class PatientDetailPage : ContentPage
    {
        private int _patientId;
        private PatientDTO _patient;
        private string _diagnoses;
        private string _prescriptions;

        public int PatientId
        {
            get => _patientId;
            set
            {
                _patientId = value;
                LoadPatient();
            }
        }

        public PatientDTO Patient
        {
            get => _patient;
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }

        public string Diagnoses
        {
            get => _diagnoses;
            set
            {
                _diagnoses = value;
                OnPropertyChanged();
            }
        }

        public string Prescriptions
        {
            get => _prescriptions;
            set
            {
                _prescriptions = value;
                OnPropertyChanged();
            }
        }

        public PatientDetailPage()
        {
            InitializeComponent();
            Patient = new PatientDTO();
            BindingContext = this;
        }

        private void LoadPatient()
        {
            if (_patientId > 0)
            {
                var patient = PatientServiceProxy.Current.patients.FirstOrDefault(p => p?.Id == _patientId);
                if (patient != null)
                {
                    Patient = patient;

                    // Load diagnoses and prescriptions if they exist in a full Patient model
                    // This is a placeholder - adjust based on your actual data model
                    var fullPatient = new Patient(patient);
                    Diagnoses = fullPatient.Diagnoses ?? string.Empty;
                    Prescriptions = fullPatient.Prescriptions ?? string.Empty;

                    BindingContext = this;
                    OnPropertyChanged(nameof(Patient));
                    OnPropertyChanged(nameof(Diagnoses));
                    OnPropertyChanged(nameof(Prescriptions));
                }
            }
            else
            {
                // New patient
                Patient = new PatientDTO
                {
                    Id = 0,
                    Name = string.Empty,
                    Address = string.Empty,
                    Birthdate = DateTime.Today.AddYears(-30), // Default age 30
                    Gender = "Male",
                    Race = "Other"
                };
                Diagnoses = string.Empty;
                Prescriptions = string.Empty;
                BindingContext = this;
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(Patient.Name))
                {
                    await DisplayAlert("Validation", "Name is required", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Patient.Address))
                {
                    await DisplayAlert("Validation", "Address is required", "OK");
                    return;
                }

                // Save the patient
                var savedPatient = await PatientServiceProxy.Current.AddOrUpdate(Patient);

                if (savedPatient != null)
                {
                    await DisplayAlert("Success", "Patient saved successfully", "OK");
                    await Shell.Current.GoToAsync("PatientsPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save patient", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save patient: {ex.Message}", "OK");
            }
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirm", "Are you sure you want to cancel? Any unsaved changes will be lost.", "Yes", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("PatientsPage");
            }
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (_patientId <= 0)
            {
                await DisplayAlert("Info", "Cannot delete a patient that hasn't been saved yet", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {Patient.Name}? This action cannot be undone.",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    PatientServiceProxy.Current.Delete(_patientId);
                    await DisplayAlert("Success", "Patient deleted successfully", "OK");
                    await Shell.Current.GoToAsync("PatientsPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete patient: {ex.Message}", "OK");
                }
            }
        }
    }
}
