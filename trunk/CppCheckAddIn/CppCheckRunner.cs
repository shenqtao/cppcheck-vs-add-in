using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CppCheckAddIn
  {
  public delegate void OutputLineHandler(object iSender, string iLine);

  class CppCheckRunner
    {
    private List<string> mCheckItems;

    public event OutputLineHandler OutputLineReceived;
    public event OutputLineHandler ErrorLineReceived; 

    public void Start()
      {
      Process cppCheckProcess;
      cppCheckProcess = new Process();
      cppCheckProcess.StartInfo.FileName = "CppCheck";
      cppCheckProcess.StartInfo.Arguments = ComposeArgumentString();

      cppCheckProcess.StartInfo.UseShellExecute = false;
      cppCheckProcess.StartInfo.RedirectStandardOutput = true;
      cppCheckProcess.StartInfo.RedirectStandardError = true;

      cppCheckProcess.OutputDataReceived += new DataReceivedEventHandler(this.InternalOutputHandler);
      cppCheckProcess.ErrorDataReceived += new DataReceivedEventHandler(this.InternalErrorHandler);
      cppCheckProcess.Start();

      cppCheckProcess.BeginOutputReadLine();
      cppCheckProcess.BeginErrorReadLine();
      }

    private bool ValidateOutputLine(string iLine)
      {
      List<Regex> syntaxes = new List<Regex>();
      // 1/196 files checked 0% done  checked \d{1,3} done$
      syntaxes.Add(new Regex(@"^\d+/\d+ files checked \d+% done$"));
      //Checking d:\some.cpp...
      syntaxes.Add(new Regex(@"Checking .+\.c(pp)?...$"));

      foreach (Regex syntax in syntaxes)
        if (syntax.Match(iLine).Success)
          return true;

      return false;
      }

    private bool ValidateErrorLine(string iLine)
      {
      return true;
      }

    private void InternalOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
      {
      string line = outLine.Data;

      if (!String.IsNullOrEmpty(line) && ValidateOutputLine(line))
        {
        OnOutputLineReceived(line);
        }
      }

    private void InternalErrorHandler(object sendingProcess,
        DataReceivedEventArgs outLine)
      {
      string line = outLine.Data;

      if (!String.IsNullOrEmpty(line) && ValidateErrorLine(line))
        {
        OnErrorLineReceived(line);
        }
      }

    private void OnOutputLineReceived(string iLine)
      {
      if (OutputLineReceived != null)
        OutputLineReceived(this, iLine);
      }

    private void OnErrorLineReceived(string iLine)
      {
      if (ErrorLineReceived != null)
        ErrorLineReceived(this, iLine);
      }

    private string ComposeArgumentString()
      {
      string arguments = "--template {file}:::{line}:::{message} --enable=all d:\\DevDental\\Polaris\\src\\DentsplyIOSSharedLib\\DentsplyIOSSharedLib\\AttachmentListControl.cpp";

      return arguments;
      }
    }
  }
