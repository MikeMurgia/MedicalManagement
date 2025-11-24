using Library.MedicalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.MedicalManagement.Utilities;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Library.MedicalManagement.Services;

public class AppointmentServiceProxy
{
    private readonly List<Appointments?> appointmentsList;
    private readonly TimeSpan workStartTime = TimeSpan.FromHours(8);
    private readonly TimeSpan workEndTime = TimeSpan.FromHours(17);

    public (bool success, string message) IsValid(Appointments? newAppointment)
    {
        if (newAppointment == null || newAppointment.Physician == null || newAppointment.Patient == null)
        {
            return (false, "Incorrect Information");
        }
        DayOfWeek day = newAppointment.StartTime.Value.DayOfWeek;
        if (day == DayOfWeek.Sunday)
        {
            return (false, "Appointments must be scheduled between Monday through Friday");
        }
        if (day == DayOfWeek.Saturday)
        {
            return (false, "Appointments must be scheduled between Monday through Friday");
        }
        var hour = newAppointment.StartTime.Value.Hour;
        if (hour < 8 ||  hour > 17)
        {
            return (false, "Appointments must be scheduled between 8 AM and 5 PM");
        }
        bool overlap = appointmentsList.Any(a => a?.Physician?.Id == newAppointment.Physician.Id && a.StartTime == newAppointment.StartTime && a.Id != newAppointment.Id);
        if (overlap)
        {
            return (false, "Physician is already booked");
        }
        return (true, "Appointment made");
    }

    
    private AppointmentServiceProxy()
    {
        appointmentsList = new List<Appointments?>();
        var appointmentsResponse = new WebRequestHandler().Get("/Appointments").Result;
        if (appointmentsResponse != null)
        {
            appointmentsList = JsonConvert.DeserializeObject<List<Appointments?>>(appointmentsResponse) ?? new List<Appointments?>();
        }
    }
    private static AppointmentServiceProxy? instance;
    private static object instanceLock = new object();
    public static AppointmentServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new AppointmentServiceProxy();
                }
            }
            return instance;
        }
    }

    public List<Appointments?> appointments
    {
        get
        {
            return appointmentsList;
        }
    }

    public async Task<Appointments?> AddOrUpdate(Appointments? appointment)
    {
        if (appointment == null)
        {
            return null;
        }
        var AppointmentPayload = await new WebRequestHandler().Post("/Appointments", appointment);
        
        var AppointmentFromServer = JsonConvert.DeserializeObject<Appointments>(AppointmentPayload);

        if (appointment.Id <= 0)
        {
            appointmentsList.Add(AppointmentFromServer);
        }
        else
        {
            var AppointmentToEdit = appointments.FirstOrDefault(a => (a?.Id ?? 0) == appointment.Id);
            if (AppointmentToEdit != null)
            {
                var index = appointments.IndexOf(AppointmentToEdit);
                appointments.RemoveAt(index);
                appointmentsList.Insert(index, appointment);
            }
        }
        
        return appointment;
    }

    public Appointments? Delete(int id)
    {
        var response = new WebRequestHandler().Delete($"/Appointment/{id}").Result;
        var AppointmentToDelete = appointmentsList.Where(a => a != null).FirstOrDefault(a => (a?.Id ?? -1) == id);
        appointmentsList.Remove(AppointmentToDelete);
        return AppointmentToDelete;
    }
}

