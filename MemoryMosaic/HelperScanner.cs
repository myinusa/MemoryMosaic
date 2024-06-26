using Serilog;

namespace MemoryMosaic;

public class HelperScanner {
    private static readonly ILogger ScanLogger = Serilog.Log.ForContext<HelperScanner>();

    public static void WriteclassNameContainersToCsv(Dictionary<string, List<ClassNameContainer>> rttiClassNames, string fileName) {
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileNameWithTimestamp = $"{timestamp}-{fileName}";

        string projectRoot = Directory.GetParent(Environment.CurrentDirectory).FullName;
        string filePath = Path.Combine(projectRoot, fileNameWithTimestamp);

        using var writer = new StreamWriter(filePath);
        // Write the header line
        writer.WriteLine("Name,Address,Pattern");

        foreach (var pair in rttiClassNames) {
            foreach (var container in pair.Value) {
                // Write each record
                writer.WriteLine($"{pair.Key:X},{container.Address},{container.PatternValue}");
            }
        }
    }

    public static void WriteKeyValuePairsToCsv(Dictionary<string, List<AddressContainer>> keyValuePairs, string fileName) {
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileNameWithTimestamp = $"{timestamp}-{fileName}";

        string projectRoot = Directory.GetParent(Environment.CurrentDirectory).FullName;
        string filePath = Path.Combine(projectRoot, fileNameWithTimestamp);

        using var writer = new StreamWriter(filePath);
        // Write the header line
        writer.WriteLine("Name,BaseAddress,PointerAddress,EndModuleAddress,NoOfAddresses");

        foreach (var pair in keyValuePairs) {
            foreach (var container in pair.Value) {
                // Write each record
                writer.WriteLine($"{container.Name},{container.InitialAddress:X},{container.AddressValue:X},{container.EndAddress:X},{container.NoOfAddresses}");
            }
        }
    }

    public void RemoveDuplicateAddressValues(Dictionary<string, List<AddressContainer>> keyValuePairs) {
        ScanLogger.Information("Removing duplicate entries based on AddressValue");
        // Dictionary to keep track of seen AddressValues and their corresponding keys
        var seenAddressValues = new Dictionary<UIntPtr, string>();

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
}