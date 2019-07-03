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
        private Thread thread;
        private Action<InspectorStage, object> onUpdate;

        private readonly List<string> codeExtensions = new List<string>
        {
            //Backend
            ".php", ".py", ".go", ".htaccess",      
            // Frontend & UI
            ".html", ".htm", ".css", ".xaml", ".js", 
            // .NET
            ".cs", ".vb",
            // Java
            ".java", ".kt",
            // Low-level
            ".h", ".cpp", ".hpp", ".c", ".s",
            // Chaders
            ".vs", ".fs", ".shader",
        };

        private readonly List<string> blackList = new List<string>
        {
            ".i.g.cs", ".g.i.cs", ".i.cs", ".g.cs", ".Designer.cs", "AssemblyInfo.cs",
        };

        public Inspector()
        {
            
        }

        public void Start(List<Project> projects, Action<InspectorStage, object> onUpdate)
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

        private delegate void UpdateDelegate(InspectorStage stage, object info);

        private void Update(InspectorStage stage, object info)
        {
            onUpdate?.Invoke(stage, info);
        }

        private void AsyncCompleted(IAsyncResult resObj)
        {
            string message = (string)resObj.AsyncState;
            Console.WriteLine(message);
            Console.WriteLine("Работа асинхронного делегата завершена");
            //onCallbackComplete?.Invoke(message);
        }

        private void Process(List<Project> projects)
        {
            void ProcessUpdate(InspectorStage stage, object info)
            {
                UpdateDelegate update = new UpdateDelegate(Update);
                IAsyncResult result = update.BeginInvoke(stage, info, new AsyncCallback(AsyncCompleted), "");
            }
            //
            List<string> AllFiles = new List<string>();

            int i = 0;
            foreach (var project in projects)
            {
                ProcessUpdate(InspectorStage.Progress, (progress: ++i, total: projects.Count));
                //
                List<string> files = project.GetFiles(codeExtensions, blackList);
                project.Info.ExtensionsVolume.Clear();
                project.Info.FilesVolume.Clear();
                project.Info.Errors.Clear();
                //
                CodeVolume projectVolume = new CodeVolume();
                //int filesCount = 0;

                if (files.Count == 0)
                {
                    project.Info.Error("This project has no files to analyse");
                }

                int j = 0;
                foreach (string file in files)
                {
                    ProcessUpdate(InspectorStage.Progress2, (progress: ++j, total: files.Count));
                    //
                    if (!AllFiles.Contains(file))
                    {
                        AllFiles.Add(file);

                        ProcessUpdate(InspectorStage.FetchingFiles, AllFiles.Count);
                    }
                    else
                        project.Info.Error($"File '{file}' already has added");
                }

                j = 0;
                foreach (string file in files)
                {
                    ProcessUpdate(InspectorStage.Progress2, (progress: ++j, total: files.Count));
                    ProcessUpdate(InspectorStage.FetchingLines, projectVolume.Lines);
                    //
                    if (File.Exists(file))
                    {
                        // Geting data
                        string data = "";
                        try
                        {
                            data = File.ReadAllText(file);
                            //
                            long newLastEdit = UnixTime.ToTimestamp(File.GetLastWriteTime(file));
                            project.LastEdit = Math.Max(project.LastEdit, newLastEdit);
                        }
                        catch(Exception ex)
                        {
                            project.Info.Error($"File '{file}' thrown {ex.GetType().Name}");
                        }
                        // Calculating lines
                        string localPath = file.Replace(project.Path, "");
                        string[] lines = data.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        int linesCount = lines.Length;
                        int sloc = 0;

                        bool stop = false;
                        foreach (string line in lines)
                        {
                            string L = line.Trim();
                            //
                            if (L.StartsWith("/*")) stop = true;
                            if (L.EndsWith("*/") && stop) stop = false;
                            //
                            if (!stop)
                            {
                                if (L.StartsWith("//") || L.StartsWith("#")) continue;
                                if ("{}()[]".Contains(L)) continue;
                                sloc++;
                            }
                        }
                        //
                        var volume = new CodeVolume(sloc, linesCount, 1);
                        projectVolume += volume;
                        //

                        string ext = Path.GetExtension(file);

                        // Пушим список расширений
                        var list_ext = project.Info.ExtensionsVolume;
                        if (list_ext.ContainsKey(ext))
                            list_ext[ext] += volume;
                        else
                            list_ext.Add(ext, volume);
                        // Пушим список файлов
                        var list_files = project.Info.FilesVolume;
                        if (list_files.ContainsKey(localPath))
                            list_files[localPath] = volume;
                        else
                            list_files.Add(localPath, volume);
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
            Thread.Sleep(500);
            ProcessUpdate(InspectorStage.Completed, new ProcessEndData {
                projects = projects,
            });

            GC.Collect();
        }

        public struct ProcessEndData
        {
            public List<Project> projects;
        }
    }
}
