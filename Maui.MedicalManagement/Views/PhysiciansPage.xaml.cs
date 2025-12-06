using Library.MedicalManagement.Services;
using Library.MedicalManagement.DTO;
using Maui.MedicalManagement.ViewModels;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    public partial class PhysiciansPage : ContentPage
    {
        public ObservableCollection<PhysicianViewModel> Physicians { get; set; }
        public PhysicianViewModel? SelectedPhysician { get; set; }

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
            Physicians.Clear();

            foreach (var physician in PhysicianServiceProxy.Current.Physicians)
            {
                if (physician != null)
                {
                    Physicians.Add(new PhysicianViewModel(physician));
                }
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PhysicianDetail?physicianId=0");
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            var searchTerm = SearchEntry.Text?.Trim().ToUpper() ?? string.Empty;

            Physicians.Clear();

            var filteredPhysicians = PhysicianServiceProxy.Current.Physicians
                .Where(p => p != null &&
                    ((p.Name?.ToUpper().Contains(searchTerm) ?? false) ||
                     (p.Specializations?.ToUpper().Contains(searchTerm) ?? false) ||
                     (p.License?.ToUpper().Contains(searchTerm) ?? false)));

            foreach (var physician in filteredPhysicians)
            {
                if (physician != null)
                {
                    Physicians.Add(new PhysicianViewModel(physician));
                }
            }
        }

        private void ClearSearchClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadPhysicians();
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadPhysicians();
        }

        private async void EditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is PhysicianViewModel physician)
            {
                var physicianId = physician.Model?.Id ?? 0;
                await Shell.Current.GoToAsync($"PhysicianDetail?physicianId={physicianId}");
            }
        }

        private async void DeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is PhysicianViewModel physician)
            {
                var confirm = await DisplayAlert("Confirm Delete",
                    $"Are you sure you want to delete Dr. {physician.Model?.Name}?",
                    "Yes", "No");

                if (confirm)
                {
                    var physicianId = physician.Model?.Id ?? 0;
                    if (physicianId > 0)
                    {
                        PhysicianServiceProxy.Current.Delete(physicianId);
                        Physicians.Remove(physician);
                        await DisplayAlert("Success", "Physician deleted successfully", "OK");
                    }
                }
            }
        }
    }
}