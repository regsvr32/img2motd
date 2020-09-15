using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace img2motd {
    class MotdGenerator {
        readonly string readFile, writeFile;
        readonly Color shellBackground;

        public MotdGenerator(string read, string write, Color background) {
            readFile = read;
            writeFile = write;
            shellBackground = background;
        }

        public void Generate(bool plainText) {
            using (var img = Image.FromFile(readFile)) {
                if (plainText) {
                    using (var bmp = new Bitmap(img)) {
                        File.WriteAllText(writeFile, string.Concat(Translate(bmp, false)));
                    }
                    return;
                }
                using (var sw = File.CreateText(writeFile)) {
                    sw.Write("#!/bin/bash\n");
                    sw.Write("cat <<CURSOR_ANCHOR\n\x1b[?25l\x1b[s\nCURSOR_ANCHOR\n");
                    var fd = new FrameDimension(img.FrameDimensionsList[0]);
                    int frameCount = img.GetFrameCount(fd);
                    int delay = 0;
                    if (frameCount > 1) {
                        var item = img.GetPropertyItem(0x5100);
                        delay = (item.Value[0] + item.Value[1] * 256) * 10000;
                    }
                    for (int i = 0; i < frameCount; i++) {
                        img.SelectActiveFrame(fd, i);
                        using (var bmp = new Bitmap(img)) { sw.Write(CatFrame(bmp, i)); }
                        if (delay > 0) {
                            sw.Write($"usleep {delay}\n");
                        }
                    }
                    sw.Write("cat <<SHOW_CURSOR\n\x1b[?25h\nSHOW_CURSOR\n");
                    sw.Flush();
                }
            }
        }

        string CatFrame(Bitmap bmp, int frame) => $"cat <<FRAME_{frame}\n{string.Concat(Translate(bmp, true))}FRAME_{frame}\n";

        int BlendChannel(byte orig, byte bg, byte a) => (orig * a + bg * (255 - a)) / 255;

        Color? BlendColor(Color origin) {
            switch (origin.A) {
                case 0:
                    return null;
                case 0xff:
                    return origin;
                default:
                    return Color.FromArgb(
                        BlendChannel(origin.R, shellBackground.R, origin.A),
                        BlendChannel(origin.G, shellBackground.G, origin.A),
                        BlendChannel(origin.B, shellBackground.B, origin.A)
                    );
            }
        }

        string ColorCtrl(Color color, bool back) =>
            $"\x1b[{(back ? 48 : 38)};2;{color.R};{color.G};{color.B}m";

        IEnumerable<string> Translate(Bitmap bmp, bool resetCusor) {
            if (resetCusor) { yield return "\x1b[u"; }
            yield return "\x1b[0m";
            Color? back = null, front = null;
            for (int j = 0; j < bmp.Height / 2; j++) {
                for (int i = 0; i < bmp.Width; i++) {
                    Color? c1 = BlendColor(bmp.GetPixel(i, j * 2)), c2 = BlendColor(bmp.GetPixel(i, j * 2 + 1));
                    if (c1 == null || c2 == null) {
                        if (back != null) {
                            yield return "\x1b[0m";
                            back = null;
                            front = null;
                        }
                        if (c1 == c2) {
                            yield return " ";
                            continue;
                        }
                        Color other = (c1 ?? c2).Value;
                        if (front != other) {
                            yield return ColorCtrl(other, false);
                            front = other;
                        }
                        yield return c1 == null ? "▄" : "▀";
                        continue;
                    }
                    if (c1 == c2) {
                        if (back == c1) {
                            yield return " ";
                            continue;
                        }
                        if (front != c1) {
                            yield return ColorCtrl(c1.Value, false);
                            front = c1;
                        }
                        yield return "█";
                        continue;
                    }
                    if (back == c2 || front == c1) {
                        if (back != c2) {
                            yield return ColorCtrl(c2.Value, true);
                            back = c2;
                        }
                        if (front != c1) {
                            yield return ColorCtrl(c1.Value, false);
                            front = c1;
                        }
                        yield return "▀";
                        continue;
                    }
                    if (back != c1) {
                        yield return ColorCtrl(c1.Value, true);
                        back = c1;
                    }
                    if (front != c2) {
                        yield return ColorCtrl(c2.Value, false);
                        front = c2;
                    }
                    yield return "▄";
                }
                yield return "\n";
            }
            yield return "\x1b[0m\n";
        }
    }
}
