using System.Globalization;

namespace Osmi;

internal static class Logger {
	internal static readonly SimpleLogger logger = new(nameof(Osmi));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Log(string message) => logger.Log(message);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LogDebug(string message) =>
#if DEBUG
		logger.Log(message);
#else
		logger.LogDebug(message);
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LogError(string message) => logger.LogError(message);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LogFine(string message) =>
#if DEBUG
		logger.Log(message);
#else
		logger.LogFine(message);
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LogWarn(string message) =>
#if DEBUG
		logger.LogError(message);
#else
		logger.LogWarn(message);
#endif
}
