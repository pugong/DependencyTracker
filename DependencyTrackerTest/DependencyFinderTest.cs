using CompanyName.DependencyTracker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DependencyTrackerTest
{
    
    
    /// <summary>
    ///This is a test class for DependencyFinderTest and is intended
    ///to contain all DependencyFinderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyFinderTest
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
        ///A test for GetDistinctProjects
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DependencyTracker.exe")]
        public void GetDistinctProjectsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DependencyFinder_Accessor target = new DependencyFinder_Accessor(param0); // TODO: Initialize to an appropriate value
            ProjectList projects = new ProjectList(); // TODO: Initialize to an appropriate value
            ProjectList projectList = new ProjectList(); // TODO: Initialize to an appropriate value
            ProjectList expectList = new ProjectList();
            //target.GetDistinctProjects(projects, projectList);
            //Assert.AreEqual(expectList, projectList);
            // Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
