using Microsoft.AspNetCore.DataProtection;

namespace RunJS;

public static class SetupEncryptionExtension
{
    public static void AddEncryption(this IServiceCollection services)
    {
        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("./.keys"))
            .SetApplicationName("runjs-mcp-server");

        services.AddSingleton<SecretsService>(provider =>
        {
            var dataProtectionProvider =
                provider.GetRequiredService<IDataProtectionProvider>();
            var protector = dataProtectionProvider.CreateProtector("RunJS.Values");
            return new SecretsService(protector);
        });
    }
}
