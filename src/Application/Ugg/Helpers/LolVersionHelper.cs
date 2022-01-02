using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Application.Ugg.Helpers;

public static class LolVersionHelper
{
    public static string GetPreviousVersion(string version)
    {
        var parts = version.Split(".");

        var major = int.Parse(parts[0]);
        var minor = int.Parse(parts[1]);

        if (minor == 1)
        {
            major--;
            minor = 24;
        }
        else
        {
            minor--;
        }

        return $"{major}.{minor}";
    }
        
    public static string GetPreviousVersion(List<string> versions)
    {
        var majorMinorVersions = versions.Where(v => !v.StartsWith("lolpatch")).Select(v =>
        {
            var parts = v.Split(".");

            var major = int.Parse(parts[0]);
            var minor = int.Parse(parts[1]);

            return $"{major}.{minor}";
        }).Distinct().ToList();

        var prev = majorMinorVersions[1];

        return versions.Find(v => v.StartsWith(prev));
    }
}