using Microsoft.WindowsAzure.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient
{
    /// <summary>
    /// Extension Methods for a task
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <typeparam name="T">Return type of the task</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns>A task</returns>
        public static Task<T> HandleException<T>(this Task<T> task, Func<AggregateException, ResourceProviderClientException> exceptionHandler)
        {
            return task.ContinueWith<T>(
                e =>
                {
                    if (e.IsFaulted)
                    {
                        throw exceptionHandler(e.Exception);
                    }

                    return e.Result;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        //public static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return task.ThenImpl(t => continuation(t.Result), cancellationToken);
        //}

        //public static Task<TResult> ContinueWithContext<TResult>(
        //    this Task task,
        //    Func<Task, TResult> continuation,
        //    CancellationToken cancellationToken = default(CancellationToken),
        //    TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        //{
        //    var httpContext = HttpContext.Current;
        //    return task.ContinueWith(
        //        t =>
        //        {
        //            HttpContext.Current = httpContext;
        //            if (httpContext != null)
        //            {
        //                Thread.CurrentPrincipal = httpContext.User;
        //            }

        //            RequestCorrelationContext.ApplyCultureSettings();

        //            return continuation(t);
        //        },
        //        cancellationToken,
        //        continuationOptions,
        //        TaskScheduler.Default);
        //}

        //private static Task<TOuterResult> ThenImpl<TTask, TOuterResult>(this TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken)
        //    where TTask : Task
        //{
        //    // Stay on the same thread if we can
        //    if (task.IsCanceled || cancellationToken.IsCancellationRequested)
        //    {
        //        return TaskHelpers.Canceled<TOuterResult>();
        //    }

        //    if (task.IsFaulted)
        //    {
        //        return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
        //    }

        //    if (task.Status == TaskStatus.RanToCompletion)
        //    {
        //        try
        //        {
        //            return continuation(task);
        //        }
        //        catch (Exception ex)
        //        {
        //            return TaskHelpers.FromError<TOuterResult>(ex);
        //        }
        //    }

        //    return task.ContinueWithContext(
        //    innerTask =>
        //    {
        //        if (innerTask.IsFaulted)
        //        {
        //            return TaskHelpers.FromErrors<TOuterResult>(innerTask.Exception.InnerExceptions);
        //        }

        //        if (innerTask.IsCanceled)
        //        {
        //            return TaskHelpers.Canceled<TOuterResult>();
        //        }

        //        TaskCompletionSource<Task<TOuterResult>> tcs = new TaskCompletionSource<Task<TOuterResult>>();

        //        try
        //        {
        //            tcs.TrySetResult(continuation(task));
        //        }
        //        catch (Exception ex)
        //        {
        //            tcs.TrySetException(ex);
        //        }

        //        return tcs.Task.FastUnwrap();
        //    },
        //    cancellationToken).FastUnwrap();
        //}
    }
}
