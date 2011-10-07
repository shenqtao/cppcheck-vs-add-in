using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace CppCheckAddIn
  {
  public class OutputHandler : DTE2HandlerBase
    {
    private OutputWindowPane mOutputWindowPane;

    public OutputHandler(DTE2 iApplication) : base(iApplication)
      {
      mApplication = iApplication;
      CreateOutputWindowPane();
      }

    public void OutputMessage(string iMessage)
      {
      mOutputWindowPane.OutputString(iMessage);
      mOutputWindowPane.OutputString("\n");
      }

    private void CreateOutputWindowPane()
      {
      OutputWindow outputWindow = mApplication.ToolWindows.OutputWindow;
      mOutputWindowPane = outputWindow.OutputWindowPanes.Add("CppCheck");
      }
    }
  }
