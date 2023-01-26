using CommandLine;

namespace MCSharp.Options;

[Verb("build", true)]
class CompileOptions {
    /* Directory that is used as the root directory of the project. If omitted, the current working directory is used */
    [Option('i', "input-directory")]
    public string? InputFile { get; set; }

    /* If set, the compiler will print warnings to the standard output. Set by default */
    [Option('w', "log-warnings")]
    public bool Warn { get; set; } = true;
}