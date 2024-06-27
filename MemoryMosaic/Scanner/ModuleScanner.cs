using System.Diagnostics;
using Processory;
using Serilog;

namespace MemoryMosaic.Scanner;

public interface IModuleScanner {
    void ScanModulesForRTTIClassNames();
}

public class AddressContainer {
    public string? Name { get; internal set; }
    public nuint InitialAddress { get; set; }
    public nuint AddressValue { get; set; }
    public nuint EndAddress { get; internal set; }
    public int NoOfAddresses { get; internal set; }
}

public class ClassNameContainer {
    public ulong Address { get; set; }
    public uint PatternValue { get; set; }
}

public class ModuleScanner : ScanUtils, IModuleScanner {
    private const int PointerSize = sizeof(int);
    private static readonly ILogger ScanLogger = Log.ForContext<ModuleScanner>();
    private readonly Dictionary<string, List<ClassNameContainer>> rttiClassNames = new();
    private readonly Dictionary<string, List<AddressContainer>> keyValuePairs = new();
    public ProcessoryClient ProcessoryClient { get; set; }

    public ModuleScanner(ProcessoryClient processoryClient) {
        ProcessoryClient = processoryClient;
    }

    public void ScanModulesForRTTIClassNames() {
        EnsureValidProcessHandle();

        PerformModuleScan();
    }

    private void EnsureValidProcessHandle() {
        if (ProcessoryClient.ProcessHandle == nint.Zero) {
            throw new InvalidOperationException("Invalid Process Handle");
        }
    }

    private void PerformModuleScan() {
        //int moduleSize = CalculateModuleSize(module);
        int moduleSize = ProcessoryClient.ProcessService.GetModuleSize();
        int elementCount = moduleSize / PointerSize;
        int progressUpdateInterval = CalculateProgressInterval(elementCount);

        LogScanStart(moduleSize, elementCount);

        var stopwatch = Stopwatch.StartNew();
        ScanMemoryAddresses(elementCount, progressUpdateInterval, stopwatch);
        LogScanCompletion(stopwatch);

        HelperScanner.WriteKeyValuePairsToCsv(keyValuePairs, "Key-Value-Pairs-22.csv");
        HelperScanner.WriteclassNameContainersToCsv(rttiClassNames, "RTTI-Class-Names-22.csv");
    }

    private void ScanMemoryAddresses(int elementCount, int progressUpdateInterval, Stopwatch stopwatch) {
        ulong startAddress = (ulong)ProcessoryClient.ProcessService.GetModuleBaseAddress();
        ulong endAddress = (ulong)ProcessoryClient.ProcessService.GetModuleEndAddress();
        int currentIteration = 0;
        int nextProgressUpdate = progressUpdateInterval;

        try {
            for (ulong address = startAddress; address < endAddress; address += PointerSize) {
                ulong pointer = ProcessoryClient.MemoryReader.Read<ulong>((nuint)address);

                uint pointerUINT = ProcessoryClient.MemoryReader.Read<uint>((nuint)pointer);

                var pointerTest = ProcessoryClient.MemoryReader.Read<nuint>((nuint)address);

                string className = GetFirstRTTIClassName(pointer);
                if (ProcessoryClient.AddressHelper.IsValidPointer(pointerTest, out nuint endPtr)) {
                    keyValuePairs.Add(address.ToString("X"), new List<AddressContainer> {
                        new() {
                            Name = className,
                            InitialAddress = (nuint)address,
                            AddressValue = pointerTest,
                            EndAddress = endPtr,
                        }
                    });
                }

                ProcessMemoryAddress(pointer, pointerUINT, ref currentIteration, ref nextProgressUpdate, elementCount, stopwatch);
            }
        }
        catch (Exception ex) {
            ScanLogger.Error(ex, "Exception occurred while scanning {ModuleName}", string.Empty);
        }
    }

    private void ProcessMemoryAddress(ulong pointer, uint pointerUINT, ref int currentIteration, ref int nextProgressUpdate, int totalElements, Stopwatch stopwatch) {
        HandleValidPointerT(pointer, pointerUINT);
        UpdateScanProgress(ref currentIteration, ref nextProgressUpdate, totalElements, stopwatch);
    }

    private void HandleValidPointerT(ulong pointer, uint pointerUINT) {
        string className = GetFirstRTTIClassName(pointer);
        if (string.IsNullOrEmpty(className)) return;

        if (!rttiClassNames.TryGetValue(className, out List<ClassNameContainer>? classNameContainers)) {
            classNameContainers = new List<ClassNameContainer>();
            rttiClassNames[className] = classNameContainers;
        }

        // Check for duplicates
        if (!classNameContainers.Any(container => container.Address == pointer)) {
            classNameContainers.Add(new ClassNameContainer {
                Address = pointer,
                PatternValue = pointerUINT
            });
        }
    }

    private string GetFirstRTTIClassName(ulong pointer) {
        string[] classNames = ProcessoryClient.RunTimeTypeInformation.GetRTTIClass(pointer);
        return classNames?.FirstOrDefault() ?? string.Empty;
    }

    private void UpdateScanProgress(ref int currentIteration, ref int nextProgressUpdate, int totalElements, Stopwatch stopwatch) {
        currentIteration++;
        if (currentIteration >= nextProgressUpdate) {
            LogProgress(currentIteration, totalElements, stopwatch, rttiClassNames);
            nextProgressUpdate += CalculateProgressInterval(totalElements);
        }
    }
}