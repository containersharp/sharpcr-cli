using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpCR.CLI
{
    class Docker
    {
        public void Fire(string[] args)
        {
            if (args == null || args.Length < 1) return;

            var command = args[0].ToLower();

            switch (command)
            {
                case "pull":
                    Pull(args);
                    break;
                case "run":
                    Run(args);
                    break;
                default:
                    RunCommand(args);
                    break;
            }
        }

        void Run(string[] args)
        {
            var image = ArgsHelper.ExtractImageFromArgs(
                args,
                ArgsHelper.RUN_NO_VALUE_OPTIONS);

            if (image == null)
            {
                Console.WriteLine(
                    "Image not found from args: " + string.Join(" ", args));
                return;
            }

            if (!IsImageExisted(image))
            {
                Pull(new[] {"pull", image});
            }

            RunCommand(args);
        }

        bool IsImageExisted(string image)
        {
            var process = RunCommand(new[] {"images", "-q", image}, true);

            var output = process.StandardOutput.ReadToEnd();

            return !output.Equals("");
        }

        void Pull(string[] args)
        {
            var (source, target) = ArgsHelper.ParseImage(args.Last());
            var command = new[] {"pull"}
                .Concat(args.Skip(1).Take(args.Length - 2))
                .Concat(new[] {source}).ToArray();
            var process = RunCommand(command);

            if (process.ExitCode == 0)
            {
                RunCommand(new[] {"tag", source, target});
                RunCommand(new[] {"rmi", source});
            }
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

            return Tuple.Create(
                "command",
                string.Join(" ", new[] {"docker"}.Concat(args)));
        }
    }
}
