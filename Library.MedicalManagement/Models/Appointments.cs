using Library.MedicalManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.Models
{
    public class Appointments
    {
        public Appointments() 
        { 
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            PatientId = -1;
            PhysicianId = -1;
        }

        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int PatientId { get; set; }
        public int PhysicianId { get; set; }
        public PatientDTO? Patient { get; set; }
    }
}
