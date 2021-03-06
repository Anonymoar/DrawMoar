﻿using System;

using System.IO;
using System.Diagnostics;

using DrawMoar.BaseElements;


namespace DrawMoar.ffmpeg
{
    public static class ExportToVideo
    {
        public static void SaveToVideo(Cartoon cartoon, string outFileFormat, string pathToMusic) {
            if (outFileFormat != "mp4" && outFileFormat != "avi") {
                throw new ArgumentException("Недопустимый формат файла");
            }
            string pathToImages = Path.Combine(Editor.cartoon.WorkingDirectory, "Images");
            Directory.CreateDirectory(pathToImages);
            var concatFilename = CreateTemporaryFiles(cartoon, pathToImages);
            int count = 0;
            while(File.Exists(Path.Combine(Editor.cartoon.WorkingDirectory, $"silentOut{count}.{outFileFormat}"))){
                count++;
            }
            Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.WorkingDirectory = pathToImages;
            process.StartInfo.Arguments = $"-y -loglevel panic -f concat -i {concatFilename} silentOut{count}.{outFileFormat}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            string readyCartoonPath = Path.Combine(Editor.cartoon.WorkingDirectory, $"silentOut{count}.{outFileFormat}");
            //if (File.Exists(readyCartoonPath))File.Delete(readyCartoonPath);     //либо так перезаписывать, либо убирать увеличение count в 21-22 строчках
            File.Move(Path.Combine(pathToImages, $"silentOut{count}.{outFileFormat}"), readyCartoonPath);
            if (File.Exists(pathToMusic) && outFileFormat == "avi") {
                AddMusic(pathToMusic, $"silentOut{count}.avi", Editor.cartoon.WorkingDirectory, count);
            }
        }


        private static string CreateTemporaryFiles(Cartoon cartoon, string path) {
            string imagesDirectory = path;
            string imagesListFilename = Path.Combine(imagesDirectory, "images.txt");

            DirectoryInfo directoryInfo = new DirectoryInfo(imagesDirectory);
            int count = 0;
            using (var writer = new StreamWriter(imagesListFilename)) {
                cartoon.scenes.ForEach(
                    scene => scene.frames.ForEach(
                    frame => {
                        frame.Join().Save(Path.Combine(imagesDirectory, $"img{count}.png"));
                        writer.WriteLine("file " + $"img{count}.png");
                        writer.WriteLine($"duration {frame.duration}".Replace(',','.'));
                        count++;
                    }
                ));
            }
            return imagesListFilename;
        }


        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            Console.WriteLine(outLine.Data);
        }


        private static void AddMusic(string pathToMusic, string cartoonName, string workingDirectory, int count) {
            File.Copy(pathToMusic, Path.Combine(workingDirectory, Path.GetFileName(pathToMusic)));
            Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = $"-i \"{cartoonName}\" -i \"{Path.GetFileName(pathToMusic)}\" " +
                                           "-codec copy -shortest " +
                                           $"\"{cartoonName}+audio{count}.avi\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;    
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
