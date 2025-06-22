using Microsoft.AspNetCore.DataProtection;
using RunJS;

namespace tests;

public class TestSecretsStorage : IClassFixture<DatabaseFixture>
{
    private EncryptionService CreateEncryptionService()
    {
        // Create an ephemeral data protection provider for testing
        var dataProtectionProvider = new EphemeralDataProtectionProvider();
        var protector = dataProtectionProvider.CreateProtector("RunJS.Values");
        return new EncryptionService(protector);
    }

    [Fact]
    public void Can_Encrypt_And_Decrypt_Value()
    {
        var encryptionService = CreateEncryptionService();
        var originalValue = "This is a secret value";

        var encrypted = encryptionService.Encrypt(originalValue);
        var decrypted = encryptionService.Decrypt(encrypted);

        Assert.NotEqual(originalValue, encrypted);
        Assert.Equal(originalValue, decrypted);
    }

    [Fact]
    public void Encrypted_Values_Are_Different_Each_Time()
    {
        var encryptionService = CreateEncryptionService();
        var originalValue = "Same secret value";

        var encrypted1 = encryptionService.Encrypt(originalValue);
        var encrypted2 = encryptionService.Encrypt(originalValue);

        Assert.NotEqual(encrypted1, encrypted2);
        Assert.Equal(originalValue, encryptionService.Decrypt(encrypted1));
        Assert.Equal(originalValue, encryptionService.Decrypt(encrypted2));
    }

    [Fact]
    public async Task Can_Save_And_Retrieve_Secret()
    {
        var encryptionService = CreateEncryptionService();
        var originalValue = "This is a secret value";

        var db = DatabaseFixture.CreateDbContext();

        using var tx = await db.Database.BeginTransactionAsync();

        var encrypted = encryptionService.Encrypt(originalValue);

        await db.Secrets.AddAsync(
            new RunJS.Secret { Id = "test", EncryptedValue = encrypted }
        );

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var secret = await db.Secrets.FindAsync("test");

        Assert.NotNull(secret);
        Assert.Equal("test", secret.Id);
        Assert.Equal(encrypted, secret.EncryptedValue);
        Assert.Equal(
            originalValue,
            encryptionService.Decrypt(secret.EncryptedValue)
        );
    }

    [Fact]
    public async Task Secrets_Are_Deleted_After_Reading_Once()
    {
        var encryptionService = CreateEncryptionService();
        var originalValue = "This is a secret value";

        var db = DatabaseFixture.CreateDbContext();

        using var tx = await db.Database.BeginTransactionAsync();

        var secretsService = new DbSecretsService(encryptionService, db);

        var secretId = await secretsService.Store(originalValue, true);

        db.ChangeTracker.Clear();

        var decryptedValue = await secretsService.Retrieve(secretId);

        Assert.Equal(originalValue, decryptedValue);

        db.ChangeTracker.Clear();

        var secret = await db.Secrets.FindAsync(secretId);

        Assert.Null(secret); // Secret should be deleted after reading once
    }
}
