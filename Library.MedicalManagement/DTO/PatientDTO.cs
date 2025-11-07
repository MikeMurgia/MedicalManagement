using Library.MedicalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.DTO
{
    public class PatientDTO
    {
        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }

        public string Display
        {
            get
            {
                return $"[{Id}] {Name}";
            }
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Address { get; set; }
        public string? Race { get; set; }
        public string? Gender { get; set; }

        public PatientDTO() { }
        public PatientDTO(Patient p)
        {
            Id = p.Id ;
            Name = p.Name ;
            Birthdate = p.Birthdate ;
            Address = p.Address ;
            Race = p.Race ;
            Gender = p.Gender ;
        }
    }
}
