﻿//-----------------------------------------------------------------------
// <copyright file="AsyncOperationTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AsyncEnumSample.Test.Unit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class AsyncOperationTest
    {
        public AsyncOperationTest()
        {
        }

        [Fact]
        public void Set_result_in_ctor_and_break_completes_sync()
        {
            SetResultInCtorOperation op = new SetResultInCtorOperation(1234);
            Task<int> task = op.Start();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(1234, task.Result);
        }

        [Fact]
        public void Set_result_in_enumerator_and_break_completes_sync()
        {
            SetResultInEnumeratorOperation op = new SetResultInEnumeratorOperation(1234);
            Task<int> task = op.Start();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(1234, task.Result);
        }

        private sealed class SetResultInCtorOperation : AsyncOperation<int>
        {
            public SetResultInCtorOperation(int result)
            {
                this.Result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield break;
            }
        }

        private sealed class SetResultInEnumeratorOperation : AsyncOperation<int>
        {
            private readonly int result;

            public SetResultInEnumeratorOperation(int result)
            {
                this.result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                this.Result = this.result;
                yield break;
            }
        }
    }
}