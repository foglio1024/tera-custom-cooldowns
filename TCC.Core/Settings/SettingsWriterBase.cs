namespace TCC.Settings
{
    public abstract class SettingsWriterBase
    {
        protected string FileName = "";

        public abstract void Save();
    }
}
