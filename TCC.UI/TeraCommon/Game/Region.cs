// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Tera.Game
{
    public class Region
    {
        public Region(string key, string version)
        {
            Key = key;
            Version = version;
        }

        public string Key { get; private set; }
        public string Version { get; private set; }
    }
}