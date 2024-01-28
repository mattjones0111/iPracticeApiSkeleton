using Xunit;

namespace iPractice.IntegrationTests;

[CollectionDefinition("Api collection")]
public class ApiCollectionFixture : ICollectionFixture<ApiFixture>
{
}