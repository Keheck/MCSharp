namespace MCSharp.Core;

class EnvParser {
    public static void LoadEnvIntoEnvironmentVariables() {
        LoadEnvIntoEnvironmentVariables(null);
    }

    public static void LoadEnvIntoEnvironmentVariables(string? location) {
        foreach(KeyValuePair<string, string> kvp in LoadEnvIntoDictionary(location)) {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value == "" ? null : kvp.Value);
        }
    }

    public static Dictionary<string, string> LoadEnvIntoDictionary() {
        return LoadEnvIntoDictionary(null);
    }

    public static Dictionary<string, string> LoadEnvIntoDictionary(string? location) {
        Dictionary<string, string> env = new Dictionary<string, string>();
        FileInfo envFile = new FileInfo(Path.Combine(location ?? Directory.GetCurrentDirectory(), ".env"));
        StreamReader stream = envFile.OpenText();
        string content = stream.ReadToEnd();
        stream.Close();

        if(content.Last() != '\n')
            content += '\n';

        string key = "";
        string value = "";
        bool writingKey = true;

        foreach(char c in content) {
            if(writingKey) {
                if(c != '=')
                    key += c;
                else
                    writingKey = false;
            }
            else {
                if(c != '\n')
                    value += c;
                else {
                    writingKey = true;
                    env[key] = value;
                    key = "";
                    value = "";
                }
            }
        }

        return env;
    }
}