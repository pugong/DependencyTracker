using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CompanyName.DependencyTracker
{
    /// <summary>
    /// A list of projects
    /// </summary>
    public class ProjectList : List<Project>
    {
        public ProjectList() : base() { }

        /// <summary>
        /// For visualizing the list easily and for debugging and testing contents of the list
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> Names = new List<string>();

            foreach (Project project in this)
                Names.Add(project.Name);

            return String.Join(", ", Names.ToArray());
        }

        public string ToReferenceXML()
        {
            StringBuilder sb = new StringBuilder();
            /* <table x=����ʾ�����ꡱ y="��ʾ������" name="��Ŀ����" realtablename="��Ŀ����">
              <row name=���ļ����ơ�>
            <comment>�ļ�����</comment>
            </row>
              <row name=���ļ����ơ�>
                <comment>�ļ�����</comment>
                <relation table="��Ŀ����" row="�ļ�����" />
              </row>
              <comment>��Ŀ����</comment>
            </table>*/
            int i = 0;
            foreach (Project project in this)
            {
                GenerateProjectXML(project, sb, i);
                
            }
            return sb.ToString();

        }

        private void GenerateProjectXML(Project project, StringBuilder sb, int i)
        {

            sb.AppendFormat("<table x=\"{0}\" y=\"{1}\" name=\"{2}\" realtablename=\"{2}\">", 20, 20 * i++, project.Name);
            sb.AppendLine();
            sb.AppendFormat("    <row name=\"{0}\">", project.Name);
            sb.AppendLine();
            sb.AppendFormat("        <comment>{0}</comment>", project.ProjectPath);
            sb.AppendLine();
            if (!project.Name.Contains(".dll"))
            {
                sb.AppendFormat("        <relation table=\"{0}\" row=\"{0}\" />", project.Name);
                sb.AppendLine();
            }
            sb.AppendLine("    </row>");
            foreach (string refer in project.ReferenceList)
            {
                GenerateReferXML(refer, sb);
            }

            foreach (Project refer in project.ReferencedProjects)
            {
                GenerateReferXML(refer, sb);
            }

            sb.AppendLine("</table>");
        }

        private void GenerateReferXML(Project refer, StringBuilder sb)
        {
            sb.AppendFormat("    <row name=\"{0}\">", refer.Name);
            sb.AppendLine();
            sb.AppendFormat("        <comment>{0}</comment>", refer.ProjectPath);
            sb.AppendLine();
            sb.AppendFormat("        <relation table=\"{0}\" row=\"{1}\" />", refer.Name, refer.Name);
            sb.AppendLine();
            sb.AppendLine("    </row>");
        }

        private void GenerateReferXML(string refer, StringBuilder sb)
        {
            sb.AppendFormat("    <row name=\"{0}\">", refer);
            sb.AppendLine();
            sb.AppendFormat("        <comment>{0}</comment>", refer);
            sb.AppendLine();
            sb.AppendFormat("        <relation table=\"{0}\" row=\"{1}\" />", refer, refer);
            sb.AppendLine();
            sb.AppendLine("    </row>");
        }
    }

}
