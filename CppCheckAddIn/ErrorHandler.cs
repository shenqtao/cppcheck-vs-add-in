using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace CppCheckAddIn
  {
  class ErrorHandler : DTE2HandlerBase
    {
    private ErrorListProvider mErrorListProvider;

    public ErrorHandler(DTE2 iDTE2) : base(iDTE2)
      {
      CreateErrorListProvider();
      }

    public void PostWarning(string iDocument, int iLine, string iMessage)
      {
      PostTask(iDocument, iLine, iMessage, TaskPriority.Normal);
      }

    public void PostError(string iDocument, int iLine, string iMessage)
      {
      PostTask(iDocument, iLine, iMessage, TaskPriority.High);
      }

    public void PostMessage(string iDocument, int iLine, string iMessage)
      {
      PostTask(iDocument, iLine, iMessage, TaskPriority.Low);
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

      Window openedFile = mApplication.ItemOperations.OpenFile(task.Document, EnvDTE.Constants.vsViewKindCode);
      TextSelection textSelection = (TextSelection)mApplication.ActiveDocument.Selection;
      textSelection.GotoLine(task.Line+1, false);
      }

    private void PostTask(string iDocument, int iLine, string iMessage, TaskPriority iPriority)
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
      IServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)mApplication);
      mErrorListProvider = new ErrorListProvider(serviceProvider);
      mErrorListProvider.ProviderName = "CppCheck Errors";
      mErrorListProvider.ProviderGuid = new Guid("5A10E43F-8D1D-4026-98C0-E6B502058901");
      }
    }
  }
