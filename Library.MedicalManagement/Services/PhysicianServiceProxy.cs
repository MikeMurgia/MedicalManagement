using Library.MedicalManagement.Data;
using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Library.MedicalManagement.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.MedicalManagement.Services
{
    public class PhysicianServiceProxy
    {
        private List<PhysicianDTO?> physicianList;

        private PhysicianServiceProxy()
        {
            physicianList = new List<PhysicianDTO?>
            {
                /*Examples:
                new PhysicianDTO
                {
                    Id = 1,
                    Name = "Dr. John Smith",
                    License = "MD12345",
                    GraduationDate = DateTime.Today.AddYears(-15),
                    Specializations = "General Practice"
                },
                new PhysicianDTO
                {
                    Id = 2,
                    Name = "Dr. Sarah Johnson",
                    License = "MD67890",
                    GraduationDate = DateTime.Today.AddYears(-10),
                    Specializations = "Cardiology"
                },
                new PhysicianDTO
                {
                    Id = 3,
                    Name = "Dr. Michael Murgia",
                    License = "MD11111",
                    GraduationDate = DateTime.Today.AddYears(-8),
                    Specializations = "Pediatrics"
                }*/
            };
        }

        //Refresh not needed

        private static PhysicianServiceProxy? instance;
        private static object instanceLock = new object();

        public static PhysicianServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new PhysicianServiceProxy();
                    }
                }
                return instance;
            }
        }

        public List<PhysicianDTO?> Physicians
        {
            get
            {
                return physicianList;
            }
        }

        public Task<PhysicianDTO?> AddOrUpdate(PhysicianDTO? physicianDto)
        {
            if (physicianDto == null)
            {
                return Task.FromResult<PhysicianDTO?>(null);
            }

            if (physicianDto.Id <= 0)
            {
                // New physician
                physicianDto.Id = physicianList.Any()
                    ? physicianList.Where(p => p != null).Max(p => p!.Id) + 1
                    : 1;
                physicianList.Add(physicianDto);
            }
            else
            {
                //Update existing
                var existing = physicianList.FirstOrDefault(p => p?.Id == physicianDto.Id);
                if (existing != null)
                {
                    var index = physicianList.IndexOf(existing);
                    physicianList.RemoveAt(index);
                    physicianList.Insert(index, physicianDto);
                }
                else
                {
                    physicianList.Add(physicianDto);
                }
            }

            return Task.FromResult<PhysicianDTO?>(physicianDto);
        }

        public PhysicianDTO? Delete(int id)
        {
            var physicianToDelete = physicianList
                .Where(p => p != null)
                .FirstOrDefault(p => p!.Id == id);

            if (physicianToDelete != null)
            {
                physicianList.Remove(physicianToDelete);
            }

            return physicianToDelete;
        }

        public Task<List<PhysicianDTO>> Search(QueryRequest query)
        {
            var searchTerm = query.Content?.ToUpper() ?? string.Empty;

            var results = physicianList
                .Where(p => p != null &&
                    ((p.Name?.ToUpper().Contains(searchTerm) ?? false) ||
                     (p.Specializations?.ToUpper().Contains(searchTerm) ?? false) ||
                     (p.License?.ToUpper().Contains(searchTerm) ?? false)))
                .Cast<PhysicianDTO>()
                .ToList();

            return Task.FromResult(results);
        }
    }
}
