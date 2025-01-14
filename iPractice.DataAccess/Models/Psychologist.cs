﻿using System.Collections.Generic;

namespace iPractice.DataAccess.Models;

public class Psychologist
{
    public Psychologist()
    {
        Availability = new();
        Appointments = new();
    }

    public long Id { get; set; }
    public string Name { get; set; }

    public List<Client> Clients { get; set; }
    public List<Availability> Availability { get; set; }
    public List<Appointment> Appointments { get; set; }
}