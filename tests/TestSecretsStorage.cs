using Microsoft.AspNetCore.DataProtection;
using RunJS;

namespace tests;

public class TestSecretsStorage : IClassFixture<DatabaseFixture>
{
    private SecretsService CreateSecretsService()
    {
        // Create an ephemeral data protection provider for testing
        var dataProtectionProvider = new EphemeralDataProtectionProvider();
        var protector = dataProtectionProvider.CreateProtector("RunJS.Values");
        return new SecretsService(protector);
    }

    [Fact]
    public void Can_Encrypt_And_Decrypt_Value()
    {
        var secretsService = CreateSecretsService();
        var originalValue = "This is a secret value";

        var encrypted = secretsService.Encrypt(originalValue);
        var decrypted = secretsService.Decrypt(encrypted);

        Assert.NotEqual(originalValue, encrypted);
        Assert.Equal(originalValue, decrypted);
    }

    [Fact]
    public void Encrypted_Values_Are_Different_Each_Time()
    {
        var secretsService = CreateSecretsService();
        var originalValue = "Same secret value";

        var encrypted1 = secretsService.Encrypt(originalValue);
        var encrypted2 = secretsService.Encrypt(originalValue);

        Assert.NotEqual(encrypted1, encrypted2);
        Assert.Equal(originalValue, secretsService.Decrypt(encrypted1));
        Assert.Equal(originalValue, secretsService.Decrypt(encrypted2));
    }

    [Fact]
    public async Task Can_Save_And_Retrieve_Secret()
    {
        var secretsService = CreateSecretsService();
        var originalValue = "This is a secret value";

        var db = DatabaseFixture.CreateDbContext();

        using var tx = await db.Database.BeginTransactionAsync();

        var encrypted = secretsService.Encrypt(originalValue);

        await db.Secrets.AddAsync(
            new RunJS.Secret { Id = "test", EncryptedValue = encrypted }
        );

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var secret = await db.Secrets.FindAsync("test");

        Assert.NotNull(secret);
        Assert.Equal("test", secret.Id);
        Assert.Equal(encrypted, secret.EncryptedValue);
        Assert.Equal(originalValue, secretsService.Decrypt(secret.EncryptedValue));
    }
}
