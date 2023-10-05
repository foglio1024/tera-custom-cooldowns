using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.WinAPI;
using TCC.Utils.Exceptions;
using TeraPacketParser.Analysis;

namespace TCC.Utilities;

public static class ExceptionReportBuilder
{
    public static JObject BuildJsonCrashReport(Exception ex)
    {
        var ret = new JObject
        {
            { "timestamp" , new JValue(DateTime.UtcNow.ToEpoch()) },
            { "tcc_version" , new JValue(App.AppVersion) },
            { "user_id" , new JValue(App.Settings.LastAccountNameHash) },
            { "tcc_hash", HashUtils.GenerateFileHash(typeof(App).Assembly.Location) },
            { "exception", BuildExceptionMessage(ex)},
            { "exception_type", new JValue(ex.GetType().FullName)},
            { "exception_source", new JValue(ex.Source)},
            { "stack_trace", new JValue(ex.StackTrace)},
            { "game_version", new JValue(PacketAnalyzer.Factory == null ? 0 : PacketAnalyzer.Factory.ReleaseVersion)},
            { "region", new JValue(Game.Server.Region)},
            { "server_id", new JValue(Game.Server.ServerId.ToString())},
            { "settings_summary", new JObject
                {
                    { "windows", new JObject
                        {
                            { "cooldown", App.Settings.CooldownWindowSettings.Enabled },
                            { "buffs", App.Settings.BuffWindowSettings.Enabled },
                            { "character", App.Settings.CharacterWindowSettings.Enabled },
                            { "class", App.Settings.ClassWindowSettings.Enabled },
                            { "chat", App.Settings.ChatEnabled},
                            { "npc", App.Settings.NpcWindowSettings.Enabled},
                            { "group", App.Settings.GroupWindowSettings.Enabled }
                        }
                    },
                    {
                        "generic", new JObject
                        {
                            { "proxy_enabled", App.Settings.EnableProxy },
                            { "mode", App.ToolboxMode ? "toolbox" : "standalone" }
                        }
                    }
                }
            },
            {
                "stats", new JObject
                {
                    { "os", $"{Environment.OSVersion} {Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")?.GetValue("ProductName")}" },
                    { "current_USER_objects" , User32.GetGuiResources(Process.GetCurrentProcess().Handle, 1) },
                    { "used_memory" , Process.GetCurrentProcess().PrivateMemorySize64 },
                    { "uptime", (DateTime.Now  - Process.GetCurrentProcess().StartTime).TotalSeconds}
                }
            }
        };
        if (ex.InnerException != null)
        {
            var innEx = BuildInnerExceptionJObject(ex.InnerException);
            ret["inner_exception"] = innEx;
        }

        switch (ex)
        {
            case PacketParseException ppe:
                ret.Add("packet_opcode_name", new JValue(ppe.OpcodeName));
                ret.Add("packet_data", new JValue(ppe.RawData.ToHexString()));
                break;

            case DeadlockException de:
                ret.Add("thread_traces", GetThreadTraces(de));
                break;
        }

        return ret;
    }

    static JObject BuildInnerExceptionJObject(Exception ex)
    {
        var ret = new JObject
        {
            ["exception"] = BuildExceptionMessage(ex),
            ["exception_type"] = new JValue(ex.GetType().FullName),
            ["exception_source"] = new JValue(ex.Source),
            ["stack_trace"] = new JValue(ex.StackTrace)
        };
        if (ex.InnerException == null) return ret;
        var innEx = BuildInnerExceptionJObject(ex.InnerException);
        ret["inner_exception"] = innEx;
        return ret;
    }

    static string GetStackTraceClrmd(ClrThread runtimeThread)
    {
        var sb = new StringBuilder();
        foreach (var frame in runtimeThread.EnumerateStackTrace())
        {
            if (frame.Method == null) continue;
            sb.AppendLine($"   in {frame.Method}");
        }

        return sb.ToString();
    }

    static JArray GetThreadTraces(DeadlockException de)
    {
        using var dataTarget = DataTarget.CreateSnapshotAndAttach(Process.GetCurrentProcess().Id);

        var version = dataTarget.ClrVersions[0];
        var runtime = version.CreateRuntime();

        var ret = new JArray();
        var threads = App.RunningDispatchers.Values.Append(App.BaseDispatcher).Where(d => de.ThreadNames.Contains(d.Thread.Name)).Select(d => d.Thread).Append(PacketAnalyzer.AnalysisThread);
        foreach (var thread in threads)
        {
            if (thread.Name == null) continue;
            var runtimeThread = runtime.Threads.FirstOrDefault(t => t.ManagedThreadId == thread.ManagedThreadId);
            if (runtimeThread != null)
            {
                ret.Add(new JObject
                {
                    {"thread_name" , thread.Name},
                    {"stack_trace", GetStackTraceClrmd(runtimeThread)}
                });
            }
        }
        return ret;
    }

    static JValue BuildExceptionMessage(Exception ex)
    {
        if (ex is Win32Exception w32ex)
        {
            return new JValue($"{ex.Message} HRESULT:{w32ex.HResult} ErrorCode:{w32ex.ErrorCode} NativeCode:{w32ex.NativeErrorCode}");
        }

        return new JValue(ex.Message);
    }
}