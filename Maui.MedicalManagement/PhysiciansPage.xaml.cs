using Library.MedicalManagement.Services;
using Library.MedicalManagement.DTO;
using Maui.MedicalManagement.ViewModels;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    public partial class PhysiciansPage : ContentPage
    {
        public ObservableCollection<PhysicianViewModel> Physicians { get; set; }
        public PhysicianViewModel SelectedPhysician { get; set; }

        public PhysiciansPage()
        {
            InitializeComponent();
            Physicians = new ObservableCollection<PhysicianViewModel>();
            BindingContext = this;
            LoadPhysicians();
        }

        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            LoadPhysicians();
        }

        private void LoadPhysicians()
        {
            try
            {
                Physicians.Clear();

                //PhysicianServiceProxy.Current.Refresh();

                var physicians = PhysicianServiceProxy.Current.Physicians;
                foreach (var physician in physicians)
                {
                    if (physician != null)
                    {
                        Physicians.Add(new PhysicianViewModel(physician));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading physicians: {ex.Message}");
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PhysicianDetail?physicianId=0");
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            LoadPhysicians();
        }

        private async void EditClicked(object sender, EventArgs e)
        {
            if (SelectedPhysician == null)
            {
                await DisplayAlert("Info", "Please select a physician to edit", "OK");
                return;
            }

            var selectedId = SelectedPhysician?.Model?.Id ?? 0;
            await Shell.Current.GoToAsync($"PhysicianDetail?physicianId={selectedId}");
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (SelectedPhysician == null)
            {
                await DisplayAlert("Info", "Please select a physician to delete", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete Dr. {SelectedPhysician.Model?.Name}?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    PhysicianServiceProxy.Current.Delete(SelectedPhysician.Model?.Id ?? 0);
                    LoadPhysicians();
                    await DisplayAlert("Success", "Physician deleted successfully", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete physician: {ex.Message}", "OK");
                }
            }
        }

        private async void SearchClicked(object sender, EventArgs e)
        {
            var specialization = await DisplayPromptAsync("Search",
                "Enter specialization to search for:",
                placeholder: "e.g., Cardiology");

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                try
                {
                    var searchResults = await PhysicianServiceProxy.Current.Search(
                        new Library.MedicalManagement.Data.QueryRequest { Content = specialization });

                    Physicians.Clear();
                    foreach (var physician in searchResults)
                    {
                        if (physician != null)
                        {
                            Physicians.Add(new PhysicianViewModel(physician));
                        }
                    }

                    if (!Physicians.Any())
                    {
                        await DisplayAlert("Search Results", "No physicians found with that specialization", "OK");
                        LoadPhysicians();
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
                }
            }
        }

        private async void ViewScheduleClicked(object sender, EventArgs e)
        {
            if (SelectedPhysician == null)
            {
                await DisplayAlert("Info", "Please select a physician to view schedule", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"///AppointmentsPage?physicianId={SelectedPhysician.Model?.Id}");
        }
    }
}