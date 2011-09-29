using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace CppCheckAddIn
  {
  /// <summary> Class for parsing the solution for VC++ projects. </summary>
  public class SolutionParser
    {
    private Solution mSolution;
    private List<Project> mProjects;
    
    public List<Project> Projects { get { return mProjects; } }

    /// <summary> Constructor implementation. </summary>
    /// <param name="iSolution"> Solution to parse. </param>
    public SolutionParser(Solution iSolution)
      {
      mSolution = iSolution;
      mProjects = new List<Project>();
      }

    /// <summary> Actual parsing function. </summary>
    public void ParseForVCProjects()
      {
      foreach (Project project in mSolution.Projects)
        ParseProject(project);
      }

    private void ParseProject(Project iProject)
      {
      if (iProject == null)
        return;

      if (IsVCProject(iProject))
        Projects.Add(iProject);
      else if (IsSolutionDirectory(iProject))
        ParseProjectItems(iProject.ProjectItems);
      }

    private void ParseProjectItems(ProjectItems iProjectItems)
      {
      foreach (ProjectItem projectItem in iProjectItems)
        {
        Project project = null;

        try
        { project = (Project)projectItem.Object; }
        catch
        { project = null; }

        ParseProject(project);
        }
      }

    private bool IsVCProject(Project iProject)
      {
      // "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}" - stanart GUID for VC++ projects
      return iProject.Kind == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
      }

    private bool IsSolutionDirectory(Project iProject)
      {
      return iProject.Kind == EnvDTE.Constants.vsProjectKindSolutionItems;
      }
    }
  }
