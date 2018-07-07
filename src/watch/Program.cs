using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace watch {
    public class Program {
        public static void Main(string[] args) {
            if (args.Length < 1) {
                PrintUsage();
                return;
            }

            int loopSeconds = 2;
            bool printHeader = true;

            int argi = 0;
            while (argi < args.Length && TryGetOption(args[argi], out string option)) {
                argi++;
                switch (option) {
                    case "n":
                        string s = args[argi++];
                        if (int.TryParse(s, out int n) && n > 0) {
                            loopSeconds = n;
                            break;
                        } else {
                            Console.WriteLine($"watch: invalid number '{s}'");
                            return;
                        }
                    case "t":
                        printHeader = false;
                        break;
                    default:
                        Console.WriteLine($"watch: unrecognized option: {option}");
                        PrintUsage();
                        return;
                }
            }

            if (argi >= args.Length) {
                PrintUsage();
                return;
            }

            string command = string.Join(' ', args, argi, args.Length - argi);

            Watch(command, loopSeconds, printHeader);
        }

        private static void Watch(string command, int loopSeconds = 2, bool printHeader = true) {
            var process = new Process() {
                StartInfo = new ProcessStartInfo() {
                    FileName = "cmd",
                    Arguments = $"/C {command}",
                    UseShellExecute = false
                }
            };

            string header = $"Every {loopSeconds}s: {command}";

            while (true) {
                string now = DateTime.Now.ToString();

                Console.Title = $"{header} [{now}]";

                Console.Clear();
                if (printHeader) {
                    Console.SetCursorPosition(0, 0);
                    Console.Write(header);
                    Console.SetCursorPosition(Console.WindowWidth - now.Length, 0);
                    Console.Write(now);
                    Console.SetCursorPosition(0, 1);
                }

                process.Start();
                process.WaitForExit();

                Thread.Sleep(loopSeconds * 1000);
            }
        }

        private static void PrintUsage() {
            const string Usage =
@"Usage: watch [-n SEC] [-t] PROG ARGS

Run PROG periodically

        -n      Loop period in seconds (default 2)
        -t      Don't print header";
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
    }
}
