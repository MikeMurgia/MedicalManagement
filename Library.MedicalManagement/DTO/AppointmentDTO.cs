using Library.MedicalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PatientId { get; set; }
        public int PhysicianId { get; set; }
        public string? PatientName { get; set; }
        public string? PhysicianName { get; set; }
        public PatientDTO? Patient { get; set; }
        public PhysicianDTO? Physician { get; set; }

        public string Display
        {
            get
            {
                return $"[{Id}] {StartTime:MM/dd/yyyy HH:mm} - Patient: {PatientName ?? "N/A"} - Doctor: {PhysicianName ?? "N/A"}";
            }
        }

        public AppointmentDTO()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddMinutes(30);
        }

        public AppointmentDTO(Appointments a)
        {
            Id = a.Id;
            StartTime = a.StartTime;
            EndTime = a.StartTime?.AddMinutes(30); // Default 30-minute appointments
            PatientId = a.PatientId;
            PhysicianId = a.PhysicianId;

            if (a.Patient != null)
            {
                Patient = new PatientDTO(a.Patient);
                PatientName = a.Patient.Name;
            }

            if (a.Physician != null)
            {
                Physician = new PhysicianDTO(a.Physician);
                PhysicianName = a.Physician.Name;
            }
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
