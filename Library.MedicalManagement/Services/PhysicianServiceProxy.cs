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
            physicianList = new List<PhysicianDTO?>();
            var physicianResponse = new WebRequestHandler().Get("/Physicians").Result;
            if (physicianResponse != null)
            {
                var physicians = JsonConvert.DeserializeObject<List<Physician?>>(physicianResponse) ?? new List<Physician?>();
                physicianList = physicians
                    .Where(p => p != null)
                    .Select(p => new PhysicianDTO(p!))
                    .Cast<PhysicianDTO?>()
                    .ToList();
            }
        }
        public void Refresh()
        {
            var physicianResponse = new WebRequestHandler().Get("/Physicians").Result;
            if (physicianResponse != null)
            {
                var physicians = JsonConvert.DeserializeObject<List<Physician?>>(physicianResponse) ?? new List<Physician?>();
                physicianList = physicians
                    .Where(p => p != null)
                    .Select(p => new PhysicianDTO(p!))
                    .Cast<PhysicianDTO?>()
                    .ToList();
            }
        }

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

        public async Task<PhysicianDTO?> AddOrUpdate(PhysicianDTO? physicianDto)
        {
            if (physicianDto == null)
            {
                return null;
            }

            // Convert DTO to Model for API call
            var physician = new Physician
            {
                Id = physicianDto.Id,
                Name = physicianDto.Name ?? string.Empty,
                License = physicianDto.License ?? string.Empty,
                GraduationDate = physicianDto.GraduationDate,
                Specializations = physicianDto.Specializations ?? string.Empty
            };

            var physicianPayload = await new WebRequestHandler().Post("/Physicians", physician);
            var physicianFromServer = JsonConvert.DeserializeObject<Physician>(physicianPayload);

            // Convert back to DTO
            var physicianDtoFromServer = physicianFromServer != null ? new PhysicianDTO(physicianFromServer) : null;

            if (physicianDto.Id <= 0)
            {
                physicianList.Add(physicianDtoFromServer);
            }
            else
            {
                var physicianToEdit = Physicians.FirstOrDefault(p => (p?.Id ?? 0) == physicianDto.Id);
                if (physicianToEdit != null)
                {
                    var index = Physicians.IndexOf(physicianToEdit);
                    Physicians.RemoveAt(index);
                    physicianList.Insert(index, physicianDtoFromServer);
                }
            }

            return physicianDtoFromServer;
        }

        public PhysicianDTO? Delete(int id)
        {
            var response = new WebRequestHandler().Delete($"/Physicians/{id}").Result;
            var physicianToDelete = physicianList
                .Where(p => p != null)
                .FirstOrDefault(p => (p?.Id ?? -1) == id);
            physicianList.Remove(physicianToDelete);
            return physicianToDelete;
        }

        public async Task<List<PhysicianDTO>> Search(QueryRequest query)
        {
            var physicianPayload = await new WebRequestHandler().Post("/Physicians/Search", query);
            var physiciansFromServer = JsonConvert.DeserializeObject<List<PhysicianDTO?>>(physicianPayload);

            if (physiciansFromServer != null)
            {
                physicianList = physiciansFromServer;
                return physiciansFromServer.Where(p => p != null).Cast<PhysicianDTO>().ToList();
            }

            return new List<PhysicianDTO>();
        }
    }
}
