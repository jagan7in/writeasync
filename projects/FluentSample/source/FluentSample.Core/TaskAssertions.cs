﻿//-----------------------------------------------------------------------
// <copyright file="TaskAssertions.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace FluentSample
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions.Execution;

    public class TaskAssertions
    {
        private readonly Task subject;

        public TaskAssertions(Task subject)
        {
            this.subject = subject;
        }

        public void BeCompleted()
        {
            Execute.Assertion
                .ForCondition(this.subject.IsCompleted)
                .FailWith("Expected task to be completed but was {0}.", this.subject.Status);
        }
    }
}
