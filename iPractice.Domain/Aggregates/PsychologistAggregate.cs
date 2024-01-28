using System;
using System.Collections.Generic;
using System.Linq;
using iPractice.DataAccess.Models;
using Availability = iPractice.DataAccess.Models.Availability;

namespace iPractice.Domain.Aggregates;

public class PsychologistAggregate
{
    readonly Psychologist dataModel;

    public PsychologistAggregate(Psychologist dataModel)
    {
        this.dataModel = dataModel;
    }

    public bool OverlapsAnyAvailability(DateTime start, DateTime end)
    {
        if (!dataModel.Availability.Any())
        {
            return false;
        }

        foreach (Availability a in dataModel.Availability)
        {
            if ((start < a.End) && (end > a.Start))
            {
                return true;
            }
        }

        return false;
    }

    public void AddAvailability(DateTime start, DateTime end)
    {
        if (OverlapsAnyAvailability(start, end))
        {
            throw new ArgumentOutOfRangeException();
        }

        dataModel.Availability.Add(new Availability
        {
            Start = start,
            End = end,
            PsychologistId = dataModel.Id
        });
    }

    public IEnumerable<DateRange> GetAvailableTimeSlots(
        int sessionDurationMinutes = Constants.DefaultSessionDurationMinutes)
    {
        if (sessionDurationMinutes < 1)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(sessionDurationMinutes)} must be a positive number.");
        }

        TimeSpan duration = TimeSpan.FromMinutes(sessionDurationMinutes);

        foreach (Availability availability in dataModel.Availability)
        {
            DateTime thisDateTime = availability.Start;
            while (thisDateTime < availability.End)
            {
                yield return new DateRange(thisDateTime, thisDateTime.Add(duration));
                thisDateTime = thisDateTime.Add(duration);
            }
        }
    }

    public bool IsAvailable(DateTime start, DateTime end)
    {
        var availableTimeSlots = GetAvailableTimeSlots();
        return availableTimeSlots.Any(x => x.From == start && x.To == end);
    }

    public record DateRange(DateTime From, DateTime To);
}
