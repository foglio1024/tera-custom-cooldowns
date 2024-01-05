namespace TCC.Data;

public class LancerLineHeldTracker : BaseStackBuffTracker
{
    public LancerLineHeldTracker()
    {
        BaseStacksChanged += stacks =>
        {
            if (stacks == 0) Stop();
        };
    }
}