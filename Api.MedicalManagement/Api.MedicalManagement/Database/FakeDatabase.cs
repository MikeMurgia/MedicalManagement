using Library.MedicalManagement.Models;
using Api.MedicalManagement.Controllers;

namespace Api.MedicalManagement.Database
{
    public class FakeDatabase
    {
        public static List<Patient> patients = new List<Patient>
        {
            new Patient{Name = "First Person", Address="321 Street", 
                Id=1, Birthdate=DateTime.Today, Gender="male", 
                Race="white", Prescriptions="none", Diagnoses="none"},
            new Patient{Name = "Second Person", Address="9876 Drive",
                Id=2, Birthdate=DateTime.Today, Gender="female",
                Race="black", Prescriptions="some", Diagnoses="some"},
            new Patient{Name = "Third Person", Address="123 Corner",
                Id=3, Birthdate=DateTime.Today, Gender="female",
                Race="white", Prescriptions="all", Diagnoses="unknown"}
        };
    }
}
