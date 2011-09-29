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
    
    /// <summary> VC++ projects list </summary>
    public List<Project> Projects { get { return mProjects; } }

    /// <summary> Solution to parse. </summary>
    public Solution Solution
      {
      get
        { 
        return mSolution; 
        }
      set
        {
        Reset();
        mSolution = value;
        }
      }

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
      Reset();

      if (Solution == null)
        return;

      foreach (Project project in mSolution.Projects)
        ParseProject(project);
      }

    /// <summary> Default values. </summary>
    public void Reset()
      {
      Projects.Clear();
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
