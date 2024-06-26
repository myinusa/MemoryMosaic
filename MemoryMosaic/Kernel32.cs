using System.Runtime.InteropServices;
using static Processory.Native.Structures;

namespace MemoryMosaic;

public static partial class NativeMethods {

    [DllImport("kernel32.dll")]
    internal static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

    [DllImport("kernel32.dll")]
    internal static extern IntPtr OpenProcess(Flags.ProcessAccess processAccess, bool bInheritHandle, int processId);

    //[DllImport("kernel32.dll")]
    //internal static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll")]
    internal static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

    [DllImport("kernel32.dll")]
    internal static extern int VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

    [DllImport("kernel32.dll")]
    internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr buffer, int size, IntPtr lpNumberOfBytesRead);
}
