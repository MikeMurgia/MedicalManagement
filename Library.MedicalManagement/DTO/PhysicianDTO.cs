using Library.MedicalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.DTO
{
    public class PhysicianDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? License { get; set; }
        public DateTime GraduationDate { get; set; }
        public string? Specializations { get; set; }

        public string Display
        {
            get
            {
                return $"[{Id}] {Name} - {License}";
            }
        }

        public PhysicianDTO() { }

        public PhysicianDTO(Physician p)
        {
            Id = p.Id;
            Name = p.Name;
            License = p.License;
            GraduationDate = p.GraduationDate;
            Specializations = p.Specializations;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} - License: {License}";
        }
    }
}
