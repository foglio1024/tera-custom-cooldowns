using System.Collections.Generic;

namespace TCC.Publisher
{
    public class PublisherSettings
    {
        public string RepositoryName { get; set; }
        public string RepositoryOwner { get; set; }
        public string SevenZipLibPath { get; set; }
        public string LocalRepositoryPath { get; set; }
        public string DiscordWebhook { get; set; }
        public string GithubToken { get; set; }
        public string FirestoreUrl { get; set; }
        public List<string> ExcludedFiles { get; set; }
    }
}