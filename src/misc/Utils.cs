using System.Reflection;

namespace Src.Misc
{
    public static class Utils
    {
        public static readonly Random Generator = new();
        public static readonly Assembly assembly =
            Assembly.GetExecutingAssembly()
            ?? throw new SystemException("Could not get execution assembly.");

        public static string[] GetResourcesWithEnding(string ending)
        {
            var names = assembly.GetManifestResourceNames();
            return [.. names.Where((n) => n.EndsWith(ending))];
        }
    }
}
