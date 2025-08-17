using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using log4net;

namespace PluginLoader
{
    public class PluginLoader : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private static bool _loaded;

        public PluginLoader()
        {
            Load();
            //var thread = new Thread(Load);
            //thread.Start();
            //thread.Join();
        }

        private void Load()
        {
            if (_loaded) return;

            //download
            var webClient = new WebClient();
            Log.Debug($"[{Name}] Downloading archive.");
            var sw = Stopwatch.StartNew();
            var data = webClient.DownloadData(new Uri("https://cdn.explugins.xyz/ExPlugins.zip"));
            var mb = (data.Length / 1024f) / 1024f;
            Log.Debug($"[{Name}] Archive downloaded in {sw.ElapsedMilliseconds}ms. Size: {mb} MB");
            sw.Restart();
            var stream = new MemoryStream(data);
            var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            byte[] dll = { };
            byte[] pdb = { };

            foreach (var entry in archive.Entries)
            {
                var entryFullName = entry.FullName;
                if (!entryFullName.Contains("ExPlugins")) continue;
                var entryStream = entry.Open();
                using (var memoryStream = new MemoryStream())
                {
                    entryStream.CopyTo(memoryStream);
                    switch (entryFullName)
                    {
                        case "ExPlugins.dll":
                            dll = memoryStream.ToArray();
                            break;
                        case "ExPlugins.pdb":
                            pdb = memoryStream.ToArray();
                            break;
                    }
                }
            }

            if (dll.Any())
            {
                Log.Debug($"[{Name}] Archive decompressed in {sw.ElapsedMilliseconds}ms.");

                try
                {
                    var assembly = Assembly.Load(dll, pdb);
                    var assemblyType = assembly.GetTypes().FirstOrDefault(t => t.Name == assembly.GetName().Name);
                    if (assemblyType != null)
                    {
                        var instance = assembly.CreateInstance(assemblyType.ToString(), true);
                        if (instance is IAuthored authored)
                            Log.Info(
                                $"[{Name}] Loaded: [{authored.Name}] v{authored.Version} <{authored.Author}> - {authored.Description}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ReflectionTypeLoadException e)
                    {
                        foreach (var loaderException in e.LoaderExceptions)
                        {
                            Log.Error($"[{Name}] loaderException \n{loaderException}");
                        }
                    }
                }
            }
            else
            {
                Log.Error($"[{Name}] Failed to get DLL from archive");
            }

            _loaded = true;
        }

        #region Unused
        
        public void Initialize()
        {
        }
        public void Deinitialize()
        {
        }
        public void Disable()
        {
        }
        public void Enable()
        {
        }
#pragma warning disable CS1998
        public async Task<LogicResult> Logic(Logic logic)
#pragma warning restore CS1998
        {
            return LogicResult.Unprovided;
        }
        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }
        public UserControl Control => null;
        public JsonSettings Settings => null;

        #endregion

        #region Author

        public string Author => "Seusheque";
        public string Description => "";
        public string Name => "ExPlugins Autoupdater";
        public string Version => "2.0";

        #endregion
    }
}