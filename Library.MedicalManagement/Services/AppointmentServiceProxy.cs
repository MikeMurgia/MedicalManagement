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

        // Examples:
        var today = DateTime.Today;
        if (today.DayOfWeek == DayOfWeek.Saturday)
            today = today.AddDays(2);
        else if (today.DayOfWeek == DayOfWeek.Sunday)
            today = today.AddDays(1);

        appointmentsList.Add(new AppointmentDTO
        {
            Id = 1,
            StartTime = today.AddHours(9),
            EndTime = today.AddHours(9).AddMinutes(30),
            PatientId = 1,
            PhysicianId = 1,
            PatientName = "person one",
            PhysicianName = "Dr. John Smith"
        });

        appointmentsList.Add(new AppointmentDTO
        {
            Id = 2,
            StartTime = today.AddHours(10),
            EndTime = today.AddHours(10).AddMinutes(30),
            PatientId = 2,
            PhysicianId = 2,
            PatientName = "person two",
            PhysicianName = "Dr. Sarah Johnson"
        });
    }

    public void Refresh()
    {
        foreach (var appointment in appointmentsList.Where(a => a != null))
        {
            if (appointment!.PatientId > 0)
            {
                var patient = PatientServiceProxy.Current.patients.FirstOrDefault(p => p?.Id == appointment.PatientId);
                if (patient != null)
                {
                    appointment.PatientName = patient.Name;
                }
            }

            if (appointment.PhysicianId > 0)
            {
                var physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p?.Id == appointment.PhysicianId);
                if (physician != null)
                {
                    appointment.PhysicianName = physician.Name;
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

    public Task<AppointmentDTO?> AddOrUpdate(AppointmentDTO? appointmentDto)
    {
        if (appointmentDto == null)
        {
            return Task.FromResult<AppointmentDTO?>(null);
        }

        // Validate before saving
        var validation = IsValid(appointmentDto);
        if (!validation.success)
        {
            throw new InvalidOperationException(validation.message);
        }

        // Get patient and physician names
        var patient = PatientServiceProxy.Current.patients.FirstOrDefault(p => p?.Id == appointmentDto.PatientId);
        var physician = PhysicianServiceProxy.Current.Physicians.FirstOrDefault(p => p?.Id == appointmentDto.PhysicianId);

        appointmentDto.PatientName = patient?.Name;
        appointmentDto.PhysicianName = physician?.Name;

        if (appointmentDto.Id <= 0)
        {
            // New appointment
            appointmentDto.Id = appointmentsList.Any()
                ? appointmentsList.Where(a => a != null).Max(a => a!.Id) + 1
                : 1;
            appointmentsList.Add(appointmentDto);
        }
        else
        {
            // Update existing
            var existing = appointmentsList.FirstOrDefault(a => a?.Id == appointmentDto.Id);
            if (existing != null)
            {
                var index = appointmentsList.IndexOf(existing);
                appointmentsList.RemoveAt(index);
                appointmentsList.Insert(index, appointmentDto);
            }
            else
            {
                appointmentsList.Add(appointmentDto);
            }
        }

        return Task.FromResult<AppointmentDTO?>(appointmentDto);
    }

    public AppointmentDTO? Delete(int id)
    {
        var appointmentToDelete = appointmentsList
            .Where(a => a != null)
            .FirstOrDefault(a => a!.Id == id);

        if (appointmentToDelete != null)
        {
            appointmentsList.Remove(appointmentToDelete);
        }

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
            currentSlot = currentSlot.AddMinutes(30);
        }

        return availableSlots;
    }
}
