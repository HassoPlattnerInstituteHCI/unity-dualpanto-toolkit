using System.Threading.Tasks;
using System.Threading;

public class PantoIntroPause : PantoIntroBase
{
    public float durationInSeconds = 1f;

    private CancellationTokenSource cancelTokenSrc = new CancellationTokenSource();

    public override async Task Introduce()
    {
        try
        {
            await Task.Delay((int)durationInSeconds * 1000, cancellationToken: cancelTokenSrc.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }
    }

    public override void CancelIntro()
    {
        cancelTokenSrc.Cancel();
    }
}
