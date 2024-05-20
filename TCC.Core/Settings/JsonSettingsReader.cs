using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Nostrum.WPF.Extensions;
using TCC.UI.Windows;
using TCC.Utilities;
using TCC.Utils;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Settings;

/// <summary>
/// Special JsonConvert resolver that allows you to ignore properties. See https://stackoverflow.com/a/13588192/1037948
/// </summary>
public class JsonIgnoreResolver : DefaultContractResolver
{
    private readonly Dictionary<Type, HashSet<string>> _ignores = new();

    /// <summary>
    /// Explicitly ignore the given property(s) for the given type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
    public void Ignore(Type type, params string[] propertyName)
    {
        // start bucket if DNE
        if (!_ignores.ContainsKey(type)) _ignores[type] = new HashSet<string>();

        foreach (var prop in propertyName)
        {
            _ignores[type].Add(prop);
        }
    }

    /// <summary>
    /// Is the given property for the given type ignored?
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private bool IsIgnored(Type type, string propertyName)
    {
        if (!_ignores.TryGetValue(type, out var props)) return false;

        // if no properties provided, ignore the type entirely
        return props.Count == 0
            || props.Contains(propertyName);
    }

    /// <summary>
    /// The decision logic goes here
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberSerialization"></param>
    /// <returns></returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (!IsIgnored(property.DeclaringType!, property.PropertyName!)) return property;
        property.ShouldSerialize = _ => false;
        property.ShouldDeserialize = _ => false;
        property.Ignored = true;

        return property;
    }
}
public class JsonSettingsReader : SettingsReaderBase
{

    public JsonSettingsReader()
    {
        FileName = SettingsGlobals.SettingsFileName;

    }

    public SettingsContainer LoadSettings(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);
                #region Compatibility
                file = file.Replace("\"TabName\"", "\"Name\"")
                    .Replace("\"ExcludedAuthors\"", "\"HiddenAuthors\"")
                    .Replace("\"ExcludedChannels\"", "\"HiddenChannels\"")
                    .Replace("\"Channels\"", "\"ShowedChannels\"")
                    .Replace("\"Authors\"", "\"ShowedAuthors\"")
                    .Replace("\"LanguageOverride\": \"\"", "\"LanguageOverride\" : 0");

                #endregion
                var ret = JsonConvert.DeserializeObject<SettingsContainer>(file, TccUtils.GetDefaultJsonSerializerSettings())!;

                #region Compatibility

#pragma warning disable CS0612 // Il tipo o il membro è obsoleto

                if (ret.StatSentVersion != App.AppVersion)
                {
                    foreach (var special in ret.BuffWindowSettings.Specials)
                    {
                        ret.AbnormalitySettings.Favorites.Add(special);
                    }
                    ret.BuffWindowSettings.Specials.Clear();

                    foreach (var hidden in ret.BuffWindowSettings.Hidden)
                    {
                        ret.AbnormalitySettings.Self.Collapsible.Add(hidden);
                    }
                    ret.BuffWindowSettings.Hidden.Clear();

                    foreach (var hidden in ret.GroupWindowSettings.Hidden)
                    {
                        ret.AbnormalitySettings.Group.Collapsible.Add(hidden);
                    }
                    ret.GroupWindowSettings.Hidden.Clear();

                    foreach (var (cl, list) in ret.BuffWindowSettings.MyAbnormals)
                    {
                        ret.AbnormalitySettings.Self.Whitelist[cl] = list;
                    }
                    ret.BuffWindowSettings.MyAbnormals.Clear();

                    foreach (var (cl, list) in ret.GroupWindowSettings.GroupAbnormals)
                    {
                        ret.AbnormalitySettings.Group.Whitelist[cl] = list;
                    }
                    ret.GroupWindowSettings.GroupAbnormals.Clear();

                    ret.AbnormalitySettings.Self.ShowAll = ret.BuffWindowSettings.ShowAll;
                    ret.AbnormalitySettings.Group.ShowAll = ret.GroupWindowSettings.ShowAllAbnormalities;
                }

#pragma warning restore CS0612 // Il tipo o il membro è obsoleto

                #endregion

                return ret;
            }
            //                else
            //                {
            //#if false
            //                    var res = TccMessageBox.Show(SR.SettingsNotFoundImport, MessageBoxType.ConfirmationWithYesNo);
            //                    if (res == MessageBoxResult.No)
            //                    {
            //                        App.Settings = new SettingsContainer();
            //                        return;
            //                    }
            //                    var diag = new OpenFileDialog
            //                    {
            //                        Title = $"Import TCC settings file ({FileName})",
            //                        Filter = $"{FileName} (*.json)|*.json"
            //                    };
            //                    if (diag.ShowDialog() == true)
            //                    {
            //                        path = diag.FileName;
            //                        LoadSettings(path);
            //                    }
            //                    else App.Settings = new SettingsContainer();
            //#else
            //                    return new SettingsContainer();
            //#endif
            //                }
        }
        catch
        {
            var res = TccMessageBox.Show("TCC", SR.SettingsNotFoundDefault, MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (res == MessageBoxResult.Yes) File.Delete(path);
            LoadSettings(path);
        }
        return new SettingsContainer();
    }
}