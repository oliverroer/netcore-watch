using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace watch {
    class Program {
        private const string Usage =
@"Usage: watch [-n SEC] [-t] PROG ARGS

Run PROG periodically

        -n      Loop period in seconds (default 2)
        -t      Don't print header";

        private static void PrintUsage() {
            Console.WriteLine(Usage);
        }

        private static bool TryGetOption(string arg, out string option) {
            if (!arg.StartsWith('-')) {
                option = null;
                return false;
            }

            option = arg.Remove(0, arg.StartsWith("--") ? 2 : 1);

            return true;
        }

        static void Main(string[] args) {
            if (args.Length < 1) {
                PrintUsage();
                return;
            }

            int argi = 0;
            int loopSeconds = 2;
            bool printHeader = true;
            while (argi < args.Length && TryGetOption(args[argi], out string option)) {
                switch (option) {
                    case "n":
                        argi++;
                        string s = args[argi];
                        if (int.TryParse(s, out int n) && n > 0) {
                            loopSeconds = argi;
                            argi++;
                            break;
                        } else {
                            Console.WriteLine($"watch: invalid number '{s}'");
                            return;
                        }
                    case "t":
                        printHeader = false;
                        argi++;
                        break;
                    default:
                        Console.WriteLine($"watch: unrecognized option: {option}");
                        PrintUsage();
                        return;
                }
            }

            string cmd = null;
            if (argi < args.Length) {
                cmd = string.Join(' ', args, argi, args.Length - argi);
            } else {
                PrintUsage();
                return;
            }

            string arguments = "/C " + cmd;
            var startInfo = new ProcessStartInfo() {
                FileName = "cmd",
                Arguments = arguments,
                UseShellExecute = false
            };

            var process = new Process() {
                StartInfo = startInfo
            };

            

            
            string header = $"Every {loopSeconds}s: {cmd}";
            while (true) {
                string now = DateTime.Now.ToString();

                Console.Clear();
                if (printHeader) {
                    Console.SetCursorPosition(0, 0);
                    Console.Write(header);
                    Console.SetCursorPosition(Console.WindowWidth - now.Length, 0);
                    Console.Write(now);
                    Console.SetCursorPosition(0, 1);
                }
                Console.Title = $"{header} [{now}]";
                
                process.Start();
                process.WaitForExit();

                Thread.Sleep(loopSeconds * 1000);
            }
        }
    }
}
