using CommandLine;

namespace MCSharp.Options;

[Verb("build", true)]
class CompileOptions {
    /* Directory that is used as the root directory of the project. If omitted, the current working directory is used */
    [Option('i', "input-directory")]
    public string? InputFile { get; set; }

    /* If set, the compiler will not print warnings to the standard output. */
    [Option("quiet")]
    public bool Quiet { get; set; } = false;

    /* If set, the compiler will not implicitly add the standard library as a dependency */
    [Option("no-stdlib")]
    public bool NoStandardLib { get; set; } = false;
}