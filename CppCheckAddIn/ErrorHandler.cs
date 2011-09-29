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
