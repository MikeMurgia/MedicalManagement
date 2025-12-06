namespace Maui.MedicalManagement
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Patient", typeof(PatientDetailPage));
            Routing.RegisterRoute("PatientDetail", typeof(PatientDetailPage));
            Routing.RegisterRoute("PhysicianDetail", typeof(PhysicianDetailPage));
            Routing.RegisterRoute("AppointmentDetail", typeof(AppointmentDetailPage));
        }
    }
}
