using Library.MedicalManagement.Services;
using Maui.MedicalManagement.ViewModels;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    public partial class AppointmentsPage : ContentPage
    {
        public ObservableCollection<AppointmentViewModel> Appointments { get; set; }
        public AppointmentViewModel? SelectedAppointment { get; set; }

        public AppointmentsPage()
        {
            InitializeComponent();
            Appointments = new ObservableCollection<AppointmentViewModel>();
            BindingContext = this;
            LoadAppointments();
        }

        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            Appointments.Clear();

            // Refresh to update patient/physician names
            AppointmentServiceProxy.Current.Refresh();

            var sortedAppointments = AppointmentServiceProxy.Current.Appointments
                .Where(a => a != null)
                .OrderBy(a => a!.StartTime);

            foreach (var appointment in sortedAppointments)
            {
                if (appointment != null)
                {
                    Appointments.Add(new AppointmentViewModel(appointment));
                }
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentDetail?appointmentId=0");
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            var searchTerm = SearchEntry.Text?.Trim().ToUpper() ?? string.Empty;

            Appointments.Clear();

            var filteredAppointments = AppointmentServiceProxy.Current.Appointments
                .Where(a => a != null &&
                    ((a.PatientName?.ToUpper().Contains(searchTerm) ?? false) ||
                     (a.PhysicianName?.ToUpper().Contains(searchTerm) ?? false)))
                .OrderBy(a => a!.StartTime);

            foreach (var appointment in filteredAppointments)
            {
                if (appointment != null)
                {
                    Appointments.Add(new AppointmentViewModel(appointment));
                }
            }
        }

        private void ClearSearchClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadAppointments();
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            LoadAppointments();
        }

        private async void EditItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is AppointmentViewModel appointment)
            {
                var appointmentId = appointment.Model?.Id ?? 0;
                await Shell.Current.GoToAsync($"AppointmentDetail?appointmentId={appointmentId}");
            }
        }

        private async void DeleteItemClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is AppointmentViewModel appointment)
            {
                var confirm = await DisplayAlert("Confirm Cancel",
                    $"Are you sure you want to cancel the appointment on {appointment.Model?.StartTime:MM/dd/yyyy HH:mm}?",
                    "Yes", "No");

                if (confirm)
                {
                    var appointmentId = appointment.Model?.Id ?? 0;
                    if (appointmentId > 0)
                    {
                        AppointmentServiceProxy.Current.Delete(appointmentId);
                        Appointments.Remove(appointment);
                        await DisplayAlert("Success", "Appointment cancelled successfully", "OK");
                    }
                }
            }
        }
    }
}