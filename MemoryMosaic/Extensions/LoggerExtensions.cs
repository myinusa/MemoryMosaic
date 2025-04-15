using System.Runtime.CompilerServices;
using Serilog;

// https://stackoverflow.com/questions/29470863/serilog-output-enrich-all-messages-with-methodname-from-which-log-entry-was-ca
namespace MemoryMosaic.Extensions;

public static class LoggerExtensions {
    public static ILogger Here(
        this ILogger logger,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) {
        return logger
            .ForContext("MemberName", memberName)
            .ForContext("FilePath", sourceFilePath)
            .ForContext("LineNumber", sourceLineNumber);
    }
}