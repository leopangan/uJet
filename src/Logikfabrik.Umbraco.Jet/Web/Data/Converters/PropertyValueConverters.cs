﻿// <copyright file="PropertyValueConverters.cs" company="Logikfabrik">
//   Copyright (c) 2016 anton(at)logikfabrik.se. Licensed under the MIT license.
// </copyright>

namespace Logikfabrik.Umbraco.Jet.Web.Data.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The <see cref="PropertyValueConverters" /> class.
    /// </summary>
    public static class PropertyValueConverters
    {
        /// <summary>
        /// Gets the converters.
        /// </summary>
        /// <value>The converters.</value>
        public static PropertyValueConverterDictionary Converters { get; } = GetDefaultConverters();

        /// <summary>
        /// Gets a converter that can convert from and to the specified types.
        /// </summary>
        /// <param name="uiHint">The UI hint.</param>
        /// <param name="from">The type to convert from.</param>
        /// <param name="to">The type to convert to.</param>
        /// <returns>A converter that can convert from and to the specified types.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="from" />, or <paramref name="to" /> are <c>null</c>.</exception>
        internal static IPropertyValueConverter GetConverter(string uiHint, Type from, Type to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            IEnumerable<IPropertyValueConverter> converters;

            return !Converters.TryGetValue(to, out converters)
                ? null
                : converters.FirstOrDefault(c => c.CanConvertValue(uiHint, from, to));
        }

        /// <summary>
        /// Gets the default converters.
        /// </summary>
        /// <returns>The default converters.</returns>
        private static PropertyValueConverterDictionary GetDefaultConverters()
        {
            return new PropertyValueConverterDictionary
            {
                { typeof(string), new[] { new HtmlStringPropertyValueConverter() } },
                { typeof(decimal), new[] { new FloatingDecimalPointPropertyValueConverter() } },
                { typeof(decimal?), new[] { new FloatingDecimalPointPropertyValueConverter() } },
                { typeof(float), new[] { new FloatingBinaryPointPropertyValueConverter() } },
                { typeof(float?), new[] { new FloatingBinaryPointPropertyValueConverter() } },
                { typeof(double), new[] { new FloatingBinaryPointPropertyValueConverter() } },
                { typeof(double?), new[] { new FloatingBinaryPointPropertyValueConverter() } }
            };
        }
    }
}