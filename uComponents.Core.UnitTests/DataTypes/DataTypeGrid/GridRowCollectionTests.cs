namespace uComponents.Core.UnitTests.DataTypes.DataTypeGrid
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using uComponents.DataTypes.DataTypeGrid.Model;

    using umbraco.MacroEngines;

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