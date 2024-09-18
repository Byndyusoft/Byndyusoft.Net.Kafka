namespace Byndyusoft.Net.Kafka.Infrastructure
{
    using System.Linq;
    using CaseExtensions;

    internal static class SolutionNameExtensions
    {
        internal static (string projectName, string serviceName) ExtractProjectAndServiceNames(this string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var projectName = solutionNameParts[1].ToSnakeCase();
            var serviceName = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return (projectName, serviceName);
        }
    }
}