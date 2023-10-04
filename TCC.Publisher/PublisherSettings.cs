using System.Collections.Generic;

namespace TCC.Publisher;

public class PublisherSettings
{
    /// <summary>
    /// Name of the repository (eg. Tera-custom-cooldowns)
    /// </summary>
    public string RepositoryName { get; set; } = "";

    /// <summary>
    /// Username of repository owner (eg. Foglio1024)
    /// </summary>
    public string RepositoryOwner { get; set; } = "";

    /// <summary>
    /// Path to 7z.dll (eg. C:\Program Files\7-Zip\7z.dll)
    /// </summary>
    public string SevenZipLibPath { get; set; } = "";

    /// <summary>
    /// Path to local repository (eg. D:\Repos\TCC)
    /// </summary>
    public string LocalRepositoryPath { get; set; } = "";

    /// <summary>
    /// Webhook used for #update channel
    /// </summary>
    public string DiscordWebhook { get; set; } = "";

    /// <summary>
    /// Token generated from GitHub
    /// </summary>
    public string GithubToken { get; set; } = "";

    /// <summary>
    /// Url to http activator of update_version function
    /// </summary>
    public string FirestoreUrl { get; set; } = "";
    /// <summary>
    /// File names to not include in final zip
    /// </summary>
    public List<string> ExcludedFiles { get; set; } = new List<string>();
}