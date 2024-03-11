using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FintechMessageConsumer.Application
{
    [ExcludeFromCodeCoverage]
    public class AssemblyReference
    {
        public Assembly GetAssembly()
        {
            return GetType().Assembly;
        }
    }
}
