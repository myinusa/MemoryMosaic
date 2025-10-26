using System.Text;
using Serilog;

namespace MemoryMosaic.Scanner;

public class HelperScanner {
    private static readonly ILogger ScanLogger = Log.ForContext<HelperScanner>();

    public static void WriteclassNameContainersToCsv(Dictionary<string, List<ClassNameContainer>> rttiClassNames, string fileName) {
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileNameWithTimestamp = $"{timestamp}-{fileName}";

        string projectRoot = Directory.GetParent(Environment.CurrentDirectory)?.FullName
            ?? Environment.CurrentDirectory;
        string filePath = Path.Combine(projectRoot, fileNameWithTimestamp);

        using var writer = new StreamWriter(filePath);
        // Write the header line
        writer.WriteLine("Name,Address,Pattern");

        foreach (var pair in rttiClassNames) {
            var representative = GetDistinctClassNameContainers(pair.Value)
                .OrderBy(container => container.Address)
                .FirstOrDefault();

            if (representative is null) {
                continue;
            }

            writer.WriteLine(
                $"{FormatCsvField(pair.Key)},{FormatCsvField(representative.Address.ToString("X"))},{representative.PatternValue}");
        }
    }

    public static void WriteKeyValuePairsToCsv(Dictionary<string, List<AddressContainer>> keyValuePairs, string fileName) {
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileNameWithTimestamp = $"{timestamp}-{fileName}";

        string projectRoot = Directory.GetParent(Environment.CurrentDirectory)?.FullName
            ?? Environment.CurrentDirectory;
        string filePath = Path.Combine(projectRoot, fileNameWithTimestamp);

        using var writer = new StreamWriter(filePath);
        // Write the header line
        writer.WriteLine("Name,BaseAddress,PointerAddress,EndModuleAddress,NoOfAddresses");

        foreach (var pair in keyValuePairs) {
            foreach (var container in pair.Value) {
                // Write each record
                writer.WriteLine(
                    $"{FormatCsvField(container.Name)},{FormatCsvField(container.InitialAddress.ToString("X"))},{FormatCsvField(container.AddressValue.ToString("X"))},{FormatCsvField(container.EndAddress.ToString("X"))},{FormatCsvField(container.NoOfAddresses.ToString())}");
            }
        }
    }

    public static void RemoveDuplicateAddressValues(Dictionary<string, List<AddressContainer>> keyValuePairs) {
        ScanLogger.Information("Removing duplicate entries based on AddressValue");
        // Dictionary to keep track of seen AddressValues and their corresponding keys
        var seenAddressValues = new Dictionary<nuint, string>();

        // List to store keys of keyValuePairs that need to be removed
        var keysToRemove = new List<string>();

        foreach (var pair in keyValuePairs) {
            foreach (var container in pair.Value) {
                if (seenAddressValues.ContainsKey(container.AddressValue)) {
                    // If we have seen this AddressValue before, mark the previous key for removal
                    keysToRemove.Add(seenAddressValues[container.AddressValue]);
                    // Also mark the current key for removal to ensure all duplicates are removed
                    keysToRemove.Add(pair.Key);
                }
                else {
                    // Otherwise, add the AddressValue and its key to the seen dictionary
                    seenAddressValues.Add(container.AddressValue, pair.Key);
                }
            }
        }

        // Remove duplicates from keyValuePairs
        foreach (var key in keysToRemove.Distinct()) {
            keyValuePairs.Remove(key);
        }

        ScanLogger.Information("Removed {Count} duplicate entries based on AddressValue.", keysToRemove.Distinct().Count());
    }

    private static IEnumerable<ClassNameContainer> GetDistinctClassNameContainers(IEnumerable<ClassNameContainer> containers) {
        var seen = new HashSet<ulong>();

        foreach (var container in containers) {
            if (seen.Add(container.Address)) {
                yield return container;
            }
        }
    }

    private static string FormatCsvField(string? value) {
        string sanitized = SanitizeForCsv(value);

        if (sanitized.IndexOfAny(new[] { '"', ',', '\n', '\r' }) >= 0) {
            sanitized = sanitized.Replace("\"", "\"\"");
            return $"\"{sanitized}\"";
        }

        return sanitized;
    }

    private static string SanitizeForCsv(string? value) {
        if (string.IsNullOrEmpty(value)) {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);

        foreach (char character in value) {
            if (character >= ' ' && character <= '~') {
                builder.Append(character);
            }
        }

        return builder.Length == 0 ? string.Empty : builder.ToString();
    }
}