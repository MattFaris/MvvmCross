﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

namespace MvvmCross.Binding.BindingContext
{
    public class MvxInlineBindingTarget<TViewModel>
    {
        public MvxInlineBindingTarget(IMvxBindingContextOwner bindingContextOwner)
        {
            BindingContextOwner = bindingContextOwner;
        }

        public IMvxBindingContextOwner BindingContextOwner { get; private set; }
    }
}
