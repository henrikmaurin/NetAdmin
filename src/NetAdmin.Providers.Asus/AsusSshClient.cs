using System.Threading.Tasks;

namespace NetAdmin.Providers.Asus;

/// <summary>
/// Minimal SSH helper using Renci.SshNet. This enables executing commands on devices that
/// expose SSH. The project must reference Renci.SshNet NuGet package for this to compile.
/// </summary>
public static class AsusSshClient
{
    public static Task<string?> ExecuteCommandAsync(string host, int port, string username, string password, string command, int timeoutSeconds = 5)
    {
        return Task.Run(() =>
        {
            try
            {
                // Renci.SshNet ensure package reference in project
                using var client = new Renci.SshNet.SshClient(host, port, username, password);
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                client.Connect();
                if (!client.IsConnected) return null;
                var cmd = client.CreateCommand(command);
                cmd.CommandTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                var result = cmd.Execute();
                client.Disconnect();
                return result;
            }
            catch
            {
                return null;
            }
        });
    }
}
