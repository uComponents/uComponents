namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using Umbraco.Core.Dynamics;

    [TestClass]
    public class GridRowCollectionTests
    {
        [TestMethod]
        public void Add_WhenGivenGridRowsWithUniqueIds_ShouldAddGridRow()
        {
            var c = new GridRowCollection();
            var r1 = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var r2 = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            c.Add(r1);
            c.Add(r2);

            Assert.IsTrue(c.Any(x => x.Id == r1.Id));
            Assert.IsTrue(c.Any(x => x.Id == r2.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_WhenGivenGridRowsWithNonUniqueIds_ShouldThrowException()
        {
            var c = new GridRowCollection();
            var r1 = new GridRow(0) { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var r2 = new GridRow(0) { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            c.Add(r1);
            c.Add(r2);
        }

        [TestMethod]
        public void Insert_WhenGivenGridRowsWithUniqueIds_ShouldAddGridRow()
        {
            var c = new GridRowCollection();
            var r1 = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var r2 = new GridRow() { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            c.Insert(0, r1);
            c.Insert(0, r2);

            Assert.IsTrue(c.Any(x => x.Id == r1.Id));
            Assert.IsTrue(c.Any(x => x.Id == r2.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Insert_WhenGivenGridRowsWithNonUniqueIds_ShouldThrowException()
        {
            var c = new GridRowCollection();
            var r1 = new GridRow(0) { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };
            var r2 = new GridRow(0) { new GridCell() { Alias = "test", DataType = -88, Name = "Test", Value = "1234" } };

            c.Insert(0, r1);
            c.Insert(0, r2);
        }

        [TestMethod]
        public void New_WhenGivenValidXml_ShouldCreateGridRowCollection()
        {
            var xml = "<items><item id=\"0\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">Manual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6306?_download=true&amp;_ts=13ab17f5a82</documentUrl></item><item id=\"1\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6305?_download=true&amp;_ts=13ab17f1573</documentUrl></item></items>";

            var c = new GridRowCollection(xml);

            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void New_WhenGivenValidXml2_ShouldCreateGridRowCollection()
        {
            var xml = "<items><item id=\"1\" sortOrder=\"1\"><featureHeading nodeName=\"Feature Heading\" nodeType=\"-88\">Build your no claims discount fast with our Student Bonus Accelerator</featureHeading><featureDescription nodeName=\"Feature Description\" nodeType=\"1038\">If you don’t make a claim, you could build 2 years no claims discount in just 18 months. Even if you make a windscreen claim or have a claim where all costs are recovered, it won’t affect this offer.This exclusive offer is only available to students. If you’d like information on what happens if your occupation changes or you transfer your policy please see our terms and conditions.</featureDescription><featureAccordion nodeName=\"Feature Accordion (show/hide text)\" nodeType=\"1034\" /></item><item id=\"3\" sortOrder=\"2\"><featureHeading nodeName=\"Feature Heading\" nodeType=\"-88\">We’ll get you back on the road quicker</featureHeading><featureDescription nodeName=\"Feature Description\" nodeType=\"1038\">We settle quickly. One call to our team will help you with everything for your claim. Our mobile repair service, where possible, comes to you to save you time.</featureDescription><featureAccordion nodeName=\"Feature Accordion (show/hide text)\" nodeType=\"1034\" /></item><item id=\"2\" sortOrder=\"3\"><featureHeading nodeName=\"Feature Heading\" nodeType=\"-88\">24 hour windscreen and glass cover with our comprehensive policies</featureHeading><featureDescription nodeName=\"Feature Description\" nodeType=\"1038\">When you take out a comprehensive policy, you can rest assured that your windscreen and other glass is protected.This is a new para.</featureDescription><featureAccordion nodeName=\"Feature Accordion (show/hide text)\" nodeType=\"1034\" /></item></items>";

            var c = new GridRowCollection(xml);

            Assert.IsNotNull(c);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void New_WhenGivenInvalidXml_ShouldCreateGridRowCollection()
        {
            var xml = "<items><item id=\"0\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">Manual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6306?_download=true&amp;_ts=13ab17f5a82</documentUrl></item><item id=\"0\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6305?_download=true&amp;_ts=13ab17f1573</documentUrl></item></items>";

            var c = new GridRowCollection(xml);
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenValidGridRowCollection_ShouldReturnDynamicXml()
        {
            var c = new GridRowCollection();
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void AsDynamicXml_WhenGivenValidXml_ShouldReturnDynamicXml()
        {
            var c = new GridRowCollection("<items><item id=\"2\"><member nodeName=\"Member\" nodeType=\"1036\">1312</member></item><item id=\"3\"><member nodeName=\"Member\" nodeType=\"1036\">1189</member></item><item id=\"4\"><member nodeName=\"Member\" nodeType=\"1036\">1370</member></item></items>");
            var x = c.AsDynamicXml();

            Assert.IsInstanceOfType(x, typeof(DynamicXml));
        }

        [TestMethod]
        public void ToString_WhenGivenValidXml_ShouldReturnSameXml()
        {
            var xml1 = "<items><item id=\"0\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">Manual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6306?_download=true&amp;_ts=13ab17f5a82</documentUrl></item><item id=\"1\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6305?_download=true&amp;_ts=13ab17f1573</documentUrl></item></items>";
            var xml2 = "<items><item id=\"0\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">Installasjonsmanual</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6296?_download=true&amp;_ts=13ab176bd6a</documentUrl></item><item id=\"1\" sortOrder=\"0\"><documentName nodeName=\"\" nodeType=\"0\">FDV dokumentasjon</documentName><documentUrl nodeName=\"\" nodeType=\"0\">http://jotul.com/no/produkter.xml/_attachment/6295?_download=true&amp;_ts=13ab176537b</documentUrl></item></items>";

            var c1 = new GridRowCollection(xml1);
            var c2 = new GridRowCollection(xml2);

            var s1 = c1.ToString();
            var s2 = c2.ToString();

            Assert.AreEqual(s1, xml1);
            Assert.AreEqual(s2, xml2);
        }
    }
}