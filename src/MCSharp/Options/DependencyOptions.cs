using CommandLine;
using System.Collections.Generic;

namespace MCSharp.Options;

[Verb("dependencies", aliases: new string[]{"dependency"})]
class DependencyOptions {
    /* If set, the dependency manager will add one or multiple dependency references of {owner}:{repo}:{version} to the mcsharp.xml file and download the required assets to the dependency cache.
       {owner} is the username of the repository owner, {repo} is the name of the repository and {version} is either the target commit shorthash or the release tag */
    [Option('a', "add")]
    public IEnumerable<string> DependenciesToAdd { get; set; } = Enumerable.Empty<string>();

    /* If set, the dependency manager will look through the referenced repositories {owner}:{repo} and update them to the latest release, 
       or latest commit if no release can be found */
    [Option('u', "upgrade")]
    public IEnumerable<string> DependenciesToUpgrade { get; set; } = Enumerable.Empty<string>();

    /* If set, the dependency manager will remove unused dependencies from the cache and download missing dependencies as referenced in the mcsharp.xml file*/
    [Option('f', "fetch")]
    public bool Fetch { get; set; }

    /* If set, it removes the dependency {owner}:{repo} from the dependency cache and it's reference from the mcsharp.xml file */
    [Option('r', "remove")]
    public IEnumerable<string> DependenciesToRemove { get; set; } = Enumerable.Empty<string>();

    /* If set, this token will be used to access the GitHub API. It comes with the privilege of 5000 requests/hour instead of the 60 requests/hour for unauthenticated requests */
    [Option('t', "token")]
    public string AccessToken { get; set; } = "";

    /* If set, this one prints out ratelimit information to the user. Be aware that fetching ratelimit info in itself adds to the ratelimit counter. */
    [Option('i', "info")]
    public bool GetInformation { get; set; } = false;

    /* If set, this points to the location of the mcsharp.xml file. If omitted, the current working directory will be assumed. */
    [Option("sharp-dir")]
    public string? CompilerDirectory { get; set; }
}