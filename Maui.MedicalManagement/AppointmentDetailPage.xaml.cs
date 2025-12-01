using Library.MedicalManagement.Services;
using Library.MedicalManagement.DTO;
using Maui.MedicalManagement.ViewModels;

namespace Maui.MedicalManagement
{
    [QueryProperty(nameof(AppointmentId), "appointmentId")]
    public partial class AppointmentDetailPage : ContentPage
    {
        private AppointmentViewModel viewModel;
        private int _appointmentId;

        public int AppointmentId
        {
            get => _appointmentId;
            set
            {
                _appointmentId = value;
                LoadAppointment();
            }
        }

        public AppointmentDetailPage()
        {
            InitializeComponent();
            viewModel = new AppointmentViewModel();
            BindingContext = viewModel;
        }

        private void LoadAppointment()
        {
            if (_appointmentId > 0)
            {
                var appointment = AppointmentServiceProxy.Current.Appointments
                    .FirstOrDefault(a => a?.Id == _appointmentId);

                if (appointment != null)
                {
                    viewModel = new AppointmentViewModel(appointment);
                    BindingContext = viewModel;
                }
            }
            else
            {
                viewModel = new AppointmentViewModel();
                viewModel.SelectedDate = GetNextWeekday();
                viewModel.SelectedTime = new TimeSpan(9, 0, 0);
                BindingContext = viewModel;
            }
        }

        private DateTime GetNextWeekday()
        {
            var date = DateTime.Today;
            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(2);
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);
            return date;
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Confirm", "Are you sure you want to cancel? Any unsaved changes will be lost.", "Yes", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("///AppointmentsPage");
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (viewModel.SelectedPatient == null)
                {
                    await DisplayAlert("Validation", "Please select a patient", "OK");
                    return;
                }

                if (viewModel.SelectedPhysician == null)
                {
                    await DisplayAlert("Validation", "Please select a physician", "OK");
                    return;
                }

                if (!viewModel.SelectedDate.HasValue)
                {
                    await DisplayAlert("Validation", "Please select a date", "OK");
                    return;
                }

                if (!viewModel.SelectedTime.HasValue)
                {
                    await DisplayAlert("Validation", "Please select a time", "OK");
                    return;
                }

                viewModel.Model.PatientId = viewModel.SelectedPatient.Id;
                viewModel.Model.PhysicianId = viewModel.SelectedPhysician.Id;
                viewModel.Model.StartTime = viewModel.SelectedDate.Value.Date + viewModel.SelectedTime.Value;
                viewModel.Model.EndTime = viewModel.Model.StartTime.Value.AddMinutes(30);

                var validation = AppointmentServiceProxy.Current.IsValid(viewModel.Model);
                if (!validation.success)
                {
                    await DisplayAlert("Validation Error", validation.message, "OK");
                    return;
                }

                await AppointmentServiceProxy.Current.AddOrUpdate(viewModel.Model);
                await DisplayAlert("Success", "Appointment scheduled successfully", "OK");
                await Shell.Current.GoToAsync("///AppointmentsPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save appointment: {ex.Message}", "OK");
            }
        }

        private async void CheckAvailabilityClicked(object sender, EventArgs e)
        {
            try
            {
                if (viewModel.SelectedPhysician == null)
                {
                    await DisplayAlert("Info", "Please select a physician first", "OK");
                    return;
                }

                if (!viewModel.SelectedDate.HasValue)
                {
                    await DisplayAlert("Info", "Please select a date first", "OK");
                    return;
                }

                viewModel.CheckAvailabilityCommand?.Execute(null);

                if (viewModel.AvailableTimeSlots == null || !viewModel.AvailableTimeSlots.Any())
                {
                    await DisplayAlert("Availability", "No available time slots for this physician on the selected date", "OK");
                }
                else
                {
                    await DisplayAlert("Availability", $"Found {viewModel.AvailableTimeSlots.Count} available time slots", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to check availability: {ex.Message}", "OK");
            }
        }

        private async void DeleteClicked(object sender, EventArgs e)
        {
            if (_appointmentId <= 0)
            {
                await DisplayAlert("Info", "Cannot delete an appointment that hasn't been saved yet", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm Delete",
                "Are you sure you want to delete this appointment? This action cannot be undone.",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    AppointmentServiceProxy.Current.Delete(_appointmentId);
                    await DisplayAlert("Success", "Appointment deleted successfully", "OK");
                    await Shell.Current.GoToAsync("///AppointmentsPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete appointment: {ex.Message}", "OK");
                }
            }
        }

        private void PatientSelectionChanged(object sender, EventArgs e)
        {
            if (viewModel.SelectedPatient != null)
            {
                viewModel.Model.PatientId = viewModel.SelectedPatient.Id;
                viewModel.Model.PatientName = viewModel.SelectedPatient.Name;
            }
        }

        private void PhysicianSelectionChanged(object sender, EventArgs e)
        {
            if (viewModel.SelectedPhysician != null)
            {
                viewModel.Model.PhysicianId = viewModel.SelectedPhysician.Id;
                viewModel.Model.PhysicianName = viewModel.SelectedPhysician.Name;

                viewModel.AvailableTimeSlots?.Clear();
            }
        }

        private void DateSelectionChanged(object sender, DateChangedEventArgs e)
        {
            viewModel.AvailableTimeSlots?.Clear();
        }
    }
}