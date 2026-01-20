using TFA.Storage.Entities;

namespace TFA.Storage.Tests;

public class SignInStorageFixture : StorageTestFixture
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await using var dbContext = GetDbContext();
        await dbContext.Users.AddRangeAsync(new User
        {
            UserId = Guid.Parse("0dd87f18-bc01-4be9-89cb-cb9ff9e12e3b"),
            Login = "testUser",
            PasswordHash = [1],
            Salt = [1]
        }, new User
        {
            UserId = Guid.Parse("a4edfda0-8b42-49d8-9e1a-2603a23446ec"),
            Login = "testUser2",
            PasswordHash = [1],
            Salt = [1]
        });
        await dbContext.SaveChangesAsync();
    }
}
