using Microsoft.AspNetCore.DataProtection;

namespace RunJS;

public static class SetupEncryptionExtension
{
    /// <summary>
    /// Set up the data protection sub-system for secure encryption of the
    /// secret values stored in the database.
    /// </summary>
    /// <param name="services"></param>
    public static void AddEncryption(this IServiceCollection services)
    {
        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("./.keys"))
            .SetApplicationName("runjs-mcp-server");

        services.AddSingleton(provider =>
        {
            var dataProtectionProvider =
                provider.GetRequiredService<IDataProtectionProvider>();
            var protector = dataProtectionProvider.CreateProtector("RunJS.Values");
            return new EncryptionService(protector);
        });
    }
}
