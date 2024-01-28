using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace iPractice.IntegrationTests;

[Collection("Api collection")]
public class CanGetSwagger
{
    readonly ApiFixture apiFixture;

    public CanGetSwagger(ApiFixture apiFixture)
    {
        this.apiFixture = apiFixture;
    }

    [Fact]
    public async Task Test()
    {
        var response = await apiFixture.Client.GetAsync("/swagger");

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