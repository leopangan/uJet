﻿// <copyright file="JetControllerActionInvokerTest.cs" company="Logikfabrik">
//   Copyright (c) 2016 anton(at)logikfabrik.se. Licensed under the MIT license.
// </copyright>

namespace Logikfabrik.Umbraco.Jet.Test.Web.Mvc
{
    using Jet.Extensions;
    using Jet.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JetControllerActionInvokerTest
    {
        [TestMethod]
        public void CanGetActionNameForPreviewAction()
        {
            Assert.AreEqual("Index", JetControllerActionInvoker.GetActionName(PreviewTemplateAttribute.TemplateName.Alias()));
        }

        [TestMethod]
        public void CanGetActionNameForIndexAction()
        {
            Assert.AreEqual("Index", JetControllerActionInvoker.GetActionName("Index"));
        }
    }
}
