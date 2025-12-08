using Api.MedicalManagement.Database;
using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
    
namespace Api.MedicalManagement.Enterprise
{
    public class PatientsEC
    {
        public IEnumerable<PatientDTO> GetPats()
        {
            return Filebase.Current.Pats
                //.Where(p => p.UserId == CLAIM.UserId)
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Birthdate = p.Birthdate,
                    Race = p.Race,
                    Gender = p.Gender,
                    Diagnoses = p.Diagnoses,
                    Prescriptions = p.Prescriptions
                })
                .OrderByDescending(p => p.Id)
                .Take(100);
        }
        public PatientDTO? GetById(int id)
        {
            var patient = Filebase.Current.Pats.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return null;
            }
            return new PatientDTO(patient);
        }

        public PatientDTO? Delete(int id)
        {
            var patToDelete = Filebase.Current.Delete(id);
            if (patToDelete != null)
            {
                return new PatientDTO(patToDelete);
            }
            return null;
        }

        public PatientDTO? AddOrUpdate(PatientDTO? patientDTO)
        {
            if (patientDTO == null)
            {
                return null;
            }
            var patient = new Patient(patientDTO);
            patientDTO = new PatientDTO(Filebase.Current.AddOrUpdate(patient));
            return patientDTO;
        }

        public IEnumerable<PatientDTO?> Search(string query)
        {
            return Filebase.Current.Pats.Where(
                        p => (p?.Name?.ToUpper()?.Contains(query?.ToUpper() ?? string.Empty) ?? false)
                        || (p?.Address?.ToUpper()?.Contains(query?.ToUpper() ?? string.Empty) ?? false)
                    ).Select(p => new PatientDTO(p));
        }
    }
}
