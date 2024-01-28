using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using iPractice.DataAccess.Models;
using Xunit;

namespace iPractice.IntegrationTests;

[Collection("Api collection")]
public class CanCreateAvailability
{
    private readonly ApiFixture apiFixture;

    public CanCreateAvailability(ApiFixture apiFixture)
    {
        this.apiFixture = apiFixture;
    }

    [Fact]
    public async Task HappyPathTest()
    {
        // arrange
        // seed a psychologist
        Psychologist p = new()
        {
            Name = "integration-test"
        };

        apiFixture.Db.Psychologists.Add(p);

        await apiFixture.Db.SaveChangesAsync();

        // add new availability
        Availability newAvailability = new()
        {
            Start = new DateTime(2024, 1, 1, 9, 0, 0),
            End = new DateTime(2024, 1, 1, 17, 0, 0)
        };

        // act
        HttpResponseMessage response =
            await apiFixture.Client.PostAsJsonAsync(
                $"/Psychologists/{p.Id}/availability",
                newAvailability);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // TODO the 'Created' body resposne should contain a
        // URL of the new resource.
    }

    [Fact]
    public async Task CannotCreateAvailabilityForUnknownPsychologist()
    {
        long id = Random.Shared.Next(1000, int.MaxValue);

        // add new availability
        Availability newAvailability = new()
        {
            Start = new DateTime(2024, 1, 1, 9, 0, 0),
            End = new DateTime(2024, 1, 1, 17, 0, 0)
        };

        // act
        HttpResponseMessage response =
            await apiFixture.Client.PostAsJsonAsync(
                $"/Psychologists/{id}/availability",
                newAvailability);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CannotCreateAvailabilityWhichStartsBeforeItEnds()
    {
        // arrange
        // seed a psychologist
        Psychologist p = new()
        {
            Name = "integration-test"
        };

        apiFixture.Db.Psychologists.Add(p);

        await apiFixture.Db.SaveChangesAsync();

        Availability newAvailability = new()
        {
            Start = new DateTime(2024, 1, 1, 17, 0, 0),
            End = new DateTime(2024, 1, 1, 9, 0, 0)
        };

        // act
        HttpResponseMessage response =
            await apiFixture.Client.PostAsJsonAsync(
                $"/Psychologists/{p.Id}/availability",
                newAvailability);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
