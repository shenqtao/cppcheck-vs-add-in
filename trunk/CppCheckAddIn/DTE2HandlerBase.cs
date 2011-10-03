using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;

namespace CppCheckAddIn
  {
  public class DTE2HandlerBase
    {
    protected DTE2 mApplication;

    public DTE2HandlerBase(DTE2 iApplication)
      {
      mApplication = iApplication;
      }
    }
  }
