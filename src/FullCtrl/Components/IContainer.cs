﻿using System;

namespace FullCtrl
{
    public interface IContainer
    {
        void Bind(Type objectType, Type targetType);
        bool IsBound(Type objectType);
        object Resolve(Type objectType);
        Type ResolveType(Type objectType);
    }
}