using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;


namespace CompanyName.DependencyTracker
{
    public class DependencyFinder
    {
        string SearchPath = "";
        string MainLineOnly = "";

        /// <summary>
        /// Init dependency finder with a path to search for project files
        /// </summary>
        /// <param name="SearchPath"></param>
        public DependencyFinder(string searchPath, string mainLineOnly = "T")
        {
            this.SearchPath = searchPath;
            this.MainLineOnly = mainLineOnly;
        }

        /// <summary>
        /// In the given path and all folders below it, load all valid projects, currently only .csproj files
        /// </summary>
        /// <param name="SearchPath"></param>
        /// <returns></returns>
        public ProjectList GetAllProjects()
        {
            //return list of projects
            ProjectList projects = new ProjectList();
            Console.WriteLine("start to fetch .csproj files");
            //get list of all csproj files below current directory
            string[] FilePaths = Directory.GetFiles(SearchPath, "*.csproj", SearchOption.AllDirectories);
            foreach (string FilePath in FilePaths)
            {
                if (!FilePath.ToLower().Contains("mainline") && MainLineOnly.ToUpper().Equals("T"))
                    continue;
                else
                {
                    string FileName = new FileInfo(FilePath).Name;

                    //if not a unit test (Test or UnitTest.csproj, then add it
                    //TODO: refactor to allow exclusions to be passed in?
                    if (!FileName.StartsWith("Test") && !FileName.StartsWith("UnitTest"))
                    {
                        Console.WriteLine(string.Format("Adding project {0} ...", FilePath));
                        projects.Add(new Project(FilePath));
                    }
                }
            }
            return projects;
        }

        /// <summary>
        /// Given a filepath for a JPG, get all the projects and their dependencies and output a JPG file to that path.
        /// Download graphviz from http://www.graphviz.org/pub/graphviz/ARCHIVE/graphviz-2.14.1.exe
        /// </summary>
        /// <param name="OutputJPGFilePath"></param>
        public void VisualizeProjects(string OutputPath)
        {
            ProjectList projects = GetAllProjects();
            // VisualOutput(projects, OutputPath);
            ConsoleOutput(projects, OutputPath);
            
           
        }


        private void ConsoleOutput(ProjectList projects, string OutputPath)
        {
            if (projects.Count == 0)
            {
                Console.WriteLine("No Project was found, please remove mainonlyflag and try again");
                return;
            }
            //loop through every project and output its dependencies to the file in the format
            //parent -> child  && Child back to parent
            
            
            string[] path = SearchPath.Split('\\');
            // string rootName =  + ;
            string rootName = string.Format("{0}_{1}", path.Length > 0 ? path[path.Length - 1] : projects[0].Name , DateTime.Now.ToString("yyyymmddhhMMss"));
            StringBuilder sbRefer = new StringBuilder();
            FileStream fs;
            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            string target = string.Format("{0}\\{1}_relation.xml", OutputPath, rootName);
            if (File.Exists(target))
            {
                File.Delete(target);
            }

            ProjectList distinctProjects = new ProjectList();
            GetDistinctProjects(projects, distinctProjects);

            System.IO.File.WriteAllText(target, distinctProjects.ToReferenceXML());

            OutputPath = string.Format("{0}\\{1}_details", OutputPath, rootName);
            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            foreach (Project p in distinctProjects)
            {
                target = string.Format("{0}\\{1}.txt", OutputPath, p.Name);
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                fs = System.IO.File.OpenWrite(target);
                using (StreamWriter outfile = new StreamWriter(fs))
                {
                    FetchProjectRefence(p, "Root", outfile, true);
                }
            }

            OutputPath = string.Format("{0}\\..\\{1}_ReferedBy", OutputPath, rootName);
            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);
            ProcessedLinks.Clear();
            foreach (Project p in distinctProjects)
            {
                target = string.Format("{0}\\{1}.txt", OutputPath, p.Name);
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                fs = System.IO.File.OpenWrite(target);
                using (StreamWriter outfile = new StreamWriter(fs))
                {
                    FetchProjectReferedBy(p, distinctProjects, p.Name, outfile, p.Name);
                }

            }
            // Write to DB
            string rowName = PushDataIntoDB(distinctProjects, rootName);
            System.Diagnostics.Process.Start(string.Format("http://citsm.sh.ctriptravel.com/CITSM.Data/index.aspx?listid=8&DesignName=&DesignDBName={0}", rowName));
        }

        private string  PushDataIntoDB(ProjectList projects, string name)
        {
            
            string connectionString = "Integrated Security=SSPI;Initial Catalog=ShareDB;Data Source=devdb.dev.sh.ctriptravel.com,28747";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"INSERT INTO [CDM_DBXML]([DBName],[XMLData],[ModifyDate]) VALUES (@dbname,@xmlData,getdate()); select @tid = @@Identity";
                cmd.Parameters.AddWithValue("@dbname", name);
                cmd.Parameters.AddWithValue("@xmlData", projects.ToReferenceXML());
                cmd.Parameters.Add("@tid", System.Data.SqlDbType.Int);
                cmd.Parameters["@tid"].Direction = System.Data.ParameterDirection.Output;
                //cmd.CommandText = "INSERT INTO [CDM_DBXML]([DBName],[XMLData],[ModifyDate]) VALUES ('" + DateTime.Now.ToString("yyyymmddhhMMss") + "','" + projects.ToReferenceXML() + "',getdate())";

                conn.Open();
                cmd.ExecuteNonQuery();
                int rValue = Convert.ToInt32(cmd.Parameters["@tid"].Value);
                Console.WriteLine(rValue);
                return name;
            }
        }

        private void FetchRefence(StringBuilder sbRefer, ProjectList projects, string prefix, StreamWriter outfile, bool isRoot = true)
        {
            foreach (Project project in projects)
            {
                if (isRoot)
                {
                    outfile.WriteLine();
                }
                FetchProjectRefence( project, prefix, outfile, false);
                #region change to function
                //string tempfix = prefix + " - " + project.Name;
                //Console.WriteLine(tempfix + ": " + project.ProjectPath);
                //// sbRefer.AppendLine(project.Name + ":" + project.ProjectPath);
                //outfile.WriteLine(tempfix + ": " + project.ProjectPath);
                //foreach (string refer in project.ReferenceList)
                //{
                //    Console.WriteLine(tempfix + " Assembly Refer: " + refer);
                //    // sbRefer.AppendLine(prefix +" Assembly Refer:" + refer);
                //    outfile.WriteLine(tempfix + " Assembly Refer: " + refer);
                //}
                //foreach (Project reference in project.ReferencedProjects)
                //{
                //    //if not processed output link and add to processed list
                //    if (!ProcessedLinks.Contains(project.Name + "-" + reference.Name))
                //    {
                //        // sb.Append("\"" + project.Name + "\" -> \"" + reference.Name + "\"" + Environment.NewLine);
                //        ProcessedLinks.Add(project.Name + "-" + reference.Name);
                //        foreach (string refer in reference.ReferenceList)
                //        {
                //            Console.WriteLine(tempfix + " Assembly Refer: " + refer);
                //            // sbRefer.AppendLine(prefix +" Assembly Refer:" + refer);
                //            outfile.WriteLine(tempfix + " Assembly Refer: " + refer);
                //        }
                //        Console.WriteLine(tempfix + " Project Refer: " + reference.Name);
                //        // sbRefer.AppendLine(prefix + " Project Refer:" + reference.Name);
                //        outfile.WriteLine(tempfix + " Project Refer: " + reference.Name);
                //        FetchRefence(sbRefer, reference.ReferencedProjects, tempfix, outfile, false);
                        
                //    }

                //}
                #endregion

            }

        }

        private void FetchProjectRefence( Project project, string prefix, StreamWriter outfile, bool isRoot = true)
        {
            string tempfix = string.Format("{0} - {1}", prefix, project.Name);
            string rString = string.Format("{0}: {1}", tempfix, project.ProjectPath);
            Console.WriteLine(rString);
            outfile.WriteLine(rString);
            
            foreach (string refer in project.ReferenceList)
            {
                rString = string.Format("{0} Assembly Refer:  {1}", tempfix, refer);
                Console.WriteLine(rString);
                outfile.WriteLine(rString);
            }
            foreach (Project reference in project.ReferencedProjects)
            {
                rString = string.Format("{0} Project Refer:  {1}", tempfix, reference.Name);
                Console.WriteLine(rString);
                outfile.WriteLine(rString);
                FetchProjectRefence(reference, tempfix, outfile, false);

            }
        }

        private void FetchProjectReferedBy(Project project, ProjectList projects, string suffix, StreamWriter outfile, string rootName)
        {
            Console.WriteLine(string.Format("Try to find projects have directly refer to {0} from {1}", project.Name, rootName));
            
            foreach (Project parentProject in projects)
            {
                if (!ProcessedLinks.Contains(parentProject.Name + "-" + rootName))
                {
                    if (parentProject.ReferencedProjects.Any(p => p.Name == project.Name) || parentProject.ReferenceList.Contains(project.Name))
                    {
                        Console.WriteLine(string.Format("{0} --> {3}; project path is {2}", parentProject.Name, project.Name, parentProject.ProjectPath, suffix));
                        outfile.WriteLine(string.Format("{0} --> {3}; project path is {2}", parentProject.Name, project.Name, parentProject.ProjectPath, suffix));
                        suffix = string.Format(" {0} --> {1}", parentProject.Name, suffix);
                        ProcessedLinks.Add(parentProject.Name + "-" + rootName);
                        FetchProjectReferedBy(parentProject, projects, suffix, outfile, rootName);
                    }
                }
            }
        }
        

        private void GetDistinctProjects(ProjectList projects, ProjectList projectList)
        {
            // ProjectList projectList = new ProjectList();
            foreach (Project project in projects)
            {

                if (!projectList.Any(x => x.Name == project.Name))
                    projectList.Add(project);

                foreach (string refer in project.ReferenceList)
                {
                    Project tmp = new Project();
                    tmp.Name = refer;
                    if (!projectList.Any(x => x.Name == refer))
                        projectList.Add(tmp);
                }

                foreach (Project reference in project.ReferencedProjects)
                {
                    if (!projectList.Any(x => x.Name == reference.Name))
                    {
                        projectList.Add(reference);
                        GetDistinctProjects(reference.ReferencedProjects, projectList);
                    }
                }

            }

        }

        private void WriteRefencesToXML(StringBuilder sbRefer, ProjectList projects)
        {
            foreach (Project project in projects)
            {
                string tempfix = project.Name;
                Console.WriteLine(tempfix + ": " + project.ProjectPath);
                // sbRefer.AppendLine(project.Name + ":" + project.ProjectPath);
                foreach (string refer in project.ReferenceList)
                {
                    Console.WriteLine(tempfix + " Assembly Refer: " + refer);
                    // sbRefer.AppendLine(prefix +" Assembly Refer:" + refer);
                }
                foreach (Project reference in project.ReferencedProjects)
                {
                    //if not processed output link and add to processed list
                    if (!ProcessedLinks.Contains(project.Name + "-" + reference.Name))
                    {
                        // sb.Append("\"" + project.Name + "\" -> \"" + reference.Name + "\"" + Environment.NewLine);
                        ProcessedLinks.Add(project.Name + "-" + reference.Name);
                        Console.WriteLine(tempfix + " Project Refer: " + reference.Name);
                        // sbRefer.AppendLine(prefix + " Project Refer:" + reference.Name);
                        
                    }
                }

            }
            // return sbRefer;
        }

        /// <summary>
        /// Given a filepath for a JPG, get all the projects and their dependencies and output a JPG file to that path.
        /// Download graphviz from http://www.graphviz.org/pub/graphviz/ARCHIVE/graphviz-2.14.1.exe
        /// </summary>
        /// <param name="OutputJPGFilePath"></param>
        private void VisualOutput(ProjectList projects, string OutputJPGFilePath)
        {
            //get temp file for outputting dot file for graphviz
            string DOTFile = System.IO.Path.GetTempFileName();

            StringBuilder sb = new StringBuilder("");

            //beginning dot text required
            sb.Append("digraph G {" + Environment.NewLine);

            //loop through every project and output its dependencies to the file in the format
            //and then through it sdependencies and so on and so forth
            AppendProjectLinks(sb, projects);

            //ending dot text required
            sb.Append(Environment.NewLine + "}");

            //output the file for DOT to handle
            System.IO.File.WriteAllText(DOTFile, sb.ToString());
            Console.Write(sb.ToString());

            //get a reference to the registry key used for graphviz
            Microsoft.Win32.RegistryKey GraphvizKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\ATT\\Graphviz");
            if (GraphvizKey == null) throw new ApplicationException("Unable to find Graphviz registry entry - is it installed?");

            //get path to dot.exe, should be in bin directory in install path
            string DotExePath = System.IO.Path.Combine(GraphvizKey.GetValue("InstallPath").ToString(), "bin\\dot.exe");

            //call out to graphviz to create JPG file
            System.Diagnostics.Process.Start(
                DotExePath,
                "-Tjpg \"" + DOTFile + "\" \"-o" + OutputJPGFilePath + "\""
            );

            //let's wait for it to show up, then return
            //TODO: Include some sort of timeout in case the dot.exe hangs or errors out?
            while (true)
            {
                if (System.IO.File.Exists(OutputJPGFilePath))
                    break;
                else
                    System.Threading.Thread.Sleep(500); //wait half a second before checking again
            }
        }

        //keep processed list so we don't output duplicates
        private static List<string> ProcessedLinks = new List<string>();

        /// <summary>
        /// This will output all dependencies since that is what we care about. Will need some additional work if we want to display straggler projects with no dependencies also.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="projects"></param>
        private void AppendProjectLinks(StringBuilder sb, ProjectList projects)
        {
            //loop through every project and output its dependencies to the file in the format
            //parent -> child
            //order doesn't matter for dot files
            foreach (Project project in projects)
            {
                foreach (Project reference in project.ReferencedProjects)
                {
                    //if not processed output link and add to processed list
                    if (!ProcessedLinks.Contains(project.Name + "-" + reference.Name))
                    {
                        sb.Append("\"" + project.Name + "\" -> \"" + reference.Name + "\"" + Environment.NewLine);
                        ProcessedLinks.Add(project.Name + "-" + reference.Name);
                    }

                    AppendProjectLinks(sb, project.ReferencedProjects);
                }
                
            }
        }
    }

}
