namespace NetAdmin.Core.Services;

public interface IAdminAuthService
{
    Task<bool> IsAdminConfiguredAsync(CancellationToken ct = default);

    Task<bool> IsOnboardingCompletedAsync(CancellationToken ct = default);

    Task<string?> GetAdminUsernameAsync(CancellationToken ct = default);

    Task ConfigureAdminAsync(string username, string password, CancellationToken ct = default);

    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken ct = default);

    Task SetOnboardingCompletedAsync(bool completed = true, CancellationToken ct = default);
}
