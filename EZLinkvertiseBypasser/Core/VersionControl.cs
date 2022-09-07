using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace EZLinkvertiseBypasser.Core
{
    internal class VersionControl
    {
        public async Task<string> GetGithubVersionAsync()
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            var client = new GitHubClient(new ProductHeaderValue("EZLinkvertiseBypasser"));
            var releases = await client.Repository.Release.GetAll("Glumboi", "EZLinkvertiseBypasser");

            //Setup the versions
            var latestGitHubVersion = new Version(releases[0].TagName);

            return latestGitHubVersion.ToString();
        }

        public async Task<bool> CheckGitHubNewerVersion()
        {
            //Get all releases from GitHub
            //Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            var client = new GitHubClient(new ProductHeaderValue("EZLinkvertiseBypasser"));
            var releases = await client.Repository.Release.GetAll("Glumboi", "EZLinkvertiseBypasser");

            //Setup the versions
            var latestGitHubVersion = new Version(releases[0].TagName);
            var Reference = typeof(Form1).Assembly;
            var Version = Reference.GetName().Version;
            var localVersion = new Version(Version.ToString()); //Replace this with your local version. 
            //Only tested with numeric values.

            //Compare the Versions
            //Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            var versionComparison = localVersion.CompareTo(latestGitHubVersion);
            if (versionComparison < 0)
            {
                //The version on GitHub is more up to date than this local release.
                return true;
            }
            else if (versionComparison > 0)
            {
                Debug.WriteLine("The local is higher than github");
                //This local version is greater than the release version on GitHub.
                return false;
            }
            else
            {
                Debug.WriteLine("The versions are equal");
                //This local Version and the Version on GitHub are equal.
                return false;
            }
        }

    }
}
