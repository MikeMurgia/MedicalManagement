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
        private bool _isLoaded = false;
        //private string _diagnoses;
        //private string _prescriptions;

        public int PatientId
        {
            get => _patientId;
            set
            {
                _patientId = value;
                if (_isLoaded)
                {
                    LoadPatient();
                }
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

        /*public string Diagnoses
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
        }*/

        public PatientDetailPage()
        {
            InitializeComponent();
            Patient = new PatientDTO();
            BindingContext = this;
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            _isLoaded = true;
            LoadPatient();
        }
        private void LoadPatient()
        {
            try
            {
                if (_patientId > 0)
                {
                    // Editing existing patient
                    var patient = PatientServiceProxy.Current.patients.FirstOrDefault(p => p?.Id == _patientId);
                    if (patient != null)
                    {
                        NameEntry.Text = patient.Name ?? string.Empty;
                        AddressEntry.Text = patient.Address ?? string.Empty;
                        BirthdatePicker.Date = patient.Birthdate;
                        GenderPicker.SelectedItem = patient.Gender ?? "Male";
                        RaceEntry.Text = patient.Race ?? string.Empty;
                        DiagnosesEditor.Text = patient.Diagnoses ?? string.Empty;
                        PrescriptionsEditor.Text = patient.Prescriptions ?? string.Empty;

                        System.Diagnostics.Debug.WriteLine($"Loaded patient: {patient.Name}");
                        System.Diagnostics.Debug.WriteLine($"  Diagnoses: {patient.Diagnoses}");
                        System.Diagnostics.Debug.WriteLine($"  Prescriptions: {patient.Prescriptions}");
                    }
                }
                else
                {
                    // New patient
                    NameEntry.Text = string.Empty;
                    AddressEntry.Text = string.Empty;
                    BirthdatePicker.Date = DateTime.Today.AddYears(-18);
                    GenderPicker.SelectedIndex = 0;
                    RaceEntry.Text = string.Empty;
                    DiagnosesEditor.Text = string.Empty;
                    PrescriptionsEditor.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading patient: {ex.Message}");
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameEntry.Text))
                {
                    await DisplayAlert("Validation", "Name is required", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(AddressEntry.Text))
                {
                    await DisplayAlert("Validation", "Address is required", "OK");
                    return;
                }

                var patientDto = new PatientDTO
                {
                    Id = _patientId,
                    Name = NameEntry.Text?.Trim(),
                    Address = AddressEntry.Text?.Trim(),
                    Birthdate = BirthdatePicker.Date,
                    Gender = GenderPicker.SelectedItem?.ToString() ?? "Male",
                    Race = RaceEntry.Text?.Trim() ?? string.Empty,
                    Diagnoses = DiagnosesEditor.Text?.Trim() ?? string.Empty,
                    Prescriptions = PrescriptionsEditor.Text?.Trim() ?? string.Empty
                };


                var savedPatient = await PatientServiceProxy.Current.AddOrUpdate(patientDto);

                if (savedPatient != null)
                {
                    await DisplayAlert("Success", "Patient saved successfully", "OK");
                    await Shell.Current.GoToAsync("///PatientsPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save patient - no response from server", "OK");
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
                await Shell.Current.GoToAsync("///PatientsPage");
            }
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (_patientId <= 0)
            {
                await DisplayAlert("Info", "Cannot delete a patient that hasn't been saved yet", "OK");
                return;
            }

            PatientServiceProxy.Current.Delete(_patientId);
            await DisplayAlert("Success", "Patient deleted successfully", "OK");
            await Shell.Current.GoToAsync("///PatientsPage");

        }
    }
}