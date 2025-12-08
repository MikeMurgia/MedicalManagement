using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Services;

namespace Maui.MedicalManagement
{
    [QueryProperty(nameof(PhysicianId), "physicianId")]
    public partial class PhysicianDetailPage : ContentPage
    {
        private int _physicianId;
        private PhysicianDTO _physician;

        public int PhysicianId
        {
            get => _physicianId;
            set
            {
                _physicianId = value;
                LoadPhysician();
            }
        }

        public PhysicianDTO Physician
        {
            get => _physician;
            set
            {
                _physician = value;
                OnPropertyChanged();
            }
        }

        public PhysicianDetailPage()
        {
            InitializeComponent();
            Physician = new PhysicianDTO();
            BindingContext = this;
        }

        private void LoadPhysician()
        {
            if (_physicianId > 0)
            {
                var physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p?.Id == _physicianId);
                if (physician != null)
                {
                    Physician = physician;
                    BindingContext = this;
                    OnPropertyChanged(nameof(Physician));
                }
            }
            else
            {
                Physician = new PhysicianDTO
                {
                    Id = 0,
                    Name = string.Empty,
                    License = string.Empty,
                    GraduationDate = DateTime.Today.AddYears(-4),
                    Specializations = string.Empty
                };
                BindingContext = this;
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Physician.Name))
                {
                    await DisplayAlert("Validation", "Name is required", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Physician.License))
                {
                    await DisplayAlert("Validation", "License number is required", "OK");
                    return;
                }

                var savedPhysician = await PhysicianServiceProxy.Current.AddOrUpdate(Physician);

                if (savedPhysician != null)
                {
                    await DisplayAlert("Success", "Physician saved successfully", "OK");
                    await Shell.Current.GoToAsync("///PhysiciansPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to save physician", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save physician: {ex.Message}", "OK");
            }
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirm", "Are you sure you want to cancel? Any unsaved changes will be lost.", "Yes", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("///PhysiciansPage");
            }
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (_physicianId <= 0)
            {
                await DisplayAlert("Info", "Cannot delete a physician that hasn't been saved yet", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {Physician.Name}? This action cannot be undone.",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    PhysicianServiceProxy.Current.Delete(_physicianId);
                    await DisplayAlert("Success", "Physician deleted successfully", "OK");
                    await Shell.Current.GoToAsync("///PhysiciansPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete physician: {ex.Message}", "OK");
                }
            }
        }

        private async void AddSpecializationClicked(object sender, EventArgs e)
        {
            var specialization = await DisplayPromptAsync("Add Specialization",
                "Enter specialization:",
                placeholder: "e.g., Cardiology");

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                if (string.IsNullOrWhiteSpace(Physician.Specializations))
                {
                    Physician.Specializations = specialization;
                }
                else
                {
                    Physician.Specializations += ", " + specialization;
                }

                OnPropertyChanged(nameof(Physician));
            }
        }

        private void ClearSpecializationsClicked(object sender, EventArgs e)
        {
            Physician.Specializations = string.Empty;
            OnPropertyChanged(nameof(Physician));
        }
    }
}