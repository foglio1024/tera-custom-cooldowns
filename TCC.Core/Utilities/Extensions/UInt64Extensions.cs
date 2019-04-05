namespace TCC.Utilities.Extensions
{
    public static class UInt64Extensions
    {
        public static bool IsMe(this ulong val)
        {
            return val == SessionManager.CurrentPlayer.EntityId;
        }
    }
}