using System;
using System.Linq;

namespace SharpCR.CLI
{
    class ArgsHelper
    {
        public const string SHARPCR_REGISTRY = "jijiechen-docker.pkg.coding.net";

        public static readonly string[] RUN_NO_VALUE_OPTIONS =
        {
            "--detach", "-d", "--disable-content-trust", "--help", "--init",
            "--interactive", "-i", "--no-healthcheck", "--oom-kill-disable",
            "--publish-all", "-P", "--read-only", "--rm", "--sig-proxy",
            "--tty", "-t", "-it"
        };

        public static string ExtractImageFromArgs(
            string[] args,
            string[] noValueOptions)
        {
            for (var i = 1; i < args.Length; i++)
            {
                var cur = args[i];

                if (noValueOptions.Contains(cur))
                {
                    continue;
                }

                if (cur.StartsWith("-"))
                {
                    i++;
                    continue;
                }

                return cur;
            }

            return null;
        }

        public static Tuple<string, string> ParseImage(string image)
        {
            return Tuple.Create($"{SHARPCR_REGISTRY}/{image}", image);
        }
    }
}
