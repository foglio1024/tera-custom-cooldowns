namespace TCC.Data;

public class ArcherFocusTracker : BaseStackBuffTracker
{
    public void StartFocus(long duration)
    {
        base.StartBaseBuff(duration);
    }
    public void SetFocusStacks(int stacks, long duration)
    {
        base.RefreshBaseBuff(stacks, duration);
    }
    public void StartFocusX(long duration)
    {
        base.StartEmpoweredBuff(duration);
    }
    public void StopFocusX()
    {
        base.Stop();
    }
    public void StopFocus()
    {
        base.Stop();
    }
}

public class LancerLineHeldTracker : BaseStackBuffTracker
{
    public LancerLineHeldTracker()
    {
        BaseStacksChanged += stacks => { if (stacks == 0) Stop(); };
    }
}