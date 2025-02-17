﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Source;
using MvvmCross.Binding.Bindings.Source.Construction;
using MvvmCross.Binding.Parse.PropertyPath.PropertyTokens;
using MvvmCross.Converters;

namespace MvvmCross.Plugin.FieldBinding
{
    [Preserve(AllMembers = true)]
    public class MvxChainedNotifyChangeFieldSourceBinding
        : MvxNotifyChangeFieldSourceBinding
    {
        public static bool DisableWarnIndexedValueBindingWarning = false;

        private readonly List<MvxPropertyToken> _childTokens;
        private IMvxSourceBinding _currentChildBinding;

        public MvxChainedNotifyChangeFieldSourceBinding(object source, INotifyChange notifyChange,
                                                        List<MvxPropertyToken> childTokens)
            : base(source, notifyChange)
        {
            _childTokens = childTokens;
            if (!DisableWarnIndexedValueBindingWarning)
                WarnIfChildTokensSuspiciousOfIndexedValueBinding();
            UpdateChildBinding();
        }

        private void WarnIfChildTokensSuspiciousOfIndexedValueBinding()
        {
            if (_childTokens == null || _childTokens.Count < 2)
                return;

            var firstAsName = _childTokens[0] as MvxPropertyNamePropertyToken;
            if (firstAsName == null || firstAsName.PropertyName != "Value")
                return;

            var secondAsIndexed = _childTokens[1] as MvxIndexerPropertyToken;
            if (secondAsIndexed == null)
                return;

            MvxPluginLog.Instance?.Log(LogLevel.Warning,
                "Suspicious indexed binding seen to Value[] within INC binding - this may be OK, but is often a result of FluentBinding used on INC<T> - consider using INCList<TValue> or INCDictionary<TKey,TValue> instead - see https://github.com/slodge/MvvmCross/issues/353. This message can be disabled using DisableWarnIndexedValueBindingWarning");
        }

        protected override void NotifyChangeOnChanged(object sender, EventArgs eventArgs)
        {
            UpdateChildBinding();
            FireChanged();
        }

        private IMvxSourceBindingFactory SourceBindingFactory => MvxBindingSingletonCache.Instance.SourceBindingFactory;

        public override Type SourceType
        {
            get
            {
                if (_currentChildBinding == null)
                    return typeof(object);

                return _currentChildBinding.SourceType;
            }
        }

        protected void UpdateChildBinding()
        {
            if (_currentChildBinding != null)
            {
                _currentChildBinding.Changed -= ChildSourceBindingChanged;
                _currentChildBinding.Dispose();
                _currentChildBinding = null;
            }

            if (NotifyChange == null)
            {
                return;
            }

            var currentValue = NotifyChange.Value;
            if (currentValue == null)
            {
                // value will be missing... so end consumer will need to use fallback values
                return;
            }
            else
            {
                _currentChildBinding = SourceBindingFactory.CreateBinding(currentValue, _childTokens);
                _currentChildBinding.Changed += ChildSourceBindingChanged;
            }
        }

        private void ChildSourceBindingChanged(object sender, EventArgs e)
        {
            FireChanged();
        }

        public override object GetValue()
        {
            if (_currentChildBinding == null)
            {
                return MvxBindingConstant.UnsetValue;
            }

            return _currentChildBinding.GetValue();
        }

        public override void SetValue(object value)
        {
            if (_currentChildBinding == null)
            {
                MvxPluginLog.Instance?.Log(LogLevel.Warning,
                    "SetValue ignored in binding - target property path missing");
                return;
            }

            _currentChildBinding.SetValue(value);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_currentChildBinding != null)
                {
                    _currentChildBinding.Dispose();
                    _currentChildBinding = null;
                }
            }

            base.Dispose(isDisposing);
        }
    }
}
