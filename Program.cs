using System;
using System.Drawing;

namespace img2motd {
    class Program {
        static void Main(string[] args) {
            string readFile = null, writeFile = null;
            Color shellBackground = Color.Black;
            bool plainText = true;

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "--background":
                        shellBackground = ColorTranslator.FromHtml(args[++i]);
                        break;
                    case "--out":
                        writeFile = args[++i];
                        break;
                    case "--script":
                        plainText = false;
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

            writeFile = writeFile ?? $"{readFile}.{(plainText ? "motd" : "sh")}";

            new MotdGenerator(readFile, writeFile, shellBackground).Generate(plainText);
            Console.WriteLine(writeFile);
        }
    }
}
