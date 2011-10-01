using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace CppCheckAddIn
  {
  class ErrorHandler
    {
    private ErrorListProvider mErrorListProvider;
    private DTE2 mDTE2;

    public ErrorHandler(DTE2 iDTE2)
      {
      mDTE2 = iDTE2;
      CreateErrorListProvider();
      }

    public void AddWarning(string iDocument, int iLine, string iMessage)
      {
      AddTask(iDocument, iLine, iMessage, TaskPriority.Normal);
      }

    public void AddError(string iDocument, int iLine, string iMessage)
      {
      AddTask(iDocument, iLine, iMessage, TaskPriority.High);
      }

    public void AddMessage(string iDocument, int iLine, string iMessage)
      {
      AddTask(iDocument, iLine, iMessage, TaskPriority.Low);
      }

    public void Clear()
      {
      mErrorListProvider.Tasks.Clear();      
      }
   
    private void NavigateTo(object iSender, EventArgs iArgs)
      {
      Task task = (Task)iSender;
      if (task == null)
        return;

      Window openedFile = mDTE2.ItemOperations.OpenFile(task.Document, EnvDTE.Constants.vsViewKindCode);
      TextSelection textSelection = (TextSelection)mDTE2.ActiveDocument.Selection;
      textSelection.GotoLine(task.Line+1, false);
      }

    private void AddTask(string iDocument, int iLine, string iMessage, TaskPriority iPriority)
      {
      Task task = new Task();
      task.Document = iDocument;
      task.Line     = iLine;
      task.Text     = iMessage;
      task.Priority = iPriority;
      task.Navigate += new EventHandler(this.NavigateTo);

      mErrorListProvider.Tasks.Add(task);
      }

    public TaskProvider.TaskCollection Tasks 
      {
      get 
        {
        return mErrorListProvider.Tasks;
        }
      }

    private void CreateErrorListProvider()
      {
      IServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)mDTE2);
      mErrorListProvider = new ErrorListProvider(serviceProvider);
      mErrorListProvider.ProviderName = "CppCheck Errors";
      mErrorListProvider.ProviderGuid = new Guid("5A10E43F-8D1D-4026-98C0-E6B502058901");
      }
    }
  }
