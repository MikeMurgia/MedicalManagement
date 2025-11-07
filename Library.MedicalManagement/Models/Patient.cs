using Library.MedicalManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.Models
{
    public class Patient
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Race { get; set; }
        public string? Gender { get; set; }
        public string? Diagnoses { get; set; }
        public string? Prescriptions { get; set; }
        public int Id { get; set; }

        public Patient(string name, string address, DateTime birthdate, string race, string gender, string diagnoses, string prescription, int id)
        {
            Name = name;
            Address = address;
            Birthdate = birthdate;
            Race = race;
            Gender = gender;
            Diagnoses = diagnoses;
            Prescriptions = prescription;
            Id = id;
        }

        public Patient() 
        { 
            Name = string.Empty;
            Address = string.Empty;
            Birthdate = DateTime.MinValue;
            Race = string.Empty;
            Gender = string.Empty;
        }
        public Patient(PatientDTO p)
        {
            Name = p.Name;
            Address = p.Address;
            Birthdate = p.Birthdate;
            Race = p.Race;
            Gender = p.Gender;
            Id = p.Id;
        }

        public override string ToString()
        {
            return $"[{Id}] | {Name} | {Gender} | {Race} | DOB: {Birthdate} | {Address}";
        }
    }
}