using System;
using System.Drawing;

namespace img2motd {
    class Program {
        static void Main(string[] args) {
            string readFile = null, writeFile = null;
            Color shellBackground = Color.Black;

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "--background":
                        shellBackground = ColorTranslator.FromHtml(args[++i]);
                        break;
                    case "--out":
                        writeFile = args[++i];
                        break;
                    default:
                        readFile = args[i];
                        break;
                }
            }

            if (readFile == null) {
                Console.WriteLine("no file determined.");
                return;
            }

            writeFile = writeFile ?? $"{readFile}.motd";

            new MotdGenerator(readFile, writeFile, shellBackground).Generate();
            Console.WriteLine(writeFile);
        }
    }
}
