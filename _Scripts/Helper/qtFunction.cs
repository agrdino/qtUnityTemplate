using System;
using System.Threading.Tasks;

namespace _Scripts.Helper
{
    public class qtFunction
    {
        public static async void DoSomethingAsync(Func<Task> asyncAction, Action onSuccess = null, Action<Exception> onError = null, Action onFinish = null)
        {
            try
            {
                await asyncAction();
                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
                qtLogging.LogError(e);
            }
            finally
            {
                onFinish?.Invoke();
            }
        }
    }
}