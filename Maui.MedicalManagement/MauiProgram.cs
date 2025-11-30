using Microsoft.Extensions.Logging;
using Maui.MedicalManagement.ViewModels;

namespace Maui.MedicalManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<PatientsPage>();
            builder.Services.AddTransient<PatientDetailPage>();
            builder.Services.AddTransient<PhysiciansPage>();
            builder.Services.AddTransient<PhysicianDetailPage>();
            builder.Services.AddTransient<AppointmentsPage>();
            builder.Services.AddTransient<AppointmentDetailPage>();

            // Register ViewModels
            builder.Services.AddTransient<MainDashboardViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<AppointmentViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
