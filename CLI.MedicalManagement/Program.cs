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
                        var name = Console.ReadLine() ?? string.Empty;
                        var address = Console.ReadLine() ?? string.Empty;
                        var birthdate = Console.ReadLine() ?? string.Empty;
                        var race = Console.ReadLine() ?? string.Empty;
                        var gender = Console.ReadLine() ?? string.Empty;
                        var diagnoses = Console.ReadLine() ?? string.Empty;
                        var prescriptions = Console.ReadLine() ?? string.Empty;
                        int id = 1;
                        Patient patient = new Patient(name, address, birthdate, race, gender, diagnoses, prescriptions, id);
                        break;
                    case "2":
                        break;
                    case "3":
                        foreach(var p in patients)
                        {
                            Console.WriteLine(p);
                        }
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                    case "8":
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
