using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpCR.CLI
{
    class Docker
    {
        const string SHARPCR_REGISTRY = "jijiechen-docker.pkg.coding.net";

        public void Run(string[] args)
        {
            if (args == null || args.Length < 1) return;

            var command = args[0];

            if (command.Equals("pull", StringComparison.OrdinalIgnoreCase))
            {
                Pull(args.Skip(1).ToArray());
            }
            else
            {
                RunCommand(args);
            }
        }

        void Pull(string[] args)
        {
            var image = args[0];
            var sharpImage = $"{SHARPCR_REGISTRY}/{image}";

            if (CheckImageExisted(image)) return;

            var process = RunCommand(new[] {"pull", sharpImage}.Concat(args.Skip(1)).ToArray());

            if (process.ExitCode == 0)
            {
                RunCommand(new[] {"tag", sharpImage, image});
                RunCommand(new[] {"rmi", sharpImage});
            }
        }

        bool CheckImageExisted(string image)
        {
            var process = RunCommand(new[] {"images", "-q", image}, true);

            var output = process.StandardOutput.ReadToEnd();

            return !output.Equals("");
        }

        Process RunCommand(string[] args, bool redirectOutput = false)
        {
            var (cmd, arguments) = BuildCommand(args);

            var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = arguments,
                    RedirectStandardOutput = redirectOutput
                });

            process?.WaitForExit();

            return process;
        }

        Tuple<string, string> BuildCommand(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Tuple.Create("docker.exe", string.Join(" ", args));
            }

            return Tuple.Create("command", string.Join(" ", new[] {"docker"}.Concat(args)));
        }
    }
}
