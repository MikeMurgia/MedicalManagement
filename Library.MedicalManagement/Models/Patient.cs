namespace Library.MedicalManagement.Models
{
    public class Patient
    {
        private string? Name { get; set; }
        private string? Address { get; set; }
        private string? Birthdate { get; set; }
        private string? Race { get; set; }
        private string? Gender { get; set; }
        private string? Diagnoses { get; set; }
        private string? Prescriptions { get; set; }
        public int Id { get; set; }

        public Patient(string name, string address, string birthdate, string race, string gender, string diagnoses, string prescription, int id)
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

        public override string ToString()
        {
            return $"{Name} | {Gender} | {Race} | DOB: {Birthdate} | {Address}";
        }
    }
}