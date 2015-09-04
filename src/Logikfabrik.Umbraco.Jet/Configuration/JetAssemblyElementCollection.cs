﻿//----------------------------------------------------------------------------------
// <copyright file="JetAssemblyElementCollection.cs" company="Logikfabrik">
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

namespace Logikfabrik.Umbraco.Jet.Configuration
{
    using System.Configuration;

    /// <summary>
    /// The <see cref="JetAssemblyElementCollection" /> class.
    /// </summary>
    public class JetAssemblyElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="JetAssemblyElement" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="JetAssemblyElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new JetAssemblyElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="JetAssemblyElement" /> to return the key for.</param>
        /// <returns>
        /// An <see cref="string" /> that acts as the key for the specified <see cref="JetAssemblyElement" />.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JetAssemblyElement)element).Name;
        }
    }
}
