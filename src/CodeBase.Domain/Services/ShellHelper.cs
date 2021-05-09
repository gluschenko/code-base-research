using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CodeBase.Domain.Services
{
    public static class ShellHelper
    {
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Runs shell commands
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static ShellResult Execute(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            //
            string name;

            if (IsWindows())
            {
                name = "powershell";
                escapedArgs = $"-executionpolicy unrestricted -command {escapedArgs}";
            }
            else
            if (IsLinux())
            {
                name = "/bin/bash";
                escapedArgs = $"-c \"{escapedArgs}\"";
            }
            else
            {
                // Троллим маководов
                throw new NotSupportedException("This OS is not supported");
            }

            return Run(name, escapedArgs);
        }

        private static ShellResult Run(string processName, string cmd)
        {
            var info = new ProcessStartInfo
            {
                FileName = processName,
                Arguments = cmd,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(info);
            var result = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new ShellResult(result, error);
        }

        public record ShellResult(string Result, string Error)
        {
            public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
        }
    }
}
