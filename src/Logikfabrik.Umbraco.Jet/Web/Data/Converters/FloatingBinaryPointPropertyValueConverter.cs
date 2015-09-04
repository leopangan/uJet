﻿//----------------------------------------------------------------------------------
// <copyright file="FloatingBinaryPointPropertyValueConverter.cs" company="Logikfabrik">
//     The MIT License (MIT)
//
//     Copyright (c) 2015 anton(at)logikfabrik.se
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
//----------------------------------------------------------------------------------

namespace Logikfabrik.Umbraco.Jet.Web.Data.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The <see cref="FloatingBinaryPointPropertyValueConverter" /> class.
    /// </summary>
    public class FloatingBinaryPointPropertyValueConverter : IPropertyValueConverter
    {
        /// <summary>
        /// Determines whether this instance can convert between types.
        /// </summary>
        /// <param name="uiHint">The UI hint.</param>
        /// <param name="from">From type.</param>
        /// <param name="to">To type.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert between types; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if from or to are null.</exception>
        public bool CanConvertValue(string uiHint, Type from, Type to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            var validTypes = new[] { typeof(float), typeof(float?), typeof(double), typeof(double?) };

            return from == typeof(string) && validTypes.Contains(to);
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="to">To type.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public object Convert(object value, Type to)
        {
            if (value == null)
            {
                return null;
            }

            if (to == typeof(float) || to == typeof(float?))
            {
                return ConvertToFloat(value);
            }

            if (to == typeof(double) || to == typeof(double?))
            {
                return ConvertToDouble(value);
            }

            return null;
        }

        /// <summary>
        /// Converts to float.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertToFloat(object value)
        {
            if (value == null)
            {
                return null;
            }

            float result;

            if (float.TryParse(
                    value.ToString(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture.NumberFormat,
                    out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertToDouble(object value)
        {
            if (value == null)
            {
                return null;
            }

            double result;

            if (double.TryParse(
                    value.ToString(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture.NumberFormat,
                    out result))
            {
                return result;
            }

            return null;
        }
    }
}
