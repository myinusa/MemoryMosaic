namespace MemoryMosaic;

public static class Flags {

    [Flags]
    public enum ProcessAccess : uint {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    //Memory State
    //https://msdn.microsoft.com/en-us/library/windows/desktop/aa366775(v=vs.85).aspx
    [Flags]
    public enum MemoryState : uint {
        MEM_COMMIT = 0x1000,
        PAGE_GUARD = 0x100,
        PAGE_NO_ACCESS = 0x01,
        PAGE_READWRITE = 0x04
    }
}