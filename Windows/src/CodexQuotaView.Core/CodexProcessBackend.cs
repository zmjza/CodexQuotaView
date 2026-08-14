using System.Diagnostics;
using System.Text;

namespace CodexQuotaView.Core;

public abstract class CodexProcessBackend
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(45);
    private const int MaximumResponseBytes = 8 * 1024 * 1024;

    protected abstract (string FileName, string Arguments) BuildInvocation();

    public async Task<QuotaSnapshot> FetchAsync(CancellationToken cancellationToken)
    {
        var invocation = BuildInvocation();
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = invocation.FileName,
                Arguments = invocation.Arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            },
        };

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(DefaultTimeout);

        var output = new MemoryStream();
        try
        {
            process.Start();
            var readTask = ReadBoundedAsync(process.StandardOutput.BaseStream, output, MaximumResponseBytes, cts.Token);
            try
            {
                var inputStream = process.StandardInput.BaseStream;
                await inputStream.WriteAsync(
                    System.Text.Encoding.UTF8.GetBytes("{\"method\":\"account/rateLimits/read\"}\n"),
                    cts.Token);
                await inputStream.WriteAsync(
                    System.Text.Encoding.UTF8.GetBytes("{\"method\":\"account/usage/read\"}\n"),
                    cts.Token);
                await inputStream.FlushAsync(cts.Token);
            }
            catch (IOException)
            {
                // The local app server may close stdin; the read task still determines success.
            }
            process.StandardInput.Close();
            await readTask;
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Kill(process);
            throw;
        }
        catch (Exception)
        {
            Kill(process);
            throw;
        }
        finally
        {
            if (!process.HasExited)
            {
                Kill(process);
            }
        }

        var text = Encoding.UTF8.GetString(output.ToArray());
        var snapshot = QuotaSnapshotDecoder.TryDecode(Encoding.UTF8.GetBytes(text));
        if (snapshot is null)
        {
            throw new InvalidDataException("app-server returned no decodable quota snapshot");
        }
        return snapshot;
    }

    private static async Task ReadBoundedAsync(
        Stream stream,
        MemoryStream destination,
        int maximumBytes,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[65536];
        while (destination.Length <= maximumBytes)
        {
            var read = await stream.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                return;
            }
            destination.Write(buffer, 0, read);
            if (destination.Length > maximumBytes)
            {
                throw new InvalidDataException("app-server response exceeds size bound");
            }
        }
    }

    private static void Kill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
    }
}
