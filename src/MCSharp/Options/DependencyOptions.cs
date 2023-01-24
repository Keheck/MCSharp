using CommandLine;
using System.Collections.Generic;

namespace MCSharp.Options;

[Verb("dependencies", aliases: new string[]{"dependency"})]
class DependencyOptions {
    [Option('a', "add")]
    public IEnumerable<string> DependenciesToAdd { get; set; } = Enumerable.Empty<string>();

    [Option('u', "upgrade")]
    public IEnumerable<string> DependenciesToUpgrade { get; set; } = Enumerable.Empty<string>();

    [Option('f', "fetch")]
    public bool Fetch { get; set; }

    [Option('r', "remove")]
    public IEnumerable<string> DependenciesToRemove { get; set; } = Enumerable.Empty<string>();

    [Option('t', "token")]
    public string AccessToken { get; set; } = "";

    [Option('i', "info")]
    public bool GetInformation { get; set; } = false;
}