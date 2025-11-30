using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Services;
using Maui.MedicalManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Maui.MedicalManagement.ViewModels
{
    public class MainDashboardViewModel : INotifyPropertyChanged
    {
        private int _totalPatients;
        private int _totalPhysicians;
        private int _todaysAppointments;
        private DateTime _lastRefreshTime;
        private ObservableCollection<PatientViewModel> _recentPatients;

        public MainDashboardViewModel()
        {
            _recentPatients = new ObservableCollection<PatientViewModel>();
            _lastRefreshTime = DateTime.Now;
            Refresh();
        }

        public int TotalPatients
        {
            get => _totalPatients;
            set
            {
                _totalPatients = value;
                NotifyPropertyChanged();
            }
        }

        public int TotalPhysicians
        {
            get => _totalPhysicians;
            set
            {
                _totalPhysicians = value;
                NotifyPropertyChanged();
            }
        }

        public int TodaysAppointments
        {
            get => _todaysAppointments;
            set
            {
                _todaysAppointments = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime LastRefreshTime
        {
            get => _lastRefreshTime;
            set
            {
                _lastRefreshTime = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<PatientViewModel> RecentPatients
        {
            get => _recentPatients;
            set
            {
                _recentPatients = value;
                NotifyPropertyChanged();
            }
        }

        public void Refresh()
        {
            try
            {
                // Refresh all service data
                PatientServiceProxy.Current.Refresh();
                PhysicianServiceProxy.Current.Refresh();
                AppointmentServiceProxy.Current.Refresh();

                // Update statistics
                TotalPatients = PatientServiceProxy.Current.patients?.Count(p => p != null) ?? 0;
                TotalPhysicians = PhysicianServiceProxy.Current.Physicians?.Count(p => p != null) ?? 0;

                // Count today's appointments
                var today = DateTime.Today;
                TodaysAppointments = AppointmentServiceProxy.Current.Appointments?
                    .Count(a => a != null && a.StartTime?.Date == today) ?? 0;

                // Load recent patients (last 5)
                RecentPatients.Clear();
                var recentPatients = PatientServiceProxy.Current.patients?
                    .Where(p => p != null)
                    .OrderByDescending(p => p.Id)
                    .Take(5)
                    .Select(p => new PatientViewModel(p));

                if (recentPatients != null)
                {
                    foreach (var patient in recentPatients)
                    {
                        RecentPatients.Add(patient);
                    }
                }

                LastRefreshTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                // Log error or show message
                System.Diagnostics.Debug.WriteLine($"Error refreshing dashboard: {ex.Message}");
            }
        }

        public void ExportPatients()
        {
            try
            {
                var patients = PatientServiceProxy.Current.patients
                    .Where(p => p != null)
                    .ToList();

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(patients, Newtonsoft.Json.Formatting.Indented);

                // In a real app, you'd use proper file handling for the platform
                // For now, we'll write to a standard location
                using (var writer = new System.IO.StreamWriter(@"C:\temp\data.json"))
                {
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Export failed: {ex.Message}");
            }
        }

        public void ImportPatients()
        {
            try
            {
                // In a real app, you'd use proper file handling for the platform
                // For now, we'll read from a standard location
                string json;
                using (var reader = new System.IO.StreamReader(@"C:\temp\data.json"))
                {
                    json = reader.ReadToEnd();
                }

                var patients = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientDTO>>(json);

                if (patients != null)
                {
                    foreach (var patient in patients)
                    {
                        patient.Id = 0; // Reset ID for new import
                        PatientServiceProxy.Current.AddOrUpdate(patient);
                    }

                    Refresh(); // Refresh the dashboard after import
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Import failed: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}