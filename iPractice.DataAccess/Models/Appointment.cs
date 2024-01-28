using System;

namespace iPractice.DataAccess.Models;

public class Appointment
{
    public long Id { get; set; }
    public long PsychologistId { get; set; }
    public long ClientId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public Psychologist Psychologist { get; set; }
    public Client Client { get; set; }
}
