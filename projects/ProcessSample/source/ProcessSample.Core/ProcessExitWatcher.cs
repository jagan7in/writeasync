﻿//-----------------------------------------------------------------------
// <copyright file="ProcessExitWatcher.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ProcessSample
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProcessExitWatcher : IDisposable
    {
        private readonly TaskCompletionSource<bool> exited;
        private readonly IProcessExit inner;

        private bool? savedEnableRaisingEvents;

        public ProcessExitWatcher(IProcessExit inner)
        {
            this.exited = new TaskCompletionSource<bool>();
            this.inner = inner;
            this.savedEnableRaisingEvents = this.inner.EnableRaisingEvents;
            this.inner.EnableRaisingEvents = true;
            this.inner.Exited += this.OnProcessExited;
            if (this.inner.HasExited)
            {
                this.exited.TrySetResult(false);
            }
        }

        public IProcessExit Inner
        {
            get { return this.inner; }
        }

        public Task WaitForExitAsync(CancellationToken token)
        {
            if (this.savedEnableRaisingEvents == null)
            {
                throw new ObjectDisposedException("ProcessExitWatcher");
            }

            Task task = this.exited.Task;
            if (token.CanBeCanceled)
            {
                task = this.WaitForExitInnerAsync(token);
            }

            return task;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.savedEnableRaisingEvents.HasValue)
                {
                    this.inner.EnableRaisingEvents = this.savedEnableRaisingEvents.Value;
                    this.inner.Exited -= this.OnProcessExited;
                    this.exited.TrySetException(new ObjectDisposedException("ProcessExitWatcher"));
                    this.savedEnableRaisingEvents = null;
                }
            }
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            this.exited.TrySetResult(false);
        }

        private async Task WaitForExitInnerAsync(CancellationToken token)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            using (token.Register(o => ((TaskCompletionSource<bool>)o).SetResult(false), tcs))
            {
                Task completed = await Task.WhenAny(this.exited.Task, tcs.Task);
                if (completed == this.exited.Task)
                {
                    await this.exited.Task;
                }
                else
                {
                    token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
