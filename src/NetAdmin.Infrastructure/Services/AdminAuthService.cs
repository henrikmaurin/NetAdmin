using System.Security.Cryptography;
using NetAdmin.Core.Services;

namespace NetAdmin.Infrastructure.Services;

public class AdminAuthService(IEncryptedSettingsService settingsService) : IAdminAuthService
{
    private const string UsernameKey = "System:Admin:Username";
    private const string PasswordHashKey = "System:Admin:PasswordHash";
    private const string PasswordSaltKey = "System:Admin:PasswordSalt";
    private const string OnboardingCompletedKey = "System:Onboarding:Completed";
    private const int Iterations = 100_000;
    private const int KeySize = 32;

    public async Task<bool> IsAdminConfiguredAsync(CancellationToken ct = default)
    {
        var username = await settingsService.GetSettingAsync(UsernameKey, ct: ct);
        var hash = await settingsService.GetSettingAsync(PasswordHashKey, ct: ct);
        return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(hash);
    }

    public async Task<bool> IsOnboardingCompletedAsync(CancellationToken ct = default)
    {
        if (!await IsAdminConfiguredAsync(ct))
            return false;

        var completed = await settingsService.GetSettingAsync(OnboardingCompletedKey, ct: ct);
        return string.Equals(completed, "true", StringComparison.OrdinalIgnoreCase);
    }

    public Task<string?> GetAdminUsernameAsync(CancellationToken ct = default) =>
        settingsService.GetSettingAsync(UsernameKey, ct: ct);

    public async Task ConfigureAdminAsync(string username, string password, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        await settingsService.SaveSettingAsync(UsernameKey, username.Trim(), ct);
        await settingsService.SaveSettingAsync(PasswordSaltKey, Convert.ToBase64String(salt), ct);
        await settingsService.SaveSettingAsync(PasswordHashKey, Convert.ToBase64String(hash), ct);
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var savedUsername = await settingsService.GetSettingAsync(UsernameKey, ct: ct);
        var saltBase64 = await settingsService.GetSettingAsync(PasswordSaltKey, ct: ct);
        var hashBase64 = await settingsService.GetSettingAsync(PasswordHashKey, ct: ct);

        if (string.IsNullOrWhiteSpace(savedUsername) ||
            string.IsNullOrWhiteSpace(saltBase64) ||
            string.IsNullOrWhiteSpace(hashBase64))
        {
            return false;
        }

        if (!string.Equals(savedUsername.Trim(), username.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            var salt = Convert.FromBase64String(saltBase64);
            var expectedHash = Convert.FromBase64String(hashBase64);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch
        {
            return false;
        }
    }

    public Task SetOnboardingCompletedAsync(bool completed = true, CancellationToken ct = default) =>
        settingsService.SaveSettingAsync(OnboardingCompletedKey, completed ? "true" : "false", ct);
}
