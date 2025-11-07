using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.Models
{
    public class Physician
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string License { get; set; }
        public DateTime GraduationDate { get; set; }
        public string Specializations { get; set; }
        public Physician() 
        {
            Name = string.Empty;
            License = string.Empty;
            Specializations = string.Empty;
            GraduationDate = DateTime.MinValue;
        }
    }
}
