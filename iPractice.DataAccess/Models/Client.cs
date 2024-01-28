using System.Collections.Generic;

namespace iPractice.DataAccess.Models;

public class Client
{
    public Client()
    {
        Psychologists = new();
        Appointments = new();
    }

    public long Id { get; set; }
    public string Name { get; set; }
    public List<Psychologist> Psychologists { get; set; }
    public List<Appointment> Appointments { get; set; }
}
