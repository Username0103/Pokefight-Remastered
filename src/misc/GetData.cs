using System.Reflection;

namespace src.misc
{
    public static class GetData
    {
        private static readonly string resourceName = "src.data.pokemon.yaml";

        public static void GetTheData()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream =
                assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileLoadException($"Could not find YAML reasources at {resourceName}");
            using var reader = new StreamReader(stream);
            var yamlContent = reader.ReadToEnd();
            // TODO: use data
        }
    }
}
