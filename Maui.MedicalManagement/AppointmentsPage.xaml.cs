using Library.MedicalManagement.Services;
using Library.MedicalManagement.DTO;
using Maui.MedicalManagement.ViewModels;
using System.Collections.ObjectModel;

namespace Maui.MedicalManagement
{
    [QueryProperty(nameof(PhysicianIdFilter), "physicianId")]
    public partial class AppointmentsPage : ContentPage
    {
        public ObservableCollection<AppointmentViewModel> Appointments { get; set; }
        public AppointmentViewModel SelectedAppointment { get; set; }

        private int _physicianIdFilter = 0;
        public int PhysicianIdFilter
        {
            get => _physicianIdFilter;
            set
            {
                _physicianIdFilter = value;
                LoadAppointments();
            }
        }

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
            try
            {
                Appointments.Clear();

                AppointmentServiceProxy.Current.Refresh();

                var appointments = AppointmentServiceProxy.Current.Appointments;

                if (_physicianIdFilter > 0)
                {
                    appointments = appointments.Where(a => a?.PhysicianId == _physicianIdFilter).ToList();
                }

                foreach (var appointment in appointments)
                {
                    if (appointment != null)
                    {
                        Appointments.Add(new AppointmentViewModel(appointment));
                    }
                }

                var sorted = Appointments.OrderBy(a => a.Model?.StartTime).ToList();
                Appointments.Clear();
                foreach (var item in sorted)
                {
                    Appointments.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading appointments: {ex.Message}");
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("AppointmentDetail?appointmentId=0");
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            LoadAppointments();
        }

        private async void EditClicked(object sender, EventArgs e)
        {
            if (SelectedAppointment == null)
            {
                await DisplayAlert("Info", "Please select an appointment to edit", "OK");
                return;
            }

            var selectedId = SelectedAppointment?.Model?.Id ?? 0;
            await Shell.Current.GoToAsync($"AppointmentDetail?appointmentId={selectedId}");
        }

        private async void CancelAppointmentClicked(object sender, EventArgs e)
        {
            if (SelectedAppointment == null)
            {
                await DisplayAlert("Info", "Please select an appointment to cancel", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Cancel",
                $"Are you sure you want to cancel the appointment on {SelectedAppointment.Model?.StartTime:MM/dd/yyyy HH:mm}?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    AppointmentServiceProxy.Current.Delete(SelectedAppointment.Model?.Id ?? 0);
                    LoadAppointments();
                    await DisplayAlert("Success", "Appointment cancelled successfully", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to cancel appointment: {ex.Message}", "OK");
                }
            }
        }

        private async void ViewTodayClicked(object sender, EventArgs e)
        {
            try
            {
                Appointments.Clear();

                var today = DateTime.Today;
                var appointments = AppointmentServiceProxy.Current.Appointments
                    .Where(a => a != null && a.StartTime?.Date == today);

                foreach (var appointment in appointments)
                {
                    if (appointment != null)
                    {
                        Appointments.Add(new AppointmentViewModel(appointment));
                    }
                }

                if (!Appointments.Any())
                {
                    await DisplayAlert("Today's Appointments", "No appointments scheduled for today", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to filter appointments: {ex.Message}", "OK");
            }
        }

        private async void ViewWeekClicked(object sender, EventArgs e)
        {
            try
            {
                Appointments.Clear();

                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);

                var appointments = AppointmentServiceProxy.Current.Appointments
                    .Where(a => a != null &&
                           a.StartTime >= startOfWeek &&
                           a.StartTime < endOfWeek);

                foreach (var appointment in appointments)
                {
                    if (appointment != null)
                    {
                        Appointments.Add(new AppointmentViewModel(appointment));
                    }
                }

                if (!Appointments.Any())
                {
                    await DisplayAlert("This Week's Appointments", "No appointments scheduled for this week", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to filter appointments: {ex.Message}", "OK");
            }
        }

        private void ViewAllClicked(object sender, EventArgs e)
        {
            _physicianIdFilter = 0;
            LoadAppointments();
        }

        private async void CheckConflictsClicked(object sender, EventArgs e)
        {
            try
            {
                var conflicts = new List<string>();
                var appointments = AppointmentServiceProxy.Current.Appointments
                    .Where(a => a != null && a.StartTime >= DateTime.Now)
                    .OrderBy(a => a?.StartTime)
                    .ToList();

                for (int i = 0; i < appointments.Count - 1; i++)
                {
                    for (int j = i + 1; j < appointments.Count; j++)
                    {
                        if (appointments[i]?.PhysicianId == appointments[j]?.PhysicianId &&
                            appointments[i]?.StartTime == appointments[j]?.StartTime)
                        {
                            conflicts.Add($"Dr. {appointments[i]?.PhysicianName} is double-booked at {appointments[i]?.StartTime:MM/dd/yyyy HH:mm}");
                        }
                    }
                }

                if (conflicts.Any())
                {
                    await DisplayAlert("Scheduling Conflicts", string.Join("\n", conflicts), "OK");
                }
                else
                {
                    await DisplayAlert("Scheduling", "No conflicts found", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to check conflicts: {ex.Message}", "OK");
            }
        }
    }
}