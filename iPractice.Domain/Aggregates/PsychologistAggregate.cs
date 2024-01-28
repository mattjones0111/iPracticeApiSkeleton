using System;
using System.Linq;
using iPractice.DataAccess.Models;

namespace iPractice.Domain.Aggregates;

public class PsychologistAggregate
{
    private readonly Psychologist dataModel;

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
}
