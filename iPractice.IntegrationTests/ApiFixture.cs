using System.Net.Http;
using System.Threading.Tasks;
using iPractice.Api;
using iPractice.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace iPractice.IntegrationTests;

public sealed class ApiFixture : WebApplicationFactory<Startup>, IAsyncLifetime
{
    public HttpClient Client { get; private set; }

    public ApplicationDbContext Db { get; private set; }

    public Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=iPractice.db;Cache=Shared")
            .Options;

        Db = new ApplicationDbContext(options);

        Client = CreateClient();
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (Db != null)
        {
            await Db.DisposeAsync();
        }
    }
}
