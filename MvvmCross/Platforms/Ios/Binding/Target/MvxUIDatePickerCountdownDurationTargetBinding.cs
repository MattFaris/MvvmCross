﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using UIKit;

namespace MvvmCross.Platforms.Ios.Binding.Target
{
    public class MvxUIDatePickerCountDownDurationTargetBinding : MvxBaseUIDatePickerTargetBinding
    {
        public MvxUIDatePickerCountDownDurationTargetBinding(object target, PropertyInfo targetPropertyInfo)
            : base(target, targetPropertyInfo)
        {
        }

        protected override object GetValueFrom(UIDatePicker view)
        {
            return view.CountDownDuration;
        }

        public override Type TargetValueType => typeof(double);
    }
}
