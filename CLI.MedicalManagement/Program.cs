using Library.MedicalManagement.Models;
using System;


namespace CLI.MedicalManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Medical Management!");
            List<Patient?> patients = new List<Patient?>();
            List<Physician?> physicians = new List<Physician?>();
            List<Appointments?> appointments = new List<Appointments?>();

            do
            {
                Console.WriteLine("1. Create Patient");
                Console.WriteLine("2. Edit Patients");
                Console.WriteLine("3. View Patients");
                Console.WriteLine("4. Create Physicians");
                Console.WriteLine("5. Edit Physicians");
                Console.WriteLine("6. View Physicians");
                Console.WriteLine("7. Create Appointment");
                Console.WriteLine("8. View Appointment");
                Console.WriteLine("9. Exit");

                var userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    case "1":
                        var patient = new Patient();
                        patient.Name = Console.ReadLine() ?? string.Empty;
                        patient.Address = Console.ReadLine() ?? string.Empty;
                        patient.Birthdate = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                        patient.Race = Console.ReadLine() ?? string.Empty;
                        patient.Gender = Console.ReadLine() ?? string.Empty;
                        patient.Diagnoses = Console.ReadLine() ?? string.Empty;
                        patient.Prescriptions = Console.ReadLine() ?? string.Empty;
                        var maxIdPA = -1;
                        if (patients.Any())
                        {
                            maxIdPA = patients.Select(b => b?.Id ?? -1).Max();
                        } else
                        {
                            maxIdPA = 0;
                        }
                        patient.Id = ++maxIdPA;
                        patients.Add(patient);
                        break;
                    case "2":
                        Console.WriteLine("Patient to Update (Id):");
                        var PatSelection = Console.ReadLine();
                        if (int.TryParse(PatSelection, out int PatIntSelection))
                        {
                            var PatToUpdate = patients
                                .Where(p => p != null)
                                .FirstOrDefault(p => (p?.Id ?? -1) == PatIntSelection);
                            if (PatToUpdate != null)
                            {
                                PatToUpdate.Name = Console.ReadLine();
                                PatToUpdate.Address = Console.ReadLine();
                                PatToUpdate.Birthdate = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                                PatToUpdate.Race = Console.ReadLine();
                                PatToUpdate.Gender = Console.ReadLine();
                                PatToUpdate.Diagnoses = Console.ReadLine();
                                PatToUpdate.Prescriptions = Console.ReadLine();

                            }

                        }
                        break;
                    case "3":
                        foreach(var p in patients)
                        {
                            Console.WriteLine(p);
                        }
                        break;
                    case "4":
                        var physician = new Physician();
                        physician.Name = Console.ReadLine() ?? string.Empty;
                        physician.License = Console.ReadLine() ?? string.Empty;
                        physician.Specializations = Console.ReadLine() ?? string.Empty;
                        physician.GraduationDate = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                        var maxIdPH = -1;
                        if (physicians.Any())
                        {
                            maxIdPH = physicians.Select(b => b?.Id ?? -1).Max();
                        }
                        else
                        {
                            maxIdPH = 0;
                        }
                        physician.Id = ++maxIdPH;
                        physicians.Add(physician);
                        break;
                    case "5":
                        Console.WriteLine("Physician to Update (Id):");
                        var PhySelection = Console.ReadLine();
                        if (int.TryParse(PhySelection, out int PhyIntSelection))
                        {
                            var PhyToUpdate = physicians
                                .Where(ph => ph != null)
                                .FirstOrDefault(ph => (ph?.Id ?? -1) == PhyIntSelection);
                            if (PhyToUpdate != null)
                            {
                                PhyToUpdate.Name = Console.ReadLine() ?? string.Empty;
                                PhyToUpdate.License = Console.ReadLine() ?? string.Empty;
                                PhyToUpdate.GraduationDate = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                                PhyToUpdate.Specializations = Console.ReadLine() ?? string.Empty;

                            }

                        }
                        break;
                    case "6":
                        foreach(var p in physicians)
                        { 
                            Console.WriteLine(p); 
                        }
                        break;
                    case "7":
                        var appointment = new Appointments();
                        appointment.StartTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                        appointment.EndTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                        appointment.PatientId = int.Parse(Console.ReadLine() ?? string.Empty);
                        appointment.PhysicianId = int.Parse(Console.ReadLine() ?? string.Empty);
                        break;
                    case "8":
                        Console.WriteLine("Physician to Update (Id):");
                        var AppSelection = Console.ReadLine();
                        if (int.TryParse(AppSelection, out int AppIntSelection))
                        {
                            var AppToUpdate = appointments
                                .Where(a => a != null)
                                .FirstOrDefault(a => (a?.Id ?? -1) == AppIntSelection);
                            if (AppToUpdate != null)
                            {
                                AppToUpdate.StartTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                                AppToUpdate.EndTime = DateTime.Parse(Console.ReadLine() ?? string.Empty);
                                AppToUpdate.PatientId = int.Parse(Console.ReadLine() ?? string.Empty);
                                AppToUpdate.PhysicianId = int.Parse(Console.ReadLine() ?? string.Empty);
                            }

                        }
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            } while (true);
        }
    }
}
