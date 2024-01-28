using FluentAssertions;
using iPractice.Contracts;
using iPractice.DataAccess.Models;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace iPractice.IntegrationTests;

[Collection("Api collection")]
public class CanBookAppointment : IAsyncLifetime
{
    long psychologist1Id;
    long psychologist2Id;
    long clientId;

    readonly ApiFixture apiFixture;

    public CanBookAppointment(ApiFixture apiFixture)
    {
        this.apiFixture = apiFixture;
    }

    public async Task InitializeAsync()
    {
        // create two new psychologists
        Psychologist p1 = new() { Name = "P2" };
        p1.Availability.Add(new()
        {
            Start = new DateTime(2024, 1, 1, 9, 0, 0),
            End = new DateTime(2024, 1, 1, 17, 0, 0)
        });

        Psychologist p2 = new() { Name = "P2" };
        p2.Availability.Add(new()
        {
            Start = new DateTime(2024, 1, 1, 9, 0, 0),
            End = new DateTime(2024, 1, 1, 17, 0, 0)
        });

        // create a client
        Client c = new() { Name = "Client" };
        c.Psychologists.Add(p1);
        c.Psychologists.Add(p2);

        apiFixture.Db.Psychologists.Add(p1);
        apiFixture.Db.Psychologists.Add(p2);
        apiFixture.Db.Clients.Add(c);

        await apiFixture.Db.SaveChangesAsync();

        psychologist1Id = p1.Id;
        psychologist2Id = p2.Id;
        clientId = c.Id;
    }

    [Fact]
    public async Task HappyPath()
    {
        // act
        // get timeslots for client
        var response = await apiFixture.Client
            .GetFromJsonAsync<TimeSlot[]>($"Clients/{clientId}/timeslots");

        response.Should().NotBeNull();
        response.Should().Contain(x => x.PsychologistId == psychologist1Id);
        response.Should().Contain(x => x.PsychologistId == psychologist2Id);
        response.Should().HaveCount(32); // 8 hours x 2 slots/hour x 2 therapists = 32

        // now try to book the appointment
        // get a random timeslot
        var toBook = response[Random.Shared.Next(0, response.Length - 1)];

        var bookResponse = await apiFixture
            .Client
            .PostAsJsonAsync($"/Clients/{clientId}/appointments", toBook);

        bookResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CannotBookAppointmentWithNotMyPsychologist()
    {
        // act
        // get timeslots for client
        var response = await apiFixture.Client
            .GetFromJsonAsync<TimeSlot[]>($"Clients/{clientId}/timeslots");

        // now try to book the appointment
        // get a random timeslot
        var toBook = response[Random.Shared.Next(0, response.Length - 1)];

        // modify the psychologistId to a random id
        toBook.PsychologistId = Random.Shared.Next(100_000, 200_000);

        // try to book it
        var bookResponse = await apiFixture
            .Client
            .PostAsJsonAsync($"/Clients/{clientId}/appointments", toBook);

        // assert that the request is rejected
        bookResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
