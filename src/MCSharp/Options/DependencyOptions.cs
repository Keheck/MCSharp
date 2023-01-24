using CommandLine;
using System.Collections.Generic;

namespace MCSharp.Options;

[Verb("dependencies")]
class DependencyOptions {
    [Option('a', "add")]
    public IEnumerable<string> DependenciesToAdd { get; set; } = Enumerable.Empty<string>();

    [Option('u', "upgrade")]
    public IEnumerable<string> DependenciesToUpgrade { get; set; } = Enumerable.Empty<string>();

    [Option('f', "fetch")]
    public bool Fetch { get; set; }

    [Option('r', "remove")]
    public IEnumerable<string> DependenciesToRemove { get; set; } = Enumerable.Empty<string>();
}