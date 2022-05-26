using System.Diagnostics;

namespace EnterpriseIntegration.TestCommon;

public static class TestHelper
{
    public static async Task WaitFor(Func<bool> condition, int checkIntervalInMilliseconds = 10, int maxWaitTimeInMilliseconds = 1000)
    {
        Stopwatch watch = Stopwatch.StartNew();
        while (!condition() && watch.ElapsedMilliseconds < maxWaitTimeInMilliseconds)
        {
            await Task.Delay(checkIntervalInMilliseconds);
        }
    }
}