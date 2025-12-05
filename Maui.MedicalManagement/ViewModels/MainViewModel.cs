using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Library.MedicalManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Maui.MedicalManagement.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            InlinePatient = new PatientViewModel();
            InlineCardVisibility = Visibility.Collapsed;
            ImportPath = @"C:\Users\mikem\source\data\data.json";

            patients = new ObservableCollection<PatientViewModel?>
                    (PatientServiceProxy
                    .Current
                    .patients
                    //.Where(
                    //    p => (p?.Name?.ToUpper()?.Contains(Query?.ToUpper() ?? string.Empty) ?? false)
                    //    || (p?.Address?.ToUpper()?.Contains(Query?.ToUpper() ?? string.Empty) ?? false)
                    //)
                    .Select(p => new PatientViewModel(p))
                    );
        }

        private ObservableCollection<PatientViewModel?> patients;
        public ObservableCollection<PatientViewModel?> Patients
        {
            get
            {
                return patients;
            }
        }

        private Visibility inlineCardVisibility;
        public Visibility InlineCardVisibility
        {
            get
            {
                return inlineCardVisibility;
            }

            set
            {
                if (inlineCardVisibility != value)
                {
                    inlineCardVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void Refresh()
        {
            NotifyPropertyChanged(nameof(Patients));
        }

        public void Search()
        {
            var patDTOs = PatientServiceProxy.Current.Search(new Library.MedicalManagement.Data.QueryRequest { Content = Query }).Result;
            patients = new ObservableCollection<PatientViewModel?>(patDTOs.Select(p => new PatientViewModel(p)));
            NotifyPropertyChanged(nameof(Patients));
        }

        public void Export()
        {
            var patientString = JsonConvert.SerializeObject(
                Patients
                .Where(p => p != null && p.Model != null)
                .Select(p => p?.Model));

            using (StreamWriter sw = new StreamWriter(@"C:\Users\mikem\source\data\data.json"))
            {
                sw.WriteLine(patientString);
            }
        }

        public void Import()
        {
            using (StreamReader sr = new StreamReader(ImportPath))
            {
                var patientString = sr.ReadLine();
                if (string.IsNullOrEmpty(patientString))
                {
                    return;
                }

                var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(patientString);

                if (patients != null)
                {
                    foreach (var patient in patients)
                    {
                        patient.Id = 0;
                        //PatientServiceProxy.Current.AddOrUpdate(patients);
                    }
                    PatientServiceProxy.Current?.AddOrUpdate(patients);
                    NotifyPropertyChanged(nameof(Patients));
                }
                Refresh();
            }

            //var patientString = File.ReadAllText(ImportPath);

        }

        public string ImportPath { get; set; }
        public PatientViewModel? SelectedPatient { get; set; }
        public string? Query { get; set; }

        public PatientViewModel? InlinePatient { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Delete()
        {
            if (SelectedPatient == null)
            {
                return;
            }

            PatientServiceProxy.Current.Delete(SelectedPatient?.Model?.Id ?? 0);
            NotifyPropertyChanged(nameof(Patients));
        }

        public async Task<bool> AddInlinePatient()
        {
            try
            {
                await PatientServiceProxy.Current.AddOrUpdate(InlinePatient?.Model);
                NotifyPropertyChanged(nameof(Patients));

                InlinePatient = new PatientViewModel();
                NotifyPropertyChanged(nameof(InlinePatient));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding inline patient: {e.Message}");
                return false;
            }

            return true;
        }

        public void ExpandCard()
        {
            InlineCardVisibility
                = InlineCardVisibility == Visibility.Visible ?
                Visibility.Collapsed : Visibility.Visible;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
