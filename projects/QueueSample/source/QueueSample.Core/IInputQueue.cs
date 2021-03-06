﻿//-----------------------------------------------------------------------
// <copyright file="IInputQueue.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace QueueSample
{
    public interface IInputQueue<T> : IProducerQueue<T>, IConsumerQueue<T>
    {
    }
}
