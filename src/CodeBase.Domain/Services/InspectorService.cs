using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using CodeBase.Domain.Models;
using CodeBase.Shared;

namespace CodeBase.Domain.Services
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
        ProgressPrimary,
        ProgressSecondary,
        FetchingFiles,
        FetchingLines,
    }

    public class InspectorConfig
    {
        public static readonly HashSet<string> IgnoredFilesList = new()
        {
            ".i.g.cs",
            ".g.i.cs",
            ".i.cs",
            ".g.cs",
            ".Designer.cs",
            "AssemblyInfo.cs",
        };
    }

    public class InspectorService
    {
        public event InspectStartHandler OnStart;
        public event InspectUpdateHandler OnUpdate;
        public event InspectCompleteHandler OnComplete;

        private Thread _thread;

        private readonly FileProvider _fileProvider;

        public InspectorService()
        {
            _fileProvider = new FileProvider();
        }

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
            var extentions = Languages.Get().SelectMany(x => x.Extensions).ToHashSet();
            var ignoredFiles = InspectorConfig.IgnoredFilesList;

            void ProcessUpdate(InspectorStage stage, InspectState state)
            {
                dispatcher.Invoke(() => OnUpdate?.Invoke(stage, state));
            }
            //
            var allFiles = new List<string>();

            int i = 0, j = 0;

            foreach (var project in projects)
            {
                ProcessUpdate(InspectorStage.ProgressPrimary, new InspectState
                {
                    Used = ++i,
                    All = projects.Count
                });
                //
                var files = _fileProvider
                    .GetFilesData(project, extentions, ignoredFiles, (files, dirs, cur) =>
                    {
                        ProcessUpdate(InspectorStage.ProgressSecondary, new InspectState
                        {
                            Used = Math.Min(files, dirs),
                            All = Math.Max(files, dirs)
                        });
                    });

                project.Info.Clear();
                //
                var projectVolume = new CodeVolume();

                if (!files.Any())
                {
                    project.Info.Error("This project has no files to analyse");
                }

                j = 0;
                foreach (var file in files)
                {
                    if (j % 10 == 0)
                    {
                        ProcessUpdate(InspectorStage.ProgressSecondary, new InspectState
                        {
                            Used = j,
                            All = files.Count()
                        });
                    }
                    j++;
                    //
                    if (!allFiles.Contains(file))
                    {
                        allFiles.Add(file);

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
                        project.Info.Error($"File '{file}' already has added");
                    }
                }

                j = 0;
                foreach (var file in files)
                {
                    if (j % 10 == 0)
                    {
                        ProcessUpdate(InspectorStage.ProgressSecondary, new InspectState
                        {
                            Used = j,
                            All = files.Count()
                        });

                        ProcessUpdate(InspectorStage.FetchingLines, new InspectState
                        {
                            All = projectVolume.Lines
                        });
                    }
                    j++;

                    if (File.Exists(file))
                    {
                        // Geting data
                        var data = "";
                        try
                        {
                            data = File.ReadAllText(file);
                            //
                            var newLastEdit = File.GetLastWriteTime(file);
                            if (project.LastRevision < newLastEdit)
                            {
                                project.LastRevision = newLastEdit;
                            }
                        }
                        catch (Exception ex)
                        {
                            project.Info.Error($"File '{file}' thrown {ex.GetType().Name}");
                        }
                        // Calculating lines
                        var localPath = file.StartsWith(project.Location) ? file[project.Location.Length..] : file;
                        var lines = data.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var linesCount = lines.Length;
                        var sloc = 0;

                        var skip = false;
                        foreach (var line in lines)
                        {
                            var s = line.Trim();

                            if (s.StartsWith("/*"))
                            {
                                skip = true;
                            }

                            if (s.EndsWith("*/") && skip)
                            {
                                skip = false;
                            }

                            if (skip) continue;

                            if (s.StartsWith("//") || s.StartsWith("#")) continue;
                            if ("{}()[]".Contains(s)) continue;
                            sloc++;
                        }
                        //
                        var volume = new CodeVolume(sloc, linesCount, 1);
                        projectVolume += volume;
                        //

                        var ext = Path.GetExtension(file);

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
