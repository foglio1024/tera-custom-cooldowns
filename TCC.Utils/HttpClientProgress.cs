using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TCC.Utils;

public class HttpClientProgress : HttpClient
{
    public event Action<long, long>? DownloadProgressChanged;
    public event Action<bool>? DownloadFileCompleted;

    public HttpClientProgress()
    {
        DefaultRequestHeaders.ExpectContinue = false;
    }

    public async Task DownloadFileAsync(Uri uri, string destinationPath)
    {
        try
        {
            var resp = await GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            var file = File.OpenWrite(destinationPath);
            var stream = await resp.Content.ReadAsStreamAsync();
            var totalLen = resp.Content.Headers.ContentLength;
            var buffer = new byte[4096];
            var totalBytesRead = 0;
            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer);

                if (bytesRead == 0) break;

                totalBytesRead += bytesRead;
                ReportProgress(totalLen, totalBytesRead);

                Console.WriteLine($"Read {totalBytesRead} bytes");
                await file.WriteAsync(buffer.AsMemory(0, bytesRead));
            }

            await stream.DisposeAsync();
            await file.DisposeAsync();
        }
        catch
        {
            DownloadFileCompleted?.Invoke(false);
        }

        DownloadFileCompleted?.Invoke(true);
    }

    private void ReportProgress(long? totalLen, int totalBytesRead)
    {
        DownloadProgressChanged?.Invoke(totalBytesRead, totalLen ?? -1);
    }
}
