using FluentAssertions;
using Travellers.Support.Db;

namespace TravellersTest.Architecture;

public class DatabaseAccessArchitectureTests
{
    [Fact]
    public void ShouldOnlyLetDatabaseExecutorDependOnTravellersDbContext()
    {
        var typesDependingOnDbContextDirectly = typeof(Program).Assembly
            .GetTypes()
            .Where(type => type != typeof(DatabaseExecutor))
            .Where(type => type.GetConstructors()
                .SelectMany(ctor => ctor.GetParameters())
                .Any(parameter => parameter.ParameterType == typeof(TravellersDbContext)))
            .ToList();

        typesDependingOnDbContextDirectly.Should().BeEmpty(
            "only DatabaseExecutor may depend on TravellersDbContext - everything " +
            "else must go through DatabaseExecutor.ExecuteAsync to get resilience");
    }
}
