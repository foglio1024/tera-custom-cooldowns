using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TCC.Utilities;
public static class HttpClientExtensions
{
    public static async Task DownloadFileAsync(this HttpClient client, string url, string path)
    {
        var resp = await client.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync(path, resp);
    }
}
