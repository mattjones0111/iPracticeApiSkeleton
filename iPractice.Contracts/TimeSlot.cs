using System;

namespace iPractice.Contracts;

public class TimeSlot
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string PsychologistName { get; set; }
    public long PsychologistId { get; set; }
}
