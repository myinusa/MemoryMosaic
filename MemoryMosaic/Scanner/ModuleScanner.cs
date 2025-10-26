using System.Diagnostics;
using System.Runtime.InteropServices;
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
    public ulong PatternValue { get; set; }
}

public class ModuleScanner : ScanUtils, IModuleScanner {
    private const int PointerSize32Bit = 4;
    private const int PointerSize64Bit = 8;
    private const int Architecture32Bit = 32;
    private const int Architecture64Bit = 64;
    private const ulong Max32BitAddressValue = 0xFFFFFFFFUL;
    private const ulong Max64BitAddressValue = 0xFFFFFFFFFFFFFFFFUL;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool IsWow64Process(IntPtr hProcess, out bool isWow64);

    private readonly int pointerSize;
    private readonly bool isTargetProcess64Bit;
    private static readonly ILogger ScanLogger = Log.ForContext<ModuleScanner>();
    private readonly Dictionary<string, List<ClassNameContainer>> rttiClassNames = new();
    private readonly Dictionary<string, List<AddressContainer>> keyValuePairs = new();
    public ProcessoryClient ProcessoryClient { get; set; }

    public ModuleScanner(ProcessoryClient processoryClient) {
        ProcessoryClient = processoryClient;
        isTargetProcess64Bit = DetermineTargetProcessArchitecture();
        pointerSize = isTargetProcess64Bit ? PointerSize64Bit : PointerSize32Bit;

        ScanLogger.Information(
            "Target process architecture: {Architecture}-bit, PointerSize: {PointerSize} bytes",
            isTargetProcess64Bit ? Architecture64Bit : Architecture32Bit,
            pointerSize);
    }

    private bool DetermineTargetProcessArchitecture() {
        try {
            // If we're on a 32-bit OS, all processes are 32-bit
            if (!Environment.Is64BitOperatingSystem) {
                return false;
            }

            // Get the target process handle
            var processHandle = ProcessoryClient.ProcessHandle;
            if (processHandle == IntPtr.Zero) {
                ScanLogger.Warning("Process handle is zero, assuming 32-bit process");
                return false;
            }

            // Check if the target process is running under WOW64 (32-bit on 64-bit OS)
            if (!IsWow64Process(processHandle, out bool isWow64)) {
                ScanLogger.Warning("Failed to determine process architecture via IsWow64Process, using fallback");
                // Fallback: check if the target process MainModule is in Program Files (x86)
                var mainModule = ProcessoryClient.ProcessService?.ProcessHandle?.MainModule;
                if (mainModule != null) {
                    bool isIn32BitPath = mainModule.FileName?.Contains("(x86)", StringComparison.OrdinalIgnoreCase) ?? false;
                    return !isIn32BitPath;
                }
                return false;
            }

            // If running under WOW64, it's a 32-bit process on 64-bit OS
            // If NOT running under WOW64 on a 64-bit OS, it's a 64-bit process
            return !isWow64;
        }
        catch (Exception ex) {
            ScanLogger.Error(ex, "Error determining process architecture, defaulting to 32-bit");
            return false;
        }
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
        int elementCount = moduleSize / pointerSize;
        int progressUpdateInterval = CalculateProgressInterval(elementCount);

        LogScanStart(moduleSize, elementCount);

        var stopwatch = Stopwatch.StartNew();
        // ScanMemoryAddresses(elementCount, progressUpdateInterval, stopwatch);
        ScanMemoryAddressesT(elementCount, progressUpdateInterval, stopwatch);
        LogScanCompletion(stopwatch);

        HelperScanner.WriteKeyValuePairsToCsv(keyValuePairs, "Key-Value-Pairs.csv");
        HelperScanner.WriteclassNameContainersToCsv(rttiClassNames, "RTTI-Class-Names.csv");
    }

    private void ScanMemoryAddressesT(int elementCount, int progressUpdateInterval, Stopwatch stopwatch) {
        // These automatically use the targeted module's address range
        ulong startAddress = (ulong)ProcessoryClient.ModuleBaseAddress;
        ulong endAddress = (ulong)ProcessoryClient.ProcessService.GetModuleEndAddress(ProcessoryClient.ModuleName);

        ScanLogger.Information("Scanning from 0x{Start:X} to 0x{End:X}", startAddress, endAddress);

        int currentIteration = 0;
        int nextProgressUpdate = progressUpdateInterval;
        int validPointersChecked = 0;
        int rttiFound = 0;

        try {
            for (ulong address = startAddress; address < endAddress; address += (ulong)pointerSize) {
                // Read pointer value based on target process architecture
                ulong pointerValue;
                nuint pointerRegionProbe;

                if (isTargetProcess64Bit) {
                    pointerValue = ProcessoryClient.MemoryReader.Read<ulong>((nuint)address);
                    pointerRegionProbe = (nuint)pointerValue;
                }
                else {
                    uint pointer32 = ProcessoryClient.MemoryReader.Read<uint>((nuint)address);
                    pointerValue = pointer32;
                    pointerRegionProbe = pointer32;
                }

                // Skip obviously invalid pointers
                if (pointerValue == 0 || pointerValue == Max32BitAddressValue || (isTargetProcess64Bit && pointerValue == Max64BitAddressValue)) {
                    currentIteration++;
                    continue;
                }

                // Check if this is a valid pointer before trying RTTI
                if (ProcessoryClient.AddressHelper.IsValidPointer(pointerRegionProbe, out nuint endPtr)) {
                    validPointersChecked++;

                    // Try to get RTTI class name
                    string className = GetFirstRTTIClassName(address);

                    if (!string.IsNullOrEmpty(className)) {
                        rttiFound++;

                        // Store in keyValuePairs
                        keyValuePairs.Add(address.ToString("X"), new List<AddressContainer> {
                            new() {
                                Name = className,
                                InitialAddress = (nuint)address,
                                AddressValue = (nuint)pointerValue,
                                EndAddress = endPtr,
                            }
                        });

                        // Process for RTTI class names collection
                        HandleValidPointerT(address, className);
                    }
                }

                UpdateScanProgress(ref currentIteration, ref nextProgressUpdate, elementCount, stopwatch,
                    validPointersChecked, rttiFound);
            }

            ScanLogger.Information(
                "Scan complete. Valid pointers checked: {Valid}, RTTI classes found: {RTTI}",
                validPointersChecked, rttiFound);
        }
        catch (Exception ex) {
            ScanLogger.Error(ex, "Exception occurred while scanning {ModuleName}",
                ProcessoryClient.ModuleName ?? ProcessoryClient.ProcessName);
        }
    }

    private void HandleValidPointerT(ulong vtableAddress, string className) {
        if (string.IsNullOrEmpty(className)) {
            return;
        }

        if (!rttiClassNames.TryGetValue(className, out List<ClassNameContainer>? classNameContainers)) {
            classNameContainers = new List<ClassNameContainer>();
            rttiClassNames[className] = classNameContainers;
        }

        if (!classNameContainers.Any(container => container.Address == vtableAddress)) {
            classNameContainers.Add(new ClassNameContainer {
                Address = vtableAddress,
                PatternValue = vtableAddress
            });
        }
    }

    private string GetFirstRTTIClassName(ulong vtableAddress) {
        string[] classNames = ProcessoryClient.RunTimeTypeInformation.GetRTTIClass(vtableAddress);
        return classNames?.FirstOrDefault() ?? string.Empty;
    }

    private void UpdateScanProgress(ref int currentIteration, ref int nextProgressUpdate, int totalElements,
    Stopwatch stopwatch, int validPointers, int rttiFound) {
        currentIteration++;
        if (currentIteration >= nextProgressUpdate) {
            LogProgress(currentIteration, totalElements, stopwatch, rttiClassNames, validPointers, rttiFound);
            nextProgressUpdate += CalculateProgressInterval(totalElements);
        }
    }
}