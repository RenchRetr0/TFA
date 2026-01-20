using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using TFA.Storage.Storages;

namespace TFA.Storage.Tests;

public class SignInStorageShould : IClassFixture<SignInStorageFixture>
{
    private readonly SignInStorageFixture fixture;
    private readonly SignInStorage sut;

    public SignInStorageShould(SignInStorageFixture fixture)
    {
        this.fixture = fixture;
        sut = new SignInStorage(
            new GuidFactory(),
            this.fixture.GetDbContext(),
            this.fixture.GetMapper());
    }

    [Fact]
    public async Task ReturnUser_WhenDatabaseContainsUserWhitSameLogin()
    {
        var actual = await sut.FindUser("testUser", CancellationToken.None);
        actual.Should().NotBeNull();
        actual.UserId.Should().Be(Guid.Parse("0dd87f18-bc01-4be9-89cb-cb9ff9e12e3b"));
    }

    [Fact]
    public async Task ReturnNull_WhenDatabaseDoesntContainUserWithSameLogin()
    {
        var actual = await sut.FindUser("whatever", CancellationToken.None);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task ReturnNewlyCreatedSessionId()
    {
        var sessionId = await sut.CreateSession(
            Guid.Parse("0dd87f18-bc01-4be9-89cb-cb9ff9e12e3b"),
            new DateTimeOffset(2026, 01, 20, 03, 15, 00, TimeSpan.Zero),
            CancellationToken.None
        );

        await using var dbContext = fixture.GetDbContext();
        (await dbContext.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId)).Should().NotBeNull();
    }
}
