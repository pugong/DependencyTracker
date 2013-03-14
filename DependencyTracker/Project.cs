using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace CompanyName.DependencyTracker
{
    /// <summary>
    /// A single project
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Given a file path for a project file, load it in
        /// </summary>
        /// <param name="FolderPath"></param>
        public Project(string ProjectPath)
        {
            FileInfo projectInfo = new FileInfo(ProjectPath);
            if (projectInfo.Extension != ".csproj") throw new ArgumentException("Extension " + projectInfo.Extension + " is not valid");

            _Name = projectInfo.Name.Replace(projectInfo.Extension, ""); //remove extension from file name
            _ProjectPath = ProjectPath;

            //load referenced projects
            string[] ReferenceProjectPaths = GetReferenceProjectPaths();
            foreach (string path in ReferenceProjectPaths)
                _ReferencedProjects.Add(new Project(path));
        }

        public Project()
        {
        }

        /// <summary>
        /// Load project file from disk and get list of project paths
        /// </summary>
        /// <returns></returns>
        private string[] GetReferenceProjectPaths()
        {
            XmlDocument project = new XmlDocument();
            List<string> returnItems = new List<string>();
            try
            {
                project.Load(ProjectPath);
            }
            catch
            { 
                //
                return returnItems.ToArray();
            }

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(project.NameTable);
            namespaceManager.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

            Console.Write(this.Name);
            Console.WriteLine("get reference dll");
            //couldn't get default namespace to work, had to use ns and then prefix all nodes with it
            XmlNodeList referenceNodes = project.SelectNodes("//ns:Project/ns:ItemGroup/ns:Reference/ns:HintPath", namespaceManager); 
            foreach (XmlNode node in referenceNodes)
            {
                //if the reference is to the immediate references directory then it is probably a straight DLL reference
                string refName = node.InnerText;
                // node.InnerText.Replace("..\\", "")
                if (refName.StartsWith("..\\"))
                {
                    string[] refSplit = refName.Split('\\');
                    if (refSplit.Length > 2)
                    {
                        refName = refSplit[refSplit.Length - 2] + refSplit[refSplit.Length - 1];
                    }
                    else
                    {
                        refName = refSplit[refSplit.Length - 1];
                    }
                    referenceList.Add(refName);
                    continue;
                }
            }
            
            Console.Write(this.Name);
            Console.WriteLine("get reference project");
            referenceNodes = project.SelectNodes("//ns:Project/ns:ItemGroup/ns:ProjectReference", namespaceManager); 
            foreach (XmlNode node in referenceNodes)
            {
                Directory.SetCurrentDirectory(new FileInfo(ProjectPath).Directory.FullName);

                
                //reference in project file will be to dll, we need to get project file location
                // FileInfo refDLL = new FileInfo(node.InnerText);
                FileInfo refDLL = new FileInfo(node.Attributes["Include"].Value);

                //move to relative location
                string CurrentDirectory = refDLL.Directory.Parent.FullName;
                if (Directory.Exists(CurrentDirectory))
                    Directory.SetCurrentDirectory(CurrentDirectory);
                else
                    continue; //sometimes hintpaths are bad, in this case just go to next one

                //assume name of file - extension is name of project for now
                string ProjectName = refDLL.Name.Replace(refDLL.Extension, "");
                string ProjectFilePath = ""; //init to nothing initially
                string[] ProjectFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), ProjectName + ".csproj", SearchOption.AllDirectories);
                if(ProjectFilePaths == null || ProjectFilePaths.GetUpperBound(0) < 0)
                {
                    //if we don't have anything, check if the dll is called x.y.z.projectname but the file is just projectname.csproj
                    int dotPos = ProjectName.LastIndexOf('.');
                    if (dotPos != -1)
                    {
                        ProjectName = ProjectName.Substring(dotPos + 1); //start after the last dot
                        ProjectFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), ProjectName + ".csproj", SearchOption.AllDirectories);
                    }
                } 

                //if anything found, use it
                //TODO: Should we raise an event here to let people know we couldn't find a project?
                if((ProjectFilePaths != null && ProjectFilePaths.GetUpperBound(0) >= 0))
                    ProjectFilePath = ProjectFilePaths[0]; //get first match

                //if something found add to list
                if(!String.IsNullOrEmpty(ProjectFilePath))
                    //get first match on project file
                    returnItems.Add(ProjectFilePath);
            }

            return returnItems.ToArray();
        }

        private ProjectList _ReferencedProjects = new ProjectList();

        /// <summary>
        /// All projects that the current project references
        /// </summary>
        public ProjectList ReferencedProjects
        {
            get { return _ReferencedProjects; }
            set { _ReferencedProjects = value; }
        }

        private List<string> referenceList = new List<string>();
        public List<string> ReferenceList
        {
            get { return referenceList; }
            set { referenceList = value; }
        }

        private string _Name = "";

        /// <summary>
        /// Name of the current project
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _ProjectPath = "";

        /// <summary>
        /// Path to the project file
        /// </summary>
        public string ProjectPath
        {
            get { return _ProjectPath; }
            set { _ProjectPath = value; }
        }
    }

}
