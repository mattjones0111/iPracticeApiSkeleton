using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using iPractice.Api.Models;
using Xunit;

namespace iPractice.IntegrationTests;

public class CanGetTimeslotsForClient
{
    readonly HttpClient client;

    public CanGetTimeslotsForClient()
    {
        ApiFixture api = new();
        client = api.CreateClient();
    }

    [Fact]
    public async Task Test()
    {
        var actual = await client.GetFromJsonAsync<TimeSlot[]>("/Clients/1/timeslots");

        actual.Should().BeEmpty();
    }
}
