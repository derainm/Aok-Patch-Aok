// Decompiled with JetBrains decompiler
// Type: NativeMethods
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

internal static class NativeMethods
{
  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern SafeFileHandle CreateFile(
    string lpFileName,
    NativeMethods.EFileAccess dwDesiredAccess,
    NativeMethods.EFileShare dwShareMode,
    IntPtr lpSecurityAttributes,
    NativeMethods.ECreationDisposition dwCreationDisposition,
    NativeMethods.EFileAttributes dwFlagsAndAttributes,
    IntPtr hTemplateFile);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern SafeFileHandle CreateFileMapping(
    SafeFileHandle hFile,
    IntPtr lpFileMappingAttributes,
    NativeMethods.FileMapProtection flProtect,
    uint dwMaximumSizeHigh,
    uint dwMaximumSizeLow,
    string lpName);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern SafeFileHandle CreateFileMapping(
    SafeFileHandle hFile,
    IntPtr lpFileMappingAttributes,
    NativeMethods.FileMapProtection flProtect,
    uint dwMaximumSizeHigh,
    uint dwMaximumSizeLow,
    IntPtr lpName);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern IntPtr MapViewOfFile(
    SafeFileHandle hFileMappingObject,
    NativeMethods.FileMapAccess dwDesiredAccess,
    uint dwFileOffsetHigh,
    uint dwFileOffsetLow,
    UIntPtr dwNumberOfBytesToMap);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

  [DllImport("dbghelp.dll", SetLastError = true)]
  public static extern IntPtr ImageNtHeader(IntPtr ImageBase);

  [DllImport("dbghelp.dll", SetLastError = true)]
  public static extern IntPtr ImageRvaToVa(
    IntPtr NtHeaders,
    IntPtr Base,
    uint Rva,
    IntPtr LastRvaSection);

  [Flags]
  public enum EFileAccess : uint
  {
    AccessSystemSecurity = 16777216, // 0x01000000
    MaximumAllowed = 33554432, // 0x02000000
    Delete = 65536, // 0x00010000
    ReadControl = 131072, // 0x00020000
    WriteDAC = 262144, // 0x00040000
    WriteOwner = 524288, // 0x00080000
    Synchronize = 1048576, // 0x00100000
    StandardRightsRequired = WriteOwner | WriteDAC | ReadControl | Delete, // 0x000F0000
    StandardRightsRead = ReadControl, // 0x00020000
    StandardRightsWrite = StandardRightsRead, // 0x00020000
    StandardRightsExecute = StandardRightsWrite, // 0x00020000
    StandardRightsAll = StandardRightsExecute | Synchronize | WriteOwner | WriteDAC | Delete, // 0x001F0000
    SpecificRightsAll = 65535, // 0x0000FFFF
    FILE_READ_DATA = 1,
    FILE_LIST_DIRECTORY = FILE_READ_DATA, // 0x00000001
    FILE_WRITE_DATA = 2,
    FILE_ADD_FILE = FILE_WRITE_DATA, // 0x00000002
    FILE_APPEND_DATA = 4,
    FILE_ADD_SUBDIRECTORY = FILE_APPEND_DATA, // 0x00000004
    FILE_CREATE_PIPE_INSTANCE = FILE_ADD_SUBDIRECTORY, // 0x00000004
    FILE_READ_EA = 8,
    FILE_WRITE_EA = 16, // 0x00000010
    FILE_EXECUTE = 32, // 0x00000020
    FILE_TRAVERSE = FILE_EXECUTE, // 0x00000020
    FILE_DELETE_CHILD = 64, // 0x00000040
    FILE_READ_ATTRIBUTES = 128, // 0x00000080
    FILE_WRITE_ATTRIBUTES = 256, // 0x00000100
    GenericRead = 2147483648, // 0x80000000
    GenericWrite = 1073741824, // 0x40000000
    GenericExecute = 536870912, // 0x20000000
    GenericAll = 268435456, // 0x10000000
    SPECIFIC_RIGHTS_ALL = 65535, // 0x0000FFFF
    FILE_ALL_ACCESS = FILE_WRITE_ATTRIBUTES | FILE_READ_ATTRIBUTES | FILE_DELETE_CHILD | FILE_TRAVERSE | FILE_WRITE_EA | FILE_READ_EA | FILE_CREATE_PIPE_INSTANCE | FILE_ADD_FILE | FILE_LIST_DIRECTORY | StandardRightsAll, // 0x001F01FF
    FILE_GENERIC_READ = FILE_READ_ATTRIBUTES | FILE_READ_EA | FILE_LIST_DIRECTORY | StandardRightsExecute | Synchronize, // 0x00120089
    FILE_GENERIC_WRITE = FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_CREATE_PIPE_INSTANCE | FILE_ADD_FILE | StandardRightsExecute | Synchronize, // 0x00120116
    FILE_GENERIC_EXECUTE = FILE_READ_ATTRIBUTES | FILE_TRAVERSE | StandardRightsExecute | Synchronize, // 0x001200A0
  }

  [Flags]
  public enum EFileShare : uint
  {
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
  }

  public enum ECreationDisposition : uint
  {
    New = 1,
    CreateAlways = 2,
    OpenExisting = 3,
    OpenAlways = 4,
    TruncateExisting = 5,
  }

  [Flags]
  public enum EFileAttributes : uint
  {
    Readonly = 1,
    Hidden = 2,
    System = 4,
    Directory = 16, // 0x00000010
    Archive = 32, // 0x00000020
    Device = 64, // 0x00000040
    Normal = 128, // 0x00000080
    Temporary = 256, // 0x00000100
    SparseFile = 512, // 0x00000200
    ReparsePoint = 1024, // 0x00000400
    Compressed = 2048, // 0x00000800
    Offline = 4096, // 0x00001000
    NotContentIndexed = 8192, // 0x00002000
    Encrypted = 16384, // 0x00004000
    Write_Through = 2147483648, // 0x80000000
    Overlapped = 1073741824, // 0x40000000
    NoBuffering = 536870912, // 0x20000000
    RandomAccess = 268435456, // 0x10000000
    SequentialScan = 134217728, // 0x08000000
    DeleteOnClose = 67108864, // 0x04000000
    BackupSemantics = 33554432, // 0x02000000
    PosixSemantics = 16777216, // 0x01000000
    OpenReparsePoint = 2097152, // 0x00200000
    OpenNoRecall = 1048576, // 0x00100000
    FirstPipeInstance = 524288, // 0x00080000
  }

  [Flags]
  public enum FileMapProtection : uint
  {
    PageReadonly = 2,
    PageReadWrite = 4,
    PageWriteCopy = 8,
    PageExecuteRead = 32, // 0x00000020
    PageExecuteReadWrite = 64, // 0x00000040
    SectionCommit = 134217728, // 0x08000000
    SectionImage = 16777216, // 0x01000000
    SectionNoCache = 268435456, // 0x10000000
    SectionReserve = 67108864, // 0x04000000
  }

  [Flags]
  public enum FileMapAccess : uint
  {
    FileMapCopy = 1,
    FileMapWrite = 2,
    FileMapRead = 4,
    FileMapAllAccess = 31, // 0x0000001F
    FileMapExecute = 32, // 0x00000020
  }

  public struct IMAGE_FILE_HEADER
  {
    public ushort Machine;
    public ushort NumberOfSections;
    public uint TimeDateStamp;
    public uint PointerToSymbolTable;
    public uint NumberOfSymbols;
    public ushort SizeOfOptionalHeader;
    public ushort Characteristics;
  }

  public struct IMAGE_DATA_DIRECTORY
  {
    public uint VirtualAddress;
    public uint Size;
  }

  public struct IMAGE_OPTIONAL_HEADER
  {
    public ushort Magic;
    public byte MajorLinkerVersion;
    public byte MinorLinkerVersion;
    public uint SizeOfCode;
    public uint SizeOfInitializedData;
    public uint SizeOfUninitializedData;
    public uint AddressOfEntryPoint;
    public uint BaseOfCode;
    public uint BaseOfData;
    public uint ImageBase;
    public uint SectionAlignment;
    public uint FileAlignment;
    public ushort MajorOperatingSystemVersion;
    public ushort MinorOperatingSystemVersion;
    public ushort MajorImageVersion;
    public ushort MinorImageVersion;
    public ushort MajorSubsystemVersion;
    public ushort MinorSubsystemVersion;
    public uint Win32VersionValue;
    public uint SizeOfImage;
    public uint SizeOfHeaders;
    public uint CheckSum;
    public ushort Subsystem;
    public ushort DllCharacteristics;
    public uint SizeOfStackReserve;
    public uint SizeOfStackCommit;
    public uint SizeOfHeapReserve;
    public uint SizeOfHeapCommit;
    public uint LoaderFlags;
    public uint NumberOfRvaAndSizes;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public NativeMethods.IMAGE_DATA_DIRECTORY[] DataDirectory;
  }

  public struct IMAGE_NT_HEADERS
  {
    public uint Signature;
    public NativeMethods.IMAGE_FILE_HEADER FileHeader;
    public NativeMethods.IMAGE_OPTIONAL_HEADER OptionalHeader;
  }

  public struct IMAGE_EXPORT_DIRECTORY
  {
    public uint Characteristics;
    public uint TimeDateStamp;
    public ushort MajorVersion;
    public ushort MinorVersion;
    public uint Name;
    public uint Base;
    public uint NumberOfFunctions;
    public uint NumberOfNames;
    public uint AddressOfFunctions;
    public uint AddressOfNames;
    public uint AddressOfNameOrdinals;
  }
}
