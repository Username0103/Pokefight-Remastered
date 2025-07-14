using System.Reflection;

namespace src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assembly =
                Assembly.GetExecutingAssembly()
                ?? throw new SystemException("Could not get execution assembly.");
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
                Console.WriteLine(name);
        }
    }
}
