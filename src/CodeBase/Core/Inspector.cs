using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace CodeBase
{
    public delegate void InspectStartHandler();
    public delegate void InspectUpdateHandler(InspectorStage stage, InspectState state);
    public delegate void InspectCompleteHandler();

    public struct InspectState
    {
        public int Used { get; set; }
        public int All { get; set; }
        public List<Project> Projects { get; set; }
    }

    public enum InspectorStage
    {
        Progress,
        Progress2,
        FetchingFiles,
        FetchingLines,
    }

    public class Inspector
    {
        public event InspectStartHandler OnStart;
        public event InspectUpdateHandler OnUpdate;
        public event InspectCompleteHandler OnComplete;

        private Thread _thread;

        public void Start(List<Project> projects, Dispatcher dispatcher)
        {
            Stop();

            _thread = new Thread(Run) { IsBackground = true };
            _thread.Start();

            void Run()
            {
                try
                {
                    dispatcher.Invoke(() => OnStart?.Invoke());
                    Process(projects, dispatcher);
                    dispatcher.Invoke(() => OnComplete?.Invoke());
                }
                catch (ThreadInterruptedException)
                {
                }
            }
        }

        private void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
                _thread = null;
            }
        }

        private void Process(List<Project> projects, Dispatcher dispatcher)
        {
            void ProcessUpdate(InspectorStage stage, InspectState state)
            {
                dispatcher.Invoke(() => OnUpdate?.Invoke(stage, state));
            }
            //
            var allFiles = new List<string>();

            int i = 0, j = 0;

            foreach (var project in projects)
            {
                ProcessUpdate(InspectorStage.Progress, new InspectState
                {
                    Used = ++i,
                    All = projects.Count
                });
                //
                var projectPath = project.Path.Replace('\\', '/');
                var files = project.GetFiles(InspectorConfig.CodeExtensions, InspectorConfig.FilesBlackList, (files, dirs, cur) =>
                {
                    ProcessUpdate(InspectorStage.Progress2, new InspectState
                    {
                        Used = Math.Min(files, dirs),
                        All = Math.Max(files, dirs)
                    });
                });

                project.Info.Clear();
                //
                var projectVolume = new CodeVolume();

                if (files.Count == 0)
                {
                    project.Info.Error("This project has no files to analyse");
                }

                j = 0;
                foreach (var file in files)
                {
                    if (j % 10 == 0)
                    {
                        ProcessUpdate(InspectorStage.Progress2, new InspectState
                        {
                            Used = j,
                            All = files.Count
                        });
                    }
                    j++;
                    //
                    if (!allFiles.Contains(file.Path))
                    {
                        allFiles.Add(file.Path);

                        if (allFiles.Count % 10 == 0)
                        {
                            ProcessUpdate(InspectorStage.FetchingFiles, new InspectState
                            {
                                All = allFiles.Count
                            });
                        }
                    }
                    else
                    {
                        project.Info.Error($"File '{file.Path}' already has added");
                    }
                }

                j = 0;
                foreach (var file in files)
                {
                    if (!file.IsMatch) continue;
                    //
                    if (j % 10 == 0)
                    {
                        ProcessUpdate(InspectorStage.Progress2, new InspectState
                        {
                            Used = j,
                            All = files.Count
                        });

                        ProcessUpdate(InspectorStage.FetchingLines, new InspectState
                        {
                            All = projectVolume.Lines
                        });
                    }
                    j++;

                    if (File.Exists(file.Path))
                    {
                        // Geting data
                        var data = "";
                        try
                        {
                            data = File.ReadAllText(file.Path);
                            //
                            var newLastEdit = UnixTime.ToTimestamp(File.GetLastWriteTime(file.Path));
                            project.LastEdit = Math.Max(project.LastEdit, newLastEdit);
                        }
                        catch (Exception ex)
                        {
                            project.Info.Error($"File '{file.Path}' thrown {ex.GetType().Name}");
                        }
                        // Calculating lines
                        var localPath = file.Path.StartsWith(projectPath) ? file.Path[projectPath.Length..] : file.Path;
                        var lines = data.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var linesCount = lines.Length;
                        var sloc = 0;

                        var skip = false;
                        foreach (var line in lines)
                        {
                            var s = line.Trim();
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

                        var ext = Path.GetExtension(file.Path);

                        // Пушим список расширений
                        project.Info.ExtensionsVolume.Push(ext, volume, (a, b) => a + b);
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
            GC.Collect();
        }
    }
}
