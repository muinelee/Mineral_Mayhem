using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public static class ExtensionHelper
{
    public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation op)
    {
        TaskCompletionSource<AsyncOperation> task = new TaskCompletionSource<AsyncOperation>();
        op.completed += (operation) => task.SetResult(op);
        return task.Task.GetAwaiter();
    }
}
