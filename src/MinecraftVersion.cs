using System.Text.RegularExpressions;

/* This is a utility class for easy comparison of Minecraft version numbers and other things*/
class MinecraftVersion {
    public static readonly Regex VERSION_REGEX = new Regex(@"^\d+\.\d+(\.\d+)?$");

    private MinecraftVersion() {}

    public static int Compare(string version, string other) {
        if(!VERSION_REGEX.Match(version).Success || !VERSION_REGEX.Match(other).Success) 
            throw new ArgumentException("One or both provided versions did not match the conventional versioning system");
        
        int[] versionNumbers = version.Split('.').Select(s => int.Parse(s)).ToArray();
        int[] otherVersionNumbers = other.Split('.').Select(s => int.Parse(s)).ToArray();

        for(int i = 0; i < Math.Min(versionNumbers.Length, otherVersionNumbers.Length); i++) {
            if(versionNumbers[i] != otherVersionNumbers[i])
                return versionNumbers[i] - otherVersionNumbers[i];
        }

        return versionNumbers.Length - otherVersionNumbers.Length;
    }

    public static string getMinVersion(int format) {
        
    }
}