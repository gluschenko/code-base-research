using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CodeBase
{
    public enum InspectorStage
    {
        Progress,
        Progress2,
        FetchingFiles,
        FetchingLines,
        Completed,
    }

    public class Inspector
    {
        public delegate void StageUpdate(InspectorStage stage, object info);

        private Thread thread;
        private StageUpdate onUpdate;

        public void Start(List<Project> projects, StageUpdate onUpdate)
        {
            this.onUpdate = onUpdate;
            //
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }

            thread = new Thread(() => Process(projects)) { IsBackground = true };
            thread.Start();
        }

        private void Update(InspectorStage stage, object info)
        {
            onUpdate?.Invoke(stage, info);
        }

        private void AsyncCompleted(IAsyncResult resObj)
        {
            string message = (string)resObj.AsyncState;
            Console.WriteLine(message);
            Console.WriteLine("Работа асинхронного делегата завершена");
        }

        private void Process(List<Project> projects)
        {
            void ProcessUpdate(InspectorStage stage, object info)
            {
                StageUpdate update = new StageUpdate(Update);
                IAsyncResult result = update.BeginInvoke(stage, info, new AsyncCallback(AsyncCompleted), "");
            }
            //
            List<string> AllFiles = new List<string>();

            int i = 0, j = 0;

            foreach (var project in projects)
            {
                ProcessUpdate(InspectorStage.Progress, (progress: ++i, total: projects.Count));
                //
                var files = project.GetFiles(InspectorConfig.CodeExtensions, InspectorConfig.FilesBlackList);

                project.Info.Clear();
                //
                CodeVolume projectVolume = new CodeVolume();
                //int filesCount = 0;

                if (files.Count == 0)
                {
                    project.Info.Error("This project has no files to analyse");
                }

                j = 0;
                foreach (var file in files)
                {
                    ProcessUpdate(InspectorStage.Progress2, (progress: ++j, total: files.Count));
                    //
                    if (!AllFiles.Contains(file.Path))
                    {
                        AllFiles.Add(file.Path);
                        
                        ProcessUpdate(InspectorStage.FetchingFiles, AllFiles.Count);
                    }
                    else
                        project.Info.Error($"File '{file.Path}' already has added");
                }

                j = 0;
                foreach (var file in files)
                {
                    if (!file.IsMatch) continue;
                    //
                    ProcessUpdate(InspectorStage.Progress2, (progress: ++j, total: files.Count));
                    ProcessUpdate(InspectorStage.FetchingLines, projectVolume.Lines);

                    //
                    if (File.Exists(file.Path))
                    {
                        // Geting data
                        string data = "";
                        try
                        {
                            data = File.ReadAllText(file.Path);
                            //
                            long newLastEdit = UnixTime.ToTimestamp(File.GetLastWriteTime(file.Path));
                            project.LastEdit = Math.Max(project.LastEdit, newLastEdit);
                        }
                        catch(Exception ex)
                        {
                            project.Info.Error($"File '{file.Path}' thrown {ex.GetType().Name}");
                        }
                        // Calculating lines
                        string localPath = file.Path.StartsWith(project.Path) ? file.Path.Substring(project.Path.Length) : file.Path;
                        string[] lines = data.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        int linesCount = lines.Length;
                        int sloc = 0;

                        bool skip = false;
                        foreach (string line in lines)
                        {
                            string s = line.Trim();
                            //
                            if (s.StartsWith("/*")) skip = true;
                            if (s.EndsWith("*/") && skip) skip = false;
                            //
                            if (!skip)
                            {
                                if (s.StartsWith("//") || s.StartsWith("#")) continue;
                                if ("{}()[]".Contains(s)) continue;
                                sloc++;
                            }
                        }
                        //
                        var volume = new CodeVolume(sloc, linesCount, 1);
                        projectVolume += volume;
                        //

                        string ext = Path.GetExtension(file.Path);

                        // Пушим список расширений
                        project.Info.ExtensionsVolume.Push(ext, volume, (A, B) => A + B);
                        // Пушим список файлов
                        project.Info.FilesVolume.Push(localPath, volume);
                    }
                    else
                    {
                        project.Info.Error($"{file} doesn't exist");
                    }
                }

                project.Info.Volume = projectVolume;
                //project.Info.Files = filesCount;
            }
            //

            //
            Thread.Sleep(300);
            ProcessUpdate(InspectorStage.Completed, new ProcessEndData(projects));

            GC.Collect();
        }

        public struct ProcessEndData
        {
            public List<Project> projects;

            public ProcessEndData(List<Project> projects)
            {
                this.projects = projects;
            }
        }
    }
}
