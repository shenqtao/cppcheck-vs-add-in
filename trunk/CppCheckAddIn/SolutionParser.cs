using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.Text.RegularExpressions;

namespace CppCheckAddIn
  {
  /// <summary> Class for parsing the solution for VC++ projects. </summary>
  public class SolutionParser : DTE2HandlerBase
    {
    private Solution mSolution;
    private List<Project> mProjects;
    
    public enum ESelectionKind
      {
      selectionKindItem,
      selectionKindProject
      };

    /// <summary> VC++ projects list </summary>
    public List<Project> Projects { get { return mProjects; } }

    public string PathArgument { get; set; }

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
    public SolutionParser(DTE2 iApplication) : base(iApplication)
      {
      //mSolution = iSolution;
      mProjects = new List<Project>();
      PathArgument = "";
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

    public void ParseSelection(ESelectionKind iSelectionKind)
      {
      switch (iSelectionKind)
        {
        case ESelectionKind.selectionKindItem:
          ParseSelectionItem();
          break;
        }
      }

    private void ParseSelectionItem()
      {
      foreach (UIHierarchyItem selectedItem in (Array)mApplication.ToolWindows.SolutionExplorer.SelectedItems)
        {
        ProjectItem projectItem = null;

        try { projectItem = (ProjectItem)selectedItem.Object; }
        catch { };

        if (projectItem == null || !ValidateFile(projectItem.Name))
          return;

        string fullName = "\"" + projectItem.FileNames[0] + "\" ";
        
        PathArgument += fullName;
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

    private bool ValidateFile(string iFullName)
      {
      Regex cppFile = new Regex(".+\\.(cpp|c|hpp)$");
      return cppFile.Match(iFullName).Success;
      }
    }
  }
