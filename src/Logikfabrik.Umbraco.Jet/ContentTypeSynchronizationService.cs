﻿// The MIT License (MIT)

// Copyright (c) 2015 anton(at)logikfabrik.se

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Logikfabrik.Umbraco.Jet.Extensions;
using Logikfabrik.Umbraco.Jet.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Logikfabrik.Umbraco.Jet
{
    public abstract class ContentTypeSynchronizationService : ISynchronizationService
    {
        private readonly IContentTypeService _contentTypeService;

        protected ContentTypeSynchronizationService(IContentTypeService contentTypeService)
        {
            if (contentTypeService == null)
                throw new ArgumentNullException("contentTypeService");

            _contentTypeService = contentTypeService;
        }

        public abstract void Synchronize();

        /// <summary>
        /// Creates a content type.
        /// </summary>
        /// <typeparam name="T">The reflected content type type.</typeparam>
        /// <param name="contentTypeBaseConstructor">The content type constructor.</param>
        /// <param name="contentType">The reflected content type to create.</param>
        /// <returns>A content type.</returns>
        protected static IContentTypeBase CreateContentType<T>(Func<IContentTypeBase> contentTypeBaseConstructor,
            ContentType<T> contentType) where T : ContentTypeAttribute
        {
            if (contentTypeBaseConstructor == null)
                throw new ArgumentNullException("contentTypeBaseConstructor");

            if (contentType == null)
                throw new ArgumentNullException("contentType");

            var t = contentTypeBaseConstructor();

            t.Name = contentType.Name;
            t.Alias = contentType.Alias;
            t.Description = contentType.Description;
            t.AllowedAsRoot = contentType.AllowedAsRoot;

            if (contentType.Icon != null)
                t.Icon = contentType.Icon;

            if (contentType.Thumbnail != null)
                t.Thumbnail = contentType.Thumbnail;

            CreatePropertyTypes(t, contentType.Properties);

            return t;
        }

        /// <summary>
        /// Updates a content type.
        /// </summary>
        /// <typeparam name="T">The reflected content type type.</typeparam>
        /// <param name="contentTypeBase">The content type to update.</param>
        /// <param name="contentTypeBaseConstructor">The content type constructor.</param>
        /// <param name="contentType">The reflected content type to update.</param>
        protected static void UpdateContentType<T>(IContentTypeBase contentTypeBase,
            Func<IContentTypeBase> contentTypeBaseConstructor, ContentType<T> contentType)
            where T : ContentTypeAttribute
        {
            if (contentTypeBase == null)
                throw new ArgumentNullException("contentTypeBase");

            if (contentTypeBaseConstructor == null)
                throw new ArgumentNullException("contentTypeBaseConstructor");

            if (contentType == null)
                throw new ArgumentNullException("contentType");

            contentTypeBase.Name = contentType.Name;
            contentTypeBase.Description = contentType.Description;
            contentTypeBase.AllowedAsRoot = contentType.AllowedAsRoot;

            var defaultContentType = contentTypeBaseConstructor();

            contentTypeBase.Icon = contentType.Icon ?? defaultContentType.Icon;
            contentTypeBase.Thumbnail = contentType.Thumbnail ?? defaultContentType.Thumbnail;

            UpdatePropertyTypes(contentTypeBase, contentType.Properties);
        }

        /// <summary>
        /// Creates property types.
        /// </summary>
        /// <param name="contentTypeBase">The content type to add the properties to.</param>
        /// <param name="contentTypeProperties">The reflected content type properties to create.</param>
        protected static void CreatePropertyTypes(IContentTypeBase contentTypeBase, IEnumerable<ContentTypeProperty> contentTypeProperties)
        {
            if (contentTypeBase == null)
                throw new ArgumentNullException("contentTypeBase");

            if (contentTypeProperties == null)
                throw new ArgumentNullException("contentTypeProperties");

            var p = contentTypeProperties.ToArray();

            if (!p.Any())
                return;

            foreach (var property in p)
                CreatePropertyType(contentTypeBase, property);
        }

        /// <summary>
        /// Creates a property type.
        /// </summary>
        /// <param name="contentTypeBase">The content type to add the property to.</param>
        /// <param name="contentTypeProperty">The reflected content type property to create.</param>
        private static void CreatePropertyType(IContentTypeBase contentTypeBase, ContentTypeProperty contentTypeProperty)
        {
            if (contentTypeBase == null)
                throw new ArgumentNullException("contentTypeBase");

            if (contentTypeProperty == null)
                throw new ArgumentNullException("contentTypeProperty");

            var definition = GetDataDefinition(contentTypeProperty);

            var propertyType = new PropertyType(definition)
            {
                Name = contentTypeProperty.Name,
                Alias = contentTypeProperty.Alias,
                Mandatory = contentTypeProperty.Mandatory,
                Description = contentTypeProperty.Description,
                ValidationRegExp = contentTypeProperty.RegularExpression
            };

            if (contentTypeProperty.SortOrder.HasValue)
                propertyType.SortOrder = contentTypeProperty.SortOrder.Value;

            if (!string.IsNullOrWhiteSpace(contentTypeProperty.PropertyGroup))
                contentTypeBase.AddPropertyType(propertyType, contentTypeProperty.PropertyGroup);
            else
                contentTypeBase.AddPropertyType(propertyType);
        }

        /// <summary>
        /// Updates property types.
        /// </summary>
        /// <param name="contentTypeBase">The content type to update.</param>
        /// <param name="contentTypeProperties">The reflected content type properties.</param>
        private static void UpdatePropertyTypes(IContentTypeBase contentTypeBase, IEnumerable<ContentTypeProperty> contentTypeProperties)
        {
            if (contentTypeBase == null)
                throw new ArgumentNullException("contentTypeBase");

            if (contentTypeProperties == null)
                throw new ArgumentNullException("contentTypeProperties");

            var p = contentTypeProperties.ToArray();

            if (!p.Any())
                return;

            foreach (var property in p)
                if (contentTypeBase.PropertyTypes.All(ct => ct.Alias != property.Alias))
                    CreatePropertyType(contentTypeBase, property);
                else
                    UpdatePropertyType(contentTypeBase, contentTypeBase.PropertyTypes.First(ct => ct.Alias == property.Alias), property);
        }

        /// <summary>
        /// Updates a property type.
        /// </summary>
        /// <param name="contentTypeBase">The content type to update.</param>
        /// <param name="propertyType">The property type to update.</param>
        /// <param name="contentTypeProperty">The reflected content type property.</param>
        private static void UpdatePropertyType(IContentTypeBase contentTypeBase, PropertyType propertyType, ContentTypeProperty contentTypeProperty)
        {
            if (contentTypeBase == null)
                throw new ArgumentNullException("contentTypeBase");

            if (propertyType == null)
                throw new ArgumentNullException("propertyType");

            if (contentTypeProperty == null)
                throw new ArgumentNullException("contentTypeProperty");

            if (!contentTypeBase.PropertyGroups.Contains(contentTypeProperty.PropertyGroup) ||
                (contentTypeBase.PropertyGroups.Contains(contentTypeProperty.PropertyGroup) &&
                 !contentTypeBase.PropertyGroups[contentTypeProperty.PropertyGroup].PropertyTypes.Contains(
                     contentTypeProperty.Alias)))
                contentTypeBase.MovePropertyType(contentTypeProperty.Alias, contentTypeProperty.PropertyGroup);

            propertyType.Name = contentTypeProperty.Name;
            propertyType.Alias = contentTypeProperty.Alias;
            propertyType.Mandatory = contentTypeProperty.Mandatory;
            propertyType.Description = contentTypeProperty.Description;
            propertyType.ValidationRegExp = contentTypeProperty.RegularExpression;

            if (contentTypeProperty.SortOrder.HasValue)
                propertyType.SortOrder = contentTypeProperty.SortOrder.Value;

            var definition = GetDataDefinition(contentTypeProperty);

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (propertyType.DataTypeDefinitionId != definition.Id)
                propertyType.DataTypeDefinitionId = definition.Id;
        }

        /// <summary>
        /// Gets a data type definition.
        /// </summary>
        /// <param name="contentTypeProperty">The reflected content type property to create.</param>
        /// <returns>A data type definition.</returns>
        private static IDataTypeDefinition GetDataDefinition(ContentTypeProperty contentTypeProperty)
        {
            if (contentTypeProperty == null)
                throw new ArgumentNullException("contentTypeProperty");

            var definition = DataTypeDefinitionMappings.GetDefinition(contentTypeProperty.UIHint,
                                                                      contentTypeProperty.Type);

            if (definition == null)
                throw new Exception(
                    string.Format("There is no definition for content type property of type {0}.",
                                  contentTypeProperty.Type));

            return definition;
        }

        /// <summary>
        /// Sets allowed content types.
        /// </summary>
        /// <typeparam name="T">The reflected content type type.</typeparam>
        /// <param name="contentTypeBases">Content types.</param>
        /// <param name="contentTypes">Reflected content types.</param>
        protected void SetAllowedContentTypes<T>(IContentTypeBase[] contentTypeBases,
            IEnumerable<ContentType<T>> contentTypes) where T : ContentTypeAttribute
        {
            if (contentTypeBases == null)
                throw new ArgumentNullException("contentTypeBases");

            if (contentTypes == null)
                throw new ArgumentNullException("contentTypes");

            foreach (
                var contentType in
                    contentTypes.Where(dt => dt.AllowedChildNodeTypes.Any())
                )
            {
                var contentTypeBase = contentTypeBases.FirstOrDefault(ct => ct.Alias == contentType.Alias);

                if (contentTypeBase == null)
                    continue;

                contentTypeBase.AllowedContentTypes = GetAllowedChildNodeContentTypes(contentType.AllowedChildNodeTypes);

                if (contentType.Type.IsDocumentType())
                    _contentTypeService.Save((IContentType)contentTypeBase);

                else if (contentType.Type.IsMediaType())
                    _contentTypeService.Save((IMediaType)contentTypeBase);
            }
        }

        private IEnumerable<ContentTypeSort> GetAllowedChildNodeContentTypes(IEnumerable<Type> allowedChildNodeTypes)
        {
            if (allowedChildNodeTypes == null)
                throw new ArgumentNullException("allowedChildNodeTypes");

            var nodeTypes = new List<ContentTypeSort>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var allowedChildNodeType in allowedChildNodeTypes)
            {
                var contentType = _contentTypeService.GetContentType(allowedChildNodeType.Name.Alias());

                if (contentType == null)
                    continue;

                nodeTypes.Add(new ContentTypeSort(new Lazy<int>(() => contentType.Id), nodeTypes.Count,
                    contentType.Alias));
            }

            return nodeTypes;
        }
    }
}