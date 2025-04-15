# MemoryMosaic

MemoryMosaic is a specialized memory scanning utility designed to analyze process memory, identify RTTI (Run-Time Type Information) class names, and extract memory address information from running applications. It's particularly useful for developers working with memory analysis, reverse engineering, or application debugging.

## Overview

MemoryMosaic performs low-level memory scanning operations on a target process (specifically looking for applications with "2022" in their window title). The tool systematically scans through memory addresses, identifies pointers, and extracts RTTI class information using the Processory library for memory access.

### Technical Details

The core scanning functionality works by:
1. Obtaining the base address and size of the target process module
2. Iterating through memory addresses at 4-byte or 8-byte intervals (depending on pointer size)
3. Reading memory values at each address and checking if they are valid pointers
4. When valid pointers are found, extracting RTTI class information
5. Storing the results in structured collections for later export

The scanner implements several optimizations:
- Progress reporting at calculated intervals
- Performance timing using Stopwatch
- Structured error handling to prevent crashes during scanning
- Memory validation to avoid access violations

Key features include:
- Low-level process memory scanning
- RTTI (Run-Time Type Information) class name extraction
- Memory address and pointer mapping
- Detailed CSV report generation with timestamps
- Comprehensive structured logging with Serilog

## Requirements

- .NET 8.0 SDK or later
- Windows operating system (for process memory access)
- Visual Studio 2022 or later (for development)

## Dependencies and Technical Architecture

### Core Dependencies

MemoryMosaic is built on a modern .NET architecture with the following key dependencies:

- **Microsoft.Extensions.Hosting (9.0.0)** 
  - Provides the hosting framework for the application
  - Enables dependency injection for service management
  - Handles application lifecycle events
  - Supports configuration and environment abstractions

- **Serilog (4.1.0) and Extensions**
  - Implements structured logging throughout the application
  - Components used:
    - **Serilog.Enrichers.Process** - Adds process information to log entries
    - **Serilog.Enrichers.Thread** - Adds thread information to log entries
    - **Serilog.Expressions** - Enables complex log filtering and transformation
    - **Serilog.Extensions.Hosting** - Integrates Serilog with the hosting framework
    - **Serilog.Settings.Configuration** - Supports configuration-based logger setup
    - **Serilog.Sinks.Async** - Provides asynchronous logging for performance
    - **Serilog.Sinks.Console** - Outputs logs to the console with formatting
    - **Serilog.Sinks.File** - Writes logs to files with rolling capabilities

- **System.Text.RegularExpressions (4.3.1)**
  - Used for pattern matching in memory analysis
  - Supports complex string parsing operations

- **Processory (Local Reference)**
  - Custom library for process memory access and manipulation
  - Provides the `ProcessoryClient` class used for memory operations
  - Implements RTTI class name extraction functionality
  - Handles low-level memory reading and validation

### Code Quality Tools

The project maintains high code quality standards through static analysis tools:

- **Microsoft.CodeAnalysis.NetAnalyzers (9.0.0)**
  - Enforces .NET coding standards and best practices
  - Identifies potential performance and security issues

- **Roslynator.Analyzers (4.12.9) & Roslynator.Formatting.Analyzers (4.12.9)**
  - Provides additional code style and quality analyzers
  - Enforces consistent code formatting and structure

- **SonarAnalyzer.CSharp (10.3.0.106239)**
  - Detects bugs, vulnerabilities, and code smells
  - Enforces industry-standard code quality rules

- **StyleCop.Analyzers (1.1.118)**
  - Enforces style and consistency rules
  - Ensures documentation standards are met

- **xunit.analyzers (1.17.0)**
  - Supports test quality and best practices
  - Ensures proper test structure and implementation

### Technical Architecture

The application follows a modern architecture with these key patterns:

1. **Dependency Injection**
   - Services are registered in the `Program.cs` file
   - Constructor injection is used throughout the codebase
   - Interfaces are defined for testability and loose coupling

2. **Extension Methods**
   - Extensive use of extension methods for enhanced functionality
   - Pointer manipulation extensions for low-level memory operations
   - Logging extensions for contextual information

3. **Inheritance and Abstraction**
   - Base `ScanUtils` class provides common functionality
   - Interface `IModuleScanner` defines the contract for scanners
   - Specialized implementations handle specific scanning tasks

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/MemoryMosaic.git
   ```

2. Navigate to the project directory:
   ```
   cd MemoryMosaic
   ```

3. Build the project:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run --project MemoryMosaic
   ```

## Usage and Technical Operation

MemoryMosaic operates through a systematic process of memory scanning and analysis:

1. **Initialization**:
   - The application configures Serilog with console and file outputs
   - Dependency injection container is set up with required services
   - The `ProcessoryClient` is initialized with the "fm" identifier

2. **Process Connection**:
   - The application searches for a process with "2022" in its window title
   - Process handle validation is performed to ensure memory access
   - Module base address and size are determined for scanning boundaries

3. **Memory Scanning**:
   - The scanner iterates through memory addresses at pointer-size intervals
   - For each address, it reads the memory value and checks if it's a valid pointer
   - Valid pointers are further analyzed for RTTI class information
   - Progress is reported at calculated intervals based on module size

4. **Data Collection**:
   - RTTI class names are extracted and stored in the `rttiClassNames` dictionary
   - Memory address mappings are stored in the `keyValuePairs` dictionary
   - Duplicate entries are handled to ensure data accuracy

5. **Report Generation**:
   - CSV files are generated with detailed information about findings
   - Timestamps are added to filenames for versioning and tracking
   - Reports are saved to the parent directory of the application

### Memory Analysis Approach

The scanner uses a pointer-chasing technique to identify and analyze memory structures:
- It reads a potential pointer value from memory
- Validates if the value points to a valid memory region
- Attempts to extract RTTI information from the pointer location
- Records both the pointer address and the data it points to

### Output Files

The application generates two CSV files with timestamps in their names:
- `YYYYMMDD-HHMMSS-RTTI-Class-Names-22.csv` - Contains RTTI class information:
  - Class name
  - Memory address (in hexadecimal)
  - Pattern value (numeric identifier)

- `YYYYMMDD-HHMMSS-Key-Value-Pairs-22.csv` - Contains memory address mappings:
  - Name (class name if available)
  - Base address (in hexadecimal)
  - Pointer address (in hexadecimal)
  - End module address (in hexadecimal)
  - Number of addresses

These files provide valuable information for memory analysis, debugging, and reverse engineering tasks.

## Project Structure and Technical Analysis

### Core Components

- **App.cs** - Main application logic
  - Implements the primary workflow using dependency injection
  - Contains the `Run()` method that initiates the scanning process
  - Uses the `ScanForProcessesAndModules()` method to validate the target process
  - Implements error handling and logging for the main application flow

- **Program.cs** - Application entry point and dependency injection setup
  - Configures the dependency injection container with required services
  - Sets up the `ProcessoryClient` with the "fm" identifier
  - Implements the host builder pattern for application configuration
  - Manages application lifecycle and exception handling

- **Startup.cs** - Configuration for Serilog and application startup
  - Implements a custom ANSI console theme for better log readability
  - Configures Serilog with multiple sinks (console and file)
  - Sets up log enrichment with process and thread information
  - Implements daily log file rolling with timestamps

### Scanner Components

- **ModuleScanner.cs** - Core memory scanning implementation
  - Implements the `IModuleScanner` interface with the `ScanModulesForRTTIClassNames()` method
  - Contains the `PerformModuleScan()` method that handles the scanning process
  - Uses `ScanMemoryAddresses()` to iterate through memory addresses
  - Implements `ProcessMemoryAddress()` and `HandleValidPointerT()` for memory analysis
  - Defines data structures (`AddressContainer` and `ClassNameContainer`) for storing scan results

- **ScanUtils.cs** - Base class with utility methods for scanning operations
  - Provides logging methods for scan progress and completion
  - Implements the `CalculateProgressInterval()` method for progress reporting
  - Contains error handling and reporting functionality
  - Manages performance monitoring with Stopwatch integration

- **HelperScanner.cs** - Data processing and output generation
  - Implements `WriteclassNameContainersToCsv()` to export RTTI class information
  - Provides `WriteKeyValuePairsToCsv()` for exporting memory address mappings
  - Contains `RemoveDuplicateAddressValues()` to clean up scan results
  - Handles file path generation with timestamps for output files

### Extension Methods

- **IntPtrExtension.cs** - Comprehensive extensions for IntPtr operations
  - Implements arithmetic operations (Increment, Decrement)
  - Provides comparison methods (CompareTo, Equals)
  - Contains conversion utilities (ToUInt32, ToUInt64, ToHex)
  - Implements bitwise operations (And, Or, Xor, Not)
  - Offers equality testing methods (GreaterThanOrEqualTo, LessThanOrEqualTo)

- **UIntPtrExtensions.cs** - Mirror extensions for UIntPtr operations
  - Parallels the IntPtr extensions but for unsigned pointers
  - Handles special cases for unsigned arithmetic
  - Provides the same comprehensive set of operations for UIntPtr

- **LoggerExtensions.cs** - Enhanced logging capabilities
  - Implements the `Here()` extension method for contextual logging
  - Uses `CallerMemberName`, `CallerFilePath`, and `CallerLineNumber` attributes
  - Enriches log entries with source code context information

## Customization and Advanced Usage

### Targeting Different Processes

To scan a different process or modify the scanning behavior:

1. Modify the `ScanForProcessesAndModules` method in `App.cs`:
   ```csharp
   private void ScanForProcessesAndModules(bool start, string year) {
       if (!start) return;

       // Change "2022" to your target process identifier
       if (!processoryClient.ProcessService.ProcessHandle.MainWindowTitle.Contains(year)) {
           Log.Here().Error("MainWindowTitle does not contain {Year}", year);
           Log.Here().Debug("MainWindowTitle: {Title}", processoryClient.ProcessService.ProcessHandle.MainWindowTitle);
           return;
       }
       StartModuleScan(true);
   }
   ```

2. Update the `InitializeApplication` method to use your target identifier:
   ```csharp
   public void InitializeApplication() {
       // Change "2022" to your target process identifier
       ScanForProcessesAndModules(true, "YourTargetIdentifier");
   }
   ```

### Adjusting Scanning Parameters

To modify how memory is scanned and analyzed:

1. In `ModuleScanner.cs`, adjust the scanning parameters:
   ```csharp
   private void ScanMemoryAddresses(int elementCount, int progressUpdateInterval, Stopwatch stopwatch) {
       // Modify these parameters to change scanning behavior
       ulong startAddress = (ulong)ProcessoryClient.ProcessService.GetModuleBaseAddress();
       ulong endAddress = (ulong)ProcessoryClient.ProcessService.GetModuleEndAddress();
       
       // Change the increment value to scan at different intervals
       // Current: address += PointerSize (4 or 8 bytes)
       for (ulong address = startAddress; address < endAddress; address += PointerSize) {
           // Scanning logic...
       }
   }
   ```

2. Adjust the progress reporting frequency in `ScanUtils.cs`:
   ```csharp
   // Change this value to adjust progress reporting frequency
   private const int ProgressReportIntervalPercentage = 5; // Currently reports every 5%
   ```

### Customizing Output Format

To modify the CSV output format:

1. In `HelperScanner.cs`, adjust the CSV header and row format:
   ```csharp
   public static void WriteclassNameContainersToCsv(Dictionary<string, List<ClassNameContainer>> rttiClassNames, string fileName) {
       // ...
       
       // Modify the header line to include different columns
       writer.WriteLine("Name,Address,Pattern,YourNewColumn");

       foreach (var pair in rttiClassNames) {
           foreach (var container in pair.Value) {
               // Modify the row format to include your new data
               writer.WriteLine($"{pair.Key},{container.Address:X},{container.PatternValue},YourNewData");
           }
       }
   }
   ```

2. Add new properties to the data container classes in `ModuleScanner.cs`:
   ```csharp
   public class ClassNameContainer {
       public ulong Address { get; set; }
       public uint PatternValue { get; set; }
       // Add your new properties here
       public string YourNewProperty { get; set; }
   }
   ```

### Extending Functionality

To add new scanning capabilities:

1. Implement a new scanner class that inherits from `ScanUtils`:
   ```csharp
   public class YourCustomScanner : ScanUtils, IYourCustomScanner {
       // Implement your custom scanning logic
   }
   ```

2. Register your new scanner in the dependency injection container in `Program.cs`:
   ```csharp
   private static void EntityMemoryServices(IServiceCollection services) {
       services.AddSingleton(_ => new ProcessoryClient("fm"));
       services.AddSingleton<IModuleScanner, ModuleScanner>();
       // Register your new scanner
       services.AddSingleton<IYourCustomScanner, YourCustomScanner>();
   }
   ```

3. Inject and use your scanner in `App.cs` or create a new application flow.

## License

MemoryMosaic is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Technical Limitations and Considerations

### Memory Access Constraints

- The application requires administrative privileges to access process memory
- Some processes may have protected memory regions that cannot be scanned
- Anti-cheat or security software may detect and block memory scanning operations
- Memory scanning is inherently dependent on the target process's memory layout, which can change between versions

### Performance Considerations

- Scanning large memory regions can be time-consuming
- The application uses a single-threaded scanning approach by default
- Memory usage can be significant when scanning large processes
- CSV file generation may be slow for very large result sets

### Security Implications

- Memory scanning can potentially access sensitive information
- The application does not implement any encryption for the generated CSV files
- Running with elevated privileges poses security risks

## Disclaimer

This tool is intended for educational and debugging purposes only. Use responsibly and only on processes that you have permission to analyze. Memory scanning may violate terms of service for some applications, particularly games or secure software. The authors are not responsible for any misuse of this tool or consequences thereof.

Memory scanning and analysis techniques should only be used:
- On your own applications for debugging purposes
- On applications where you have explicit permission to perform memory analysis
- In controlled environments for educational purposes

This software is provided "as is" under the MIT License with no warranties or guarantees of any kind.
