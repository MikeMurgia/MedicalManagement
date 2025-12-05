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

namespace Library.MedicalManagement.Services;

public class PatientServiceProxy
{
    private List<PatientDTO?> patientList;

    private PatientServiceProxy()
    {
        patientList = new List<PatientDTO?>();
        Refresh();
    }

    private static PatientServiceProxy? instance;
    private static object instanceLock = new object();

    public static PatientServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new PatientServiceProxy();
                }
            }
            return instance;
        }
    }

    public void Refresh()
    {
        try
        {
            var patResponse = new WebRequestHandler().Get("/Patients").Result;
            if (patResponse != null)
            {
                var patients = JsonConvert.DeserializeObject<List<Patient?>>(patResponse) ?? new List<Patient?>();
                patientList = patients
                    .Where(p => p != null)
                    .Select(p => new PatientDTO(p!))
                    .Cast<PatientDTO?>()
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing patients: {ex.Message}");
        }
    }

    public List<PatientDTO?> patients
    {
        get
        {
            return patientList;
        }
    }

    public async Task<PatientDTO?> AddOrUpdate(PatientDTO? patientDTO)
    {
        if (patientDTO == null)
        {
            return null;
        }

        var patient = new Patient(patientDTO);

        var patPayload = await new WebRequestHandler().Post("/Patients", patient);
        var patFromServer = JsonConvert.DeserializeObject<Patient>(patPayload);
        var patDTOFromServer = patFromServer != null ? new PatientDTO(patFromServer) : null;

        if (patientDTO.Id <= 0)
        {
            patientList.Add(patDTOFromServer);
        }
        else
        {
            var patToEdit = patients.FirstOrDefault(pat => (pat?.Id ?? 0) == patientDTO.Id);
            if (patToEdit != null)
            {
                var index = patients.IndexOf(patToEdit);
                patients.RemoveAt(index);
                patientList.Insert(index, patDTOFromServer);
            }
        }
        return patDTOFromServer;
    }

    public async Task<List<PatientDTO?>> AddOrUpdate(List<PatientDTO> patientDTOs)
    {
        var results = new List<PatientDTO?>();

        foreach (var patientDTO in patientDTOs)
        {
            var result = await AddOrUpdate(patientDTO);
            results.Add(result);
        }

        return results;
    }

    public PatientDTO? Delete(int id)
    {
        try
        {
            var response = new WebRequestHandler().Delete($"/Patients/{id}").Result;
            var patToDelete = patientList
                .Where(pat => pat != null)
                .FirstOrDefault(pat => (pat?.Id ?? -1) == id);
            patientList.Remove(patToDelete);
            return patToDelete;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting patient: {ex.Message}");
            // Still remove from local list even if API fails
            var patToDelete = patientList
                .Where(pat => pat != null)
                .FirstOrDefault(pat => (pat?.Id ?? -1) == id);
            patientList.Remove(patToDelete);
            return patToDelete;
        }
    }

    public async Task<List<PatientDTO>> Search(QueryRequest query)
    {
        try
        {
            var patPayload = await new WebRequestHandler().Post("/Patients/Search", query);
            var patFromServer = JsonConvert.DeserializeObject<List<PatientDTO?>>(patPayload);

            if (patFromServer != null)
            {
                return patFromServer.Where(p => p != null).Cast<PatientDTO>().ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching patients: {ex.Message}");
        }

        // Fallback to local search if API fails
        var searchTerm = query.Content?.ToUpper() ?? string.Empty;
        return patientList
            .Where(p => p != null &&
                ((p.Name?.ToUpper().Contains(searchTerm) ?? false) ||
                 (p.Address?.ToUpper().Contains(searchTerm) ?? false)))
            .Cast<PatientDTO>()
            .ToList();
    }
}

