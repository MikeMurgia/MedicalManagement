using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Library.MedicalManagement.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Maui.MedicalManagement.ViewModels
{
    public class AppointmentViewModel : INotifyPropertyChanged
    {
        public AppointmentViewModel()
        {
            Model = new AppointmentDTO();
            SetUpCommands();
            LoadPatients();
            LoadPhysicians();
        }

        public AppointmentViewModel(AppointmentDTO? model)
        {
            Model = model ?? new AppointmentDTO();
            SetUpCommands();
            LoadPatients();
            LoadPhysicians();
        }

        private void SetUpCommands()
        {
            DeleteCommand = new Command(DoDelete);
            EditCommand = new Command((a) => DoEdit(a as AppointmentViewModel));
            SaveCommand = new Command(async () => await DoSave());
            CheckAvailabilityCommand = new Command(DoCheckAvailability);
        }

        private void LoadPatients()
        {
            AvailablePatients = new ObservableCollection<PatientDTO>(
                PatientServiceProxy.Current.patients.Where(p => p != null).Cast<PatientDTO>()
            );

            if (Model?.PatientId > 0)
            {
                SelectedPatient = AvailablePatients.FirstOrDefault(p => p.Id == Model.PatientId);
            }
        }

        private void LoadPhysicians()
        {
            AvailablePhysicians = new ObservableCollection<PhysicianDTO>(
                PhysicianServiceProxy.Current.Physicians.Where(p => p != null).Cast<PhysicianDTO>()
            );

            if (Model?.PhysicianId > 0)
            {
                SelectedPhysician = AvailablePhysicians.FirstOrDefault(p => p.Id == Model.PhysicianId);
            }
        }

        private void DoDelete()
        {
            if (Model?.Id > 0)
            {
                AppointmentServiceProxy.Current.Delete(Model.Id);
                Shell.Current.GoToAsync("///AppointmentsPage");
            }
        }

        private void DoEdit(AppointmentViewModel? av)
        {
            if (av == null)
            {
                return;
            }
            var selectedId = av?.Model?.Id ?? 0;
            Shell.Current.GoToAsync($"AppointmentDetail?appointmentId={selectedId}");
        }

        private async Task DoSave()
        {
            if (Model == null)
                return;

            if (SelectedPatient != null)
                Model.PatientId = SelectedPatient.Id;

            if (SelectedPhysician != null)
                Model.PhysicianId = SelectedPhysician.Id;

            // Combine selected date and time
            if (SelectedDate.HasValue && SelectedTime.HasValue)
            {
                Model.StartTime = SelectedDate.Value.Date + SelectedTime.Value;
                Model.EndTime = Model.StartTime.Value.AddMinutes(30); // 30-minute appointments
            }

            try
            {
                var validation = AppointmentServiceProxy.Current.IsValid(Model);
                if (!validation.success)
                {
                    ValidationMessage = validation.message;
                    IsValid = false;
                    return;
                }

                await AppointmentServiceProxy.Current.AddOrUpdate(Model);
                await Shell.Current.GoToAsync("///AppointmentsPage");
            }
            catch (Exception ex)
            {
                ValidationMessage = ex.Message;
                IsValid = false;
            }
        }

        private void DoCheckAvailability()
        {
            if (SelectedPhysician == null || !SelectedDate.HasValue)
            {
                ValidationMessage = "Please select a physician and date first";
                return;
            }

            var availableSlots = AppointmentServiceProxy.Current.GetAvailableTimeSlots(
                SelectedPhysician.Id,
                SelectedDate.Value
            );

            AvailableTimeSlots = new ObservableCollection<TimeSpan>(
                availableSlots.Select(dt => dt.TimeOfDay)
            );

            if (!AvailableTimeSlots.Any())
            {
                ValidationMessage = "No available time slots for this physician on the selected date";
            }
            else
            {
                ValidationMessage = $"{AvailableTimeSlots.Count} time slots available";
            }

            NotifyPropertyChanged(nameof(AvailableTimeSlots));
            NotifyPropertyChanged(nameof(ValidationMessage));
        }

        private AppointmentDTO? _model;
        public AppointmentDTO? Model
        {
            get => _model;
            set
            {
                _model = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PatientDTO>? _availablePatients;
        public ObservableCollection<PatientDTO>? AvailablePatients
        {
            get => _availablePatients;
            set
            {
                _availablePatients = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PhysicianDTO>? _availablePhysicians;
        public ObservableCollection<PhysicianDTO>? AvailablePhysicians
        {
            get => _availablePhysicians;
            set
            {
                _availablePhysicians = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<TimeSpan>? _availableTimeSlots;
        public ObservableCollection<TimeSpan>? AvailableTimeSlots
        {
            get => _availableTimeSlots;
            set
            {
                _availableTimeSlots = value;
                NotifyPropertyChanged();
            }
        }

        private PatientDTO? _selectedPatient;
        public PatientDTO? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                if (Model != null && value != null)
                {
                    Model.PatientId = value.Id;
                    Model.PatientName = value.Name;
                }
                NotifyPropertyChanged();
            }
        }

        private PhysicianDTO? _selectedPhysician;
        public PhysicianDTO? SelectedPhysician
        {
            get => _selectedPhysician;
            set
            {
                _selectedPhysician = value;
                if (Model != null && value != null)
                {
                    Model.PhysicianId = value.Id;
                    Model.PhysicianName = value.Name;
                }
                NotifyPropertyChanged();
            }
        }

        private DateTime? _selectedDate = DateTime.Today;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime MinDate { get; } = DateTime.Today;

        private TimeSpan? _selectedTime = new TimeSpan(9, 0, 0);
        public TimeSpan? SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                NotifyPropertyChanged();
            }
        }

        private string? _validationMessage;
        public string? ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand? DeleteCommand { get; set; }
        public ICommand? EditCommand { get; set; }
        public ICommand? SaveCommand { get; set; }
        public ICommand? CheckAvailabilityCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
