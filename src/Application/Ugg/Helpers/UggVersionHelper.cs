namespace LeagueOfItems.Application.Ugg.Helpers
{
    public static class UggVersionHelper
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
    }
}