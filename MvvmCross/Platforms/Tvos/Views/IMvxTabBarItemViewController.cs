﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

namespace MvvmCross.Platforms.Tvos.Views
{
    public interface IMvxTabBarItemViewController
    {
        string TabName { get; }
        string TabIconName { get; }

        string TabSelectedIconName { get; }
    }
}
