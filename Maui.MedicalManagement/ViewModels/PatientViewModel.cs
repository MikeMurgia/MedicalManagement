using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Library.MedicalManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Maui.MedicalManagement.ViewModels
{
    public class PatientViewModel
    {
        public PatientViewModel()
        {
            Model = new PatientDTO();
            SetUpCommands();
        }

        public PatientViewModel(PatientDTO? model)
        {
            Model = model;
            SetUpCommands();
        }

        private void SetUpCommands()
        {
            DeleteCommand = new Command(DoDelete);
            EditCommand = new Command((p) => DoEdit(p as PatientViewModel));
        }

        private void DoDelete()
        {
            if (Model?.Id > 0)
            {
                PatientServiceProxy.Current.Delete(Model.Id);
                Shell.Current.GoToAsync("///PatientsPage");
            }
        }

        private void DoEdit(PatientViewModel? bv)
        {
            if (bv == null)
            {
                return;
            }
            var selectedId = bv?.Model?.Id ?? 0;
            Shell.Current.GoToAsync($"PatientDetail?patientId={selectedId}");
        }

        public PatientDTO? Model { get; set; }


        public ICommand? DeleteCommand { get; set; }
        public ICommand? EditCommand { get; set; }
    }
}
