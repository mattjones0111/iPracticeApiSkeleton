using System;
using System.Linq;
using iPractice.DataAccess.Models;

namespace iPractice.Domain.Aggregates;

public class ClientAggregate
{
    readonly Client dataModel;

    public ClientAggregate(Client dataModel)
    {
        this.dataModel = dataModel;
    }

    public bool HasPsychologist(long psychologistId)
    {
        return dataModel.Psychologists.Any(x => x.Id == psychologistId);
    }

    public void BookAppointment(DateTime from, DateTime to, long psychologistId)
    {
        dataModel.Appointments.Add(new Appointment
        {
            ClientId = dataModel.Id,
            PsychologistId = psychologistId,
            Start = from,
            End = to
        });
    }
}
