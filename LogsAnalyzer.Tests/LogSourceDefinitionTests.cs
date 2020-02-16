using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using LogAnalyzer.Analyzers.Bookings;
using LogAnalyzer.Analyzers.Bookings.Models;
using LogAnalyzer.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogAnalyzer.Tests {
    [TestClass]
    public class LogSourceDefinitionTests {
        //[TestMethod]
        public void itShouldSerialize() {
            var logSourceDef = new LogSourceDefinition();
            logSourceDef.SourceFiles.Add(@"c:\file1.log");
            logSourceDef.SourceFiles.Add(@"c:\file2.log");

            logSourceDef.SourceFolders.Add(@"c:\folder1");
            logSourceDef.SourceFolders.Add(@"c:\folder2");

            // Insert code to set properties and fields of the object.  
            XmlSerializer mySerializer = new
            XmlSerializer(typeof(LogSourceDefinition));
            // To write to a file, create a StreamWriter object.  
            StreamWriter myWriter = new StreamWriter("myFileName.xml");
            mySerializer.Serialize(myWriter, logSourceDef);
            myWriter.Close();
        }

        //[TestMethod]
        public void itShouldDeserialize() {
            var mySerializer = new XmlSerializer(typeof(LogSourceDefinition));
            // To read the file, create a FileStream.
            var myFileStream = new FileStream("myFileName.xml", FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            var myObject = (LogSourceDefinition)mySerializer.Deserialize(myFileStream);
        }

        [TestMethod]
        public void TestGenericReflection() {
            var x = new BookingAnalyzer<BookingAnalysis>();

            Debug.Print(x.ToString());

            //LogAnalyzer.Analyzers.Bookings.BookingAnalyzer`1[LogAnalyzer.Analyzers.Bookings.Models.BookingAnalysis]
        }
    }
}
