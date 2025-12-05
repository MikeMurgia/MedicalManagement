using Maui.MedicalManagement.ViewModels;
using Library.MedicalManagement.Services;

namespace Maui.MedicalManagement
    {
        public partial class MainPage : ContentPage
        {
            public MainPage()
            {
                InitializeComponent();
            }

            private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
            {
                BindingContext = new MainViewModel();
                (BindingContext as MainViewModel)?.Refresh();
            }
            private async void AddPatientClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("PatientDetail?patientId=0");
            }

            private async void ScheduleAppointmentClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("AppointmentDetail?appointmentId=0");
            }

            private async void ManagePatientsClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///PatientsPage");
            }

            private async void ManagePhysiciansClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///PhysiciansPage");
            }

            private async void ViewAppointmentsClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///AppointmentsPage");
            }

            private async void AddPhysicianClicked(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("PhysicianDetail?physicianId=0");
            }

            
            private async void PatientCardTapped(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///PatientsPage");
            }

            private async void PhysicianCardTapped(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///PhysiciansPage");
            }

            private async void AppointmentCardTapped(object sender, EventArgs e)
            {
                await Shell.Current.GoToAsync("///AppointmentsPage");
            }
        }
    }