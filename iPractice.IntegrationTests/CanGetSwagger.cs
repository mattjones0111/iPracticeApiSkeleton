using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace iPractice.IntegrationTests;

public class CanGetSwagger
{
    readonly HttpClient client;

    public CanGetSwagger()
    {
        ApiFixture fixture = new();
        client = fixture.CreateClient();
    }

    [Fact]
    public async Task Test()
    {
        var response = await client.GetAsync("/swagger");

        Assert.Multiple(() =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = response.Content.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            body.Should().Contain("<div id=\"swagger-ui\">");
        });
    }
}