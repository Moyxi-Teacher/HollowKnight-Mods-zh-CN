using Modding;
using System.IO;
using System.Runtime.CompilerServices;

namespace ConnectionMetadataInjector.Util
{
    internal static class ScopedLoggers
    {
        public static Loggable GetLogger([CallerFilePath] string callingFile = "")
        {
            return new SimpleLogger($"ConnectionMetadataInjector.{Path.GetFileNameWithoutExtension(callingFile)}");
        }
    }
}
