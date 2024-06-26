using System.Runtime.InteropServices;

namespace MemoryMosaic;

public class Structures {
    /// <summary>
    /// Defines the protection to be applied to a region of virtual memory
    /// </summary>
    [Flags]
    public enum MemoryProtection : int {
        /// <summary>
        /// Disables all access to the region of virtual memory
        /// </summary>
        ZeroAccess = 0x00,
        /// <summary>
        /// Disables all access to the region of virtual memory
        /// </summary>
        NoAccess = 0x01,
        /// <summary>
        /// Marks the region of virtual memory as readable
        /// </summary>
        ReadOnly = 0x02,
        /// <summary>
        /// Marks the region of virtual memory as readable and/or writable
        /// </summary>
        ReadWrite = 0x04,
        /// <summary>
        /// Marks the region of virtual memory as readable and/or copy on write
        /// </summary>
        WriteCopy = 0x08,
        /// <summary>
        /// Marks the region of virtual memory as executable
        /// </summary>
        Execute = 0x10,
        /// <summary>
        /// Marks the region of virtual memory as readable and/or executable
        /// </summary>
        ExecuteRead = 0x20,
        /// <summary>
        /// Marks the region of virtual memory as readable, writable and/or executable
        /// </summary>
        ExecuteReadWrite = 0x40,
        /// <summary>
        /// Marks the region of virtual memory as readable, copy on write and/or executable
        /// </summary>
        ExecuteWriteCopy = 0x80,
        /// <summary>
        /// Marks the region of virtual memory as guarded
        /// </summary>
        Guard = 0x100,
        /// <summary>
        /// Marks the region of virtual memory as readable, writable and guarded
        /// </summary>
        ReadWriteGuard = 0x104,
        /// <summary>
        /// Marks the region of virtual memory as non-cacheable 
        /// </summary>
        NoCache = 0x200,
        Writable = ReadWrite | WriteCopy | ExecuteReadWrite | ExecuteWriteCopy | Guard | ReadWriteGuard,
    }

    internal enum ProcessorArchitecture {
        X86 = 0,
        X64 = 9,
        Arm = -1,
        Itanium = 6,
        Unknown = 0xFFFF
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEM_INFO {
        /// <summary>
        /// The architecture of the processor.
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture;

        /// <summary>
        /// The page size and the granularity of page protection and commitment.
        /// </summary>
        public uint PageSize;

        /// <summary>
        /// A pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs).
        /// </summary>
        public IntPtr MinimumApplicationAddress;

        /// <summary>
        /// A pointer to the highest memory address accessible to applications and DLLs.
        /// </summary>
        public IntPtr MaximumApplicationAddress;
        /// <summary>
        /// A mask representing the set of processors configured into the system.
        /// </summary>
        public IntPtr ActiveProcessorMask;

        /// <summary>
        /// The number of logical processors in the current group.
        /// </summary>
        public uint NumberOfProcessors;

        /// <summary>
        /// An obsolete member that is retained for compatibility. 
        /// </summary>
        public uint ProcessorType;

        /// <summary>
        /// The granularity for the starting address at which virtual memory can be allocated.
        /// </summary>
        public uint AllocationGranularity;

        /// <summary>
        /// The architecture-dependent processor level.
        /// </summary>
        public ushort ProcessorLevel; // WORD

        /// <summary>
        /// The architecture-dependent processor revision.
        /// </summary>
        public ushort ProcessorRevision; // WORD
    }


    /// <summary>
    ///     Contains information about a range of pages in the virtual address space of a process
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION {
        /// <summary>
        /// A pointer to the base address of a range of pages.
        /// </summary>
        public IntPtr BaseAddress;

        /// <summary>
        /// A pointer to the base address of a range of pages allocated by the VirtualAlloc function.
        /// </summary>
        public IntPtr AllocationBase;

        /// <summary>
        /// The memory protection option when the region was initially allocated.
        /// </summary>
        public uint AllocationProtect;

        /// <summary>
        ///  The size of the region beginning at the base address in which all pages have identical attributes, in bytes.
        /// </summary>
        public IntPtr RegionSize;

        /// <summary>
        /// The state of the pages in the region (free, reserved, or committed).
        /// </summary>
        public uint State;

        /// <summary>
        /// The access protection of the pages in the region.
        /// </summary>
        public uint Protect;

        /// <summary>
        /// The type of pages in the region (private, mapped, or image).
        /// </summary>
        public uint Type;
    }


}
