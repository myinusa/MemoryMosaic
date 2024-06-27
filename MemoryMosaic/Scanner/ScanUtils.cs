using System.Diagnostics;
using MemoryMosaic.Extensions;
using Serilog;

namespace MemoryMosaic.Scanner;

public abstract class ScanUtils {
    private const double PercentageTotal = 100.0;
    private const int ProgressReportIntervalPercentage = 5;
    private const int PercentageFactor = 100;

    private static readonly ILogger Logger = Log.ForContext<ModuleScanner>();

    public static void LogError(Exception ex, string moduleName) {
        Logger.Error(ex, "Exception occurred while scanning {ModuleName}", moduleName);
    }

    internal static int CalculateProgressInterval(int numberOfElements) {
        return numberOfElements * ProgressReportIntervalPercentage / PercentageFactor;
    }

    public static void LogProgress(int iterationCount, int totalElements, Stopwatch stopwatch, Dictionary<string, List<ClassNameContainer>> rttiClassNameList) {
        Logger.Here().Information("{IterationCount:N0}/{TotalElements:N0} addresses ({PercentageComplete:F2}%) | Elapsed: {Elapsed:mm\\:ss}", iterationCount, totalElements, iterationCount * PercentageTotal / totalElements, stopwatch.Elapsed);
        Logger.Information("Classes Found: {RTTIClassCount} | Distinct Classes: {RTTIDistinct}", rttiClassNameList.Count, rttiClassNameList.Distinct().Count());
        // Logger.Information("Keys Found: {KeyCount} | Distinct Keys: {KeyDistinct}", keyValuePairs.Count, keyValuePairs.Keys.Distinct().Count());
    }

    public static void LogScanStart(int moduleSize, int numberOfElements) {
        Logger.Here().Information("Module Size: {ModuleSize} bytes", moduleSize);
        Logger.Here().Information("Number of elements: {NumberOfElements:N0}", numberOfElements);
    }

    public static void LogScanCompletion(Stopwatch stopwatch) {
        stopwatch.Stop();
        Logger.Here().Information("Completed scanning {ModuleName}. Time taken: {TimeTaken}", string.Empty, stopwatch.Elapsed);
    }
}