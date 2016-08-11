﻿using System;

namespace Remotus.Base.Observables
{
    public interface IBroadcaster<T> : IBroadcaster<T, T>
    { }

    public interface IBroadcaster<in TSource, out TResult> : IObserver<TSource>, IObservable<TResult>
    { }
}
