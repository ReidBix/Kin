using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace Anim_Reader {
    class Prgm {
        public static bool debug = false;
        public static string baseLoc;
        static Prgm() {
            baseLoc = (debug) ? "C:/Users/Jay/OneDrive/Documents/UVA/SGD/Kin/Kin/Gifs"
                : Directory.GetCurrentDirectory();
        }
        static void Main(string[] args) {
            AseReader.extractAse();
        }

        public static void pause() {
            Console.ReadLine();
        }
        public static void pause(string txt) {
            Console.WriteLine(txt);
            pause();
        }
    }

    class AnimImporter {
        static void readData() {
            DirectoryInfo baseDir = new DirectoryInfo(Prgm.baseLoc);
            FileInfo[] files = baseDir.GetFiles();
            foreach (FileInfo f in files) {
                if (f.Extension.Equals(".txt")) {
                    getShit(f);
                    //break;
                }
            }

            Prgm.pause("Completed.");
        }

        static void getShit(FileInfo f) {
            string filename = f.FullName;
            Console.WriteLine("loading Clip data from: " + f.Name);

            string[] lines = File.ReadAllLines(filename)[0].
                Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int frameCount = lines.Length;
            int[] frmDurs = new int[frameCount];
            for (int i = 0; i < frameCount; i++)
                frmDurs[i] = int.Parse(lines[i].Trim());

            int start = 0, len = 4;
            int[] frames = frmDurs.SubArray(start, len);

            for (int i = 0; i < frames.Length; i++)
                Console.WriteLine("frames[" + i + "]: " + frames[i]);

            bool dynamicRate = false;
            foreach (int i in frames)
                if (i != frames[0]) {
                    dynamicRate = true;
                    break;
                }

            float l0 = dynamicRate ? GCD(frames) : frames[0],
            s0 = (int)Math.Round(1000f / l0);
            Console.WriteLine("l0: " + l0);

            // for each frame, place event at sample index
            int end = (dynamicRate) ? frames.Length + 1 : frames.Length;
            for (int i0 = 0; i0 < end; i0++) {
                int sample = i0;
                if (dynamicRate) {
                    sample = (int)(sum(frames, i0) / l0);
                    if ((i0 == frames.Length)) sample--;
                }

                Console.WriteLine("f[" + i0 + "]: " + sample);
            }
        }

        static int sum(int[] f, int i0) {
            int sum = 0;
            for (int i = 0; i < i0; i++) sum += f[i];
            return sum;
        }

        static int GCD(int[] numbers) {
            return numbers.Aggregate(GCD);
        }

        static int GCD(int a, int b) {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
    class Gif_Decoder {
        
        /// <summary>
        /// updates frame durations from gif List based on whether or not it
        /// </summary>
        static void extractFrameDurs() {
            DirectoryInfo baseDir = new DirectoryInfo(Prgm.baseLoc);

            DateTime lastSave = DateTime.Now.ToUniversalTime(); string newName = Prgm.baseLoc + "/lastExport.amsc";
            if (!File.Exists(newName)) {
                using (StreamWriter f = new StreamWriter(newName)) {
                    f.Write(lastSave);
                }
            } else {
                lastSave = DateTime.Parse(File.ReadAllLines(newName)[0]);
            }

            FileInfo[] files = baseDir.GetFiles();
            if (files.Length == 0) {
                Prgm.pause("There are no files to convert.");
            } else {
                foreach (FileInfo file in files) {
                    if (file.Extension.Equals(".gif")) {
                        bool toWrite = (File.Exists(Prgm.baseLoc + "/" + file.Name.Replace(file.Extension, "") + ".txt")) ?
                            toWrite = lastSave < file.LastWriteTime : true;
                        if (!toWrite) continue;

                        Image gif = Image.FromFile(Prgm.baseLoc + "/" + file.Name);
                        FrameDimension dim = new FrameDimension(gif.FrameDimensionsList[0]);
                        int frameCount = gif.GetFrameCount(dim);

                        PropertyItem item = gif.GetPropertyItem(0x5100);
                        float delay; List<int> frmDurs = new List<int>();

                        for (int frm = 0; frm < frameCount * 4; frm += 4) {
                            delay = (item.Value[frm] + item.Value[frm + 1] * 256) * 10;
                            if (delay != 0) frmDurs.Add((int)delay);
                        }

                        Console.WriteLine("Extracting from Img: " + file.Name + ".....");
                        using (StreamWriter f = new StreamWriter(Prgm.baseLoc + "/" + file.Name.Replace(file.Extension, "") + file.Extension)) {
                            foreach (int dur in frmDurs)
                                f.Write(dur + " ");
                        }
                    }
                }
                Prgm.pause("Finished Extraction.");
            }
        }
    }
    static class LinqHelper {
        public static T[] SubArray<T>(this T[] data, int index, int length) {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    static class AseReader {
        static string kinLoc, asepriteLoc;
        static AseReader() {
            kinLoc = Prgm.baseLoc.Substring(0, Prgm.baseLoc.LastIndexOf("Kin"+
                (Prgm.baseLoc.Contains("/") ? "/" : "\\")));
            asepriteLoc = "C:/Program Files (x86)/Aseprite/";
        }
        public static void extractAse() {
            while (true) {
                Console.WriteLine("Is aseprite installed in the default location? (y/n)");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.KeyChar.ToString().ToLower().Equals("y")) {
                    break;
                } else if (key.KeyChar.ToString().ToLower().Equals("n")) {
                    bool fin = false;
                        Console.WriteLine("Enter the location of \"aseprite.exe\" .");
                    while (!fin) {
                        asepriteLoc = Console.ReadLine();
                        if (File.Exists(asepriteLoc + "aseprite.exe")) {
                            fin = true;
                            break;
                        } else {
                            Console.WriteLine("Could not find aseprite.exe at that location.\nPlease enter a correct location.");
                        }
                    }
                    if (fin) break;
                } else
                    Console.WriteLine("Enter (y/n).");
            }

            Console.WriteLine();
            DateTime lastSave = DateTime.Now.ToUniversalTime(); string newName = Prgm.baseLoc + "/lastExport.amsc";
            if (!File.Exists(newName)) {
                using (StreamWriter f = new StreamWriter(newName)) {
                    f.Write(lastSave);
                }
            } else {
                lastSave = DateTime.Parse(File.ReadAllLines(newName)[0]);
            }

            DirectoryInfo baseDir = new DirectoryInfo(kinLoc+"Art");
            foreach(FileInfo file in baseDir.GetFiles()) {
                if (file.Extension.Equals(".ase")) {
                    bool toWrite = (File.Exists(Prgm.baseLoc + "/" + file.Name.Replace(file.Extension, "") + file.Extension)) ?
                        toWrite = lastSave < file.LastWriteTime : true;
                    if (!toWrite) continue;

                    extractAse(file.Name.Replace(file.Extension,""));
                }
            }

            Prgm.pause("Finished Extraction.");
        }

        public static void extractAse(string aseName) {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = asepriteLoc + "aseprite.exe";
            startInfo.Arguments = "-b --list-tags --ignore-empty --data \""+
                Prgm.baseLoc+"/"+ aseName +".json\" \"" + kinLoc + "Art/"+ aseName +".ase";
            process.StartInfo = startInfo;
            process.Start();
        }
    }

    public class Clip {
        public int start, len;
        public int[] frmDurs;

    }
}


