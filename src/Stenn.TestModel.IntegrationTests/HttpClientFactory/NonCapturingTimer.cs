namespace Stenn.TestModel.IntegrationTests.HttpClientFactory
{
    internal static class NonCapturingTimer
    {
        public static Timer Create(
            TimerCallback callback,
            object state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof (callback));
            bool flag = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    flag = true;
                }
                return new Timer(callback, state, dueTime, period);
            }
            finally
            {
                if (flag)
                    ExecutionContext.RestoreFlow();
            }
        }
    }
}