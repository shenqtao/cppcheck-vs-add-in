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
    private Process mCppCheckProcess;

    public event OutputLineHandler OutputLineReceived;
    public event OutputLineHandler ErrorLineReceived; 

    public void Start()
      {
      mCppCheckProcess = new Process();
      mCppCheckProcess.StartInfo.FileName = "CppCheck";
      mCppCheckProcess.StartInfo.Arguments = ComposeArgumentString();
      
      mCppCheckProcess.StartInfo.UseShellExecute = false;
      mCppCheckProcess.StartInfo.RedirectStandardOutput = true;
      mCppCheckProcess.StartInfo.RedirectStandardError = true;
      
      mCppCheckProcess.OutputDataReceived += new DataReceivedEventHandler(this.InternalOutputHandler);
      mCppCheckProcess.ErrorDataReceived += new DataReceivedEventHandler(this.InternalErrorHandler);
      mCppCheckProcess.Start();
      
      mCppCheckProcess.BeginOutputReadLine();
      mCppCheckProcess.BeginErrorReadLine();
      }

    public void Stop()
      {
      if (!mCppCheckProcess.HasExited)
        mCppCheckProcess.Kill();
      }

    // TODO
    private void Suspend()
      {
      if (!mCppCheckProcess.HasExited) ;
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
