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
    private List<Patient?> patientList;
    private PatientServiceProxy()
    {
        patientList = new List<Patient?>();
        var patResponse = new WebRequestHandler().Get("/Patients").Result;
        if (patResponse != null )
        {
            patientList = JsonConvert.DeserializeObject<List<Patient?>>(patResponse) ?? new List<Patient?>();
        }
    }
    private static PatientServiceProxy? instance;
    private static object instanceLock = new object();
    public static PatientServiceProxy Current
    {
        get
        {
            lock(instanceLock)
            {
                if (instance == null)
                {
                    instance = new PatientServiceProxy();
                }
            }
            return instance;
        }
    }

    public List<Patient?> patients
    {
        get
        {
            return patientList;
        }
    }

    public async Task<Patient?> AddOrUpdate(Patient? patient)
    {
        if (patient == null)
        {
            return null;
        }

        var patPayload = await new WebRequestHandler().Post("/Patients", patient);
        var patFromServer = JsonConvert.DeserializeObject<Patient>(patPayload);

        if (patient.Id <= 0)
        {
            patientList.Add(patFromServer);
        }
        else
        {
            var patToEdit = patients.FirstOrDefault(pat => (pat?.Id ?? 0) == patient.Id);
            if (patToEdit != null)
            {
                var index = patients.IndexOf(patToEdit);
                patients.RemoveAt(index);
                patientList.Insert(index, patient);
            }
        }
        return patient;
    }
    public Patient? Delete(int id)
    {
        var response = new WebRequestHandler().Delete($"/Patients/{id}").Result;
        var patToDelete = patientList
            .Where(pat => pat != null)
            .FirstOrDefault(pat => (pat?.Id ?? -1) == id);
        patientList.Remove(patToDelete);
        return patToDelete;
    }
}

