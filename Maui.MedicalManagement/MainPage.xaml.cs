using Maui.MedicalManagement.ViewModels;
using Library.MedicalManagement.Services;

namespace Maui.MedicalManagement
{
    public partial class MainPage : ContentPage
    {
        private MainDashboardViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainDashboardViewModel();
            BindingContext = viewModel;
        }

        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            viewModel?.Refresh();
        }

        // Quick Action Button Handlers
        private async void AddPatientClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Patient?PatientId=0");
        }

        private async void ScheduleAppointmentClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentDetail?appointmentId=0");
        }

        private async void ManagePatientsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientsPage");
        }

        private async void ManagePhysiciansClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PhysiciansPage");
        }

        private async void ViewAppointmentsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentsPage");
        }

        private async void AddPhysicianClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PhysicianDetail?physicianId=0");
        }

        private async void ViewPatientsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientsPage");
        }

        private async void ViewTodaysScheduleClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentsPage");
        }

        private async void ViewAllPatientsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientsPage");
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            viewModel?.Refresh();
        }

        private async void ViewReportsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reports", "Reports feature coming soon!", "OK");
        }

        // Navigation Card Tap Handlers
        private async void PatientCardTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PatientsPage");
        }

        private async void PhysicianCardTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("PhysiciansPage");
        }

        private async void AppointmentCardTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentsPage");
        }

        private async void ImportExportCardTapped(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Import/Export", "Cancel", null, "Import Patients", "Export Patients");

            if (action == "Import Patients")
            {
                try
                {
                    viewModel?.ImportPatients();
                    await DisplayAlert("Success", "Patients imported successfully", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
                }
            }
            else if (action == "Export Patients")
            {
                try
                {
                    viewModel?.ExportPatients();
                    await DisplayAlert("Success", "Patients exported to C:\\temp\\data.json", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
                }
            }
        }

        private async void ReportsCardTapped(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Reports", "Cancel", null,
                "Patient Statistics",
                "Physician Workload",
                "Appointment Summary");

            if (action == "Patient Statistics")
            {
                await DisplayAlert("Patient Statistics",
                    $"Total Patients: {viewModel?.TotalPatients}\n" +
                    $"Recent Additions: {viewModel?.RecentPatients.Count}",
                    "OK");
            }
            else if (action == "Physician Workload")
            {
                await DisplayAlert("Physician Workload",
                    $"Total Physicians: {viewModel?.TotalPhysicians}\n" +
                    $"Today's Appointments: {viewModel?.TodaysAppointments}",
                    "OK");
            }
            else if (action == "Appointment Summary")
            {
                await DisplayAlert("Appointment Summary",
                    $"Today's Appointments: {viewModel?.TodaysAppointments}\n" +
                    $"Business Hours: Mon-Fri, 8 AM - 5 PM",
                    "OK");
            }
        }

        private async void SettingsCardTapped(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Settings", "Cancel", null,
                "API Configuration",
                "Business Hours",
                "About");

            if (action == "API Configuration")
            {
                await DisplayAlert("API Configuration",
                    "API Endpoint: https://localhost:7009\n" +
                    "Status: Connected",
                    "OK");
            }
            else if (action == "Business Hours")
            {
                await DisplayAlert("Business Hours",
                    "Monday - Friday\n" +
                    "8:00 AM - 5:00 PM\n" +
                    "Appointments: 30-minute slots",
                    "OK");
            }
            else if (action == "About")
            {
                await DisplayAlert("About",
                    "Medical Management System\n" +
                    "Version 1.0.0\n" +
                    "© 2024 Medical Management",
                    "OK");
            }
        }
    }
}
