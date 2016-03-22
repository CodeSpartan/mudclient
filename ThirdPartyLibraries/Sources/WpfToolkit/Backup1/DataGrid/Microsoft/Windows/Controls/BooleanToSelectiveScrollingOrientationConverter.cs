﻿//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///     Converts Boolean to SelectiveScrollingOrientation based on the given parameter.
    /// </summary> 
    [Localizability(LocalizationCategory.NeverLocalize)]
    internal sealed class BooleanToSelectiveScrollingOrientationConverter : IValueConverter
    {
        /// <summary>
        ///     Convert Boolean to SelectiveScrollingOrientation
        /// </summary>
        /// <param name="value">Boolean</param>
        /// <param name="targetType">SelectiveScrollingOrientation</param>
        /// <param name="parameter">SelectiveScrollingOrientation that should be used when the Boolean is true</param>
        /// <param name="culture">null</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && parameter is SelectiveScrollingOrientation)
            {
                var valueAsBool = (bool)value;
                var parameterSelectiveScrollingOrientation = (SelectiveScrollingOrientation)parameter;

                if (valueAsBool)
                {
                    return parameterSelectiveScrollingOrientation;
                }
            }

            return SelectiveScrollingOrientation.Both;
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}