using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace EZUnmanagedFunctions
{
    public enum EZFuncStartupAttributesValues
    {
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_NO_WINDOW = 0x08000000,
        CREATE_NEW_CONSOLE = 0x00000010,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_SECURE_PROCESS = 0x00400000,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_SUSPENDED = 0x00000004,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        DEBUG_PROCESS = 0x00000001,
        DETACHED_PROCESS = 0x00000008,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        INHERIT_PARENT_AFFINITY = 0x00010000
    }

    public class EZFuncStartupAttributes
    {
        public static uint CREATE_BREAKAWAY_FROM_JOB = 0x01000000;
        public static uint CREATE_NO_WINDOW = 0x08000000;
        public static uint CREATE_NEW_CONSOLE = 0x00000010;
        public static uint CREATE_NEW_PROCESS_GROUP = 0x00000200;
        public static uint CREATE_PROTECTED_PROCESS = 0x00040000;
        public static uint CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000;
        public static uint CREATE_SECURE_PROCESS = 0x00400000;
        public static uint CREATE_SEPARATE_WOW_VDM = 0x00000800;
        public static uint CREATE_SHARED_WOW_VDM = 0x00001000;
        public static uint CREATE_SUSPENDED = 0x00000004;
        public static uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        public static uint DEBUG_ONLY_THIS_PROCESS = 0x00000002;
        public static uint DEBUG_PROCESS = 0x00000001;
        public static uint DETACHED_PROCESS = 0x00000008;
        public static uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        public static uint INHERIT_PARENT_AFFINITY = 0x00010000;
    }
    public class EZFuncs
    {
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessW(
        string lpApplicationName,
        string lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        EZFuncStartupAttributesValues dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation
    );
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        public static bool LaunchProcessW(string applicationPath, string arguments, bool inheritHandle, EZFuncStartupAttributesValues attributes, STARTUPINFO startInfo)
        {
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            bool success = CreateProcessW(
                applicationPath,
                arguments,
                IntPtr.Zero,
                IntPtr.Zero,
                inheritHandle,
                attributes,
                IntPtr.Zero,
                null,
                ref startInfo,
                out processInfo
            );

            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"CreateProcessW failed with error {error}");
                return false;
            }

            return true;
        }
        public static bool AllocateConsoleW()
        {
            bool alloc = AllocConsole();
            return alloc;
        }

        public static bool FreeConsoleW()
        {
            bool consoleReturnVal = FreeConsole();
            return consoleReturnVal;
        }

        public static bool AllocRedirectConsoleW()
        {
            bool alloc = AllocConsole();
            if (alloc)
            {
                AttachConsole(System.Diagnostics.Process.GetCurrentProcess().Id);
                IntPtr stdHandle = GetStdHandle(-11); // STD_OUTPUT_HANDLE = -11
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                StreamWriter standardOutput = new StreamWriter(fileStream, Console.OutputEncoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            return alloc;

        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        private const int STD_OUTPUT_HANDLE = -11;
    }
}
