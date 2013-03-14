using CompanyName.DependencyTracker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace DependencyTrackerTest
{
    
    
    /// <summary>
    ///This is a test class for ProjectListTest and is intended
    ///to contain all ProjectListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProjectListTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ToReferenceXML
        ///</summary>
        [TestMethod()]
        public void ToReferenceXMLTest()
        {
            ProjectList target = new ProjectList(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToReferenceXML();
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GenerateReferXML
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DependencyTracker.exe")]
        public void GenerateReferXMLTest()
        {
            ProjectList_Accessor target = new ProjectList_Accessor(); // TODO: Initialize to an appropriate value
            string refer = "refer";
            StringBuilder sb = new StringBuilder();
            target.GenerateReferXML(refer, sb);
            string expect =
@"    <row name=""refer"">
        <comment>refer</comment>
        <relation table=""refer"" row=""refer"" />
    </row>
";
            Assert.AreEqual(expect, sb.ToString());
            //Assert.Fail("A method that does not return a value cannot be verified.");
        }

        
    }
}
