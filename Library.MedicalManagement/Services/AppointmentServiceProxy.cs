using Library.MedicalManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.MedicalManagement.Utilities;
using System.ComponentModel;
using Newtonsoft.Json;
using Library.MedicalManagement.DTO;

namespace Library.MedicalManagement.Services;

public class AppointmentServiceProxy
{
    private List<AppointmentDTO?> appointmentsList;
    private readonly TimeSpan workStartTime = TimeSpan.FromHours(8);
    private readonly TimeSpan workEndTime = TimeSpan.FromHours(17);

    public (bool success, string message) IsValid(AppointmentDTO? newAppointment)
    {
        if (newAppointment == null || newAppointment.PatientId <= 0 || newAppointment.PhysicianId <= 0)
        {
            return (false, "Incorrect Information - Patient and Physician are required");
        }

        if (newAppointment.StartTime == null)
        {
            return (false, "Start time is required");
        }

        DayOfWeek day = newAppointment.StartTime.Value.DayOfWeek;
        if (day == DayOfWeek.Sunday || day == DayOfWeek.Saturday)
        {
            return (false, "Appointments must be scheduled between Monday through Friday");
        }

        var hour = newAppointment.StartTime.Value.Hour;
        if (hour < 8 || hour >= 17)
        {
            return (false, "Appointments must be scheduled between 8 AM and 5 PM");
        }

        // Check for double-booking
        bool overlap = appointmentsList.Any(a =>
            a != null &&
            a.PhysicianId == newAppointment.PhysicianId &&
            a.StartTime?.Date == newAppointment.StartTime.Value.Date &&
            a.StartTime?.Hour == newAppointment.StartTime.Value.Hour &&
            a.StartTime?.Minute == newAppointment.StartTime.Value.Minute &&
            a.Id != newAppointment.Id);

        if (overlap)
        {
            return (false, "Physician is already booked at this time");
        }

        return (true, "Appointment is valid");
    }

    private AppointmentServiceProxy()
    {
        appointmentsList = new List<AppointmentDTO?>();
        Refresh();
    }

    public void Refresh()
    {
        var appointmentsResponse = new WebRequestHandler().Get("/Appointments").Result;
        if (appointmentsResponse != null)
        {
            var appointments = JsonConvert.DeserializeObject<List<Appointments?>>(appointmentsResponse) ?? new List<Appointments?>();
            appointmentsList = appointments
                .Where(a => a != null)
                .Select(a => new AppointmentDTO(a!))
                .Cast<AppointmentDTO?>()
                .ToList();

            // Load patient and physician details for each appointment
            foreach (var appointment in appointmentsList.Where(a => a != null))
            {
                if (appointment!.PatientId > 0)
                {
                    var patient = PatientServiceProxy.Current.patients.FirstOrDefault(p => p?.Id == appointment.PatientId);
                    if (patient != null)
                    {
                        appointment.Patient = patient;
                        appointment.PatientName = patient.Name;
                    }
                }

                if (appointment.PhysicianId > 0)
                {
                    var physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p?.Id == appointment.PhysicianId);
                    if (physician != null)
                    {
                        appointment.Physician = physician;
                        appointment.PhysicianName = physician.Name;
                    }
                }
            }
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

    public List<AppointmentDTO?> Appointments
    {
        get
        {
            return appointmentsList;
        }
    }

    public async Task<AppointmentDTO?> AddOrUpdate(AppointmentDTO? appointmentDto)
    {
        if (appointmentDto == null)
        {
            return null;
        }

        // Validate before saving
        var validation = IsValid(appointmentDto);
        if (!validation.success)
        {
            throw new InvalidOperationException(validation.message);
        }

        // Convert DTO to Model for API call
        var appointment = new Appointments
        {
            Id = appointmentDto.Id,
            StartTime = appointmentDto.StartTime,
            PatientId = appointmentDto.PatientId,
            PhysicianId = appointmentDto.PhysicianId
        };

        var appointmentPayload = await new WebRequestHandler().Post("/Appointments", appointment);
        var appointmentFromServer = JsonConvert.DeserializeObject<Appointments>(appointmentPayload);

        // Convert back to DTO
        var appointmentDtoFromServer = appointmentFromServer != null ? new AppointmentDTO(appointmentFromServer) : null;

        if (appointmentDto.Id <= 0)
        {
            appointmentsList.Add(appointmentDtoFromServer);
        }
        else
        {
            var appointmentToEdit = Appointments.FirstOrDefault(a => (a?.Id ?? 0) == appointmentDto.Id);
            if (appointmentToEdit != null)
            {
                var index = Appointments.IndexOf(appointmentToEdit);
                Appointments.RemoveAt(index);
                appointmentsList.Insert(index, appointmentDtoFromServer);
            }
        }

        // Refresh to get updated data with patient/physician details
        Refresh();

        return appointmentDtoFromServer;
    }

    public AppointmentDTO? Delete(int id)
    {
        var response = new WebRequestHandler().Delete($"/Appointments/{id}").Result;
        var appointmentToDelete = appointmentsList
            .Where(a => a != null)
            .FirstOrDefault(a => (a?.Id ?? -1) == id);
        appointmentsList.Remove(appointmentToDelete);
        return appointmentToDelete;
    }

    public List<DateTime> GetAvailableTimeSlots(int physicianId, DateTime date)
    {
        var availableSlots = new List<DateTime>();

        // Only weekdays
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return availableSlots;

        // Get physician's appointments for this date
        var physicianAppointments = appointmentsList
            .Where(a => a != null &&
                       a.PhysicianId == physicianId &&
                       a.StartTime?.Date == date.Date)
            .Select(a => a!.StartTime!.Value)
            .ToHashSet();

        // Generate all possible 30-minute slots from 8 AM to 5 PM
        var currentSlot = date.Date.AddHours(8); // Start at 8 AM
        var endTime = date.Date.AddHours(17);    // End at 5 PM

        while (currentSlot < endTime)
        {
            if (!physicianAppointments.Contains(currentSlot))
            {
                availableSlots.Add(currentSlot);
            }
            currentSlot = currentSlot.AddMinutes(30); // 30-minute slots
        }

        return availableSlots;
    }
}
