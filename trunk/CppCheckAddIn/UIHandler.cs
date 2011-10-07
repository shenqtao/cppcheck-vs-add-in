using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace CppCheckAddIn
  {
  class UIHandler : DTE2HandlerBase
    {
    private string[] mCommandCaptions;

    private AddIn mAddIn;

    private List<CommandBarButton> mButtons;
    private List<CommandBarPopup> mPopups;

    public UIHandler(DTE2 iApplication, AddIn iAddIn) : base(iApplication)
      {
      mAddIn       = iAddIn;
      mButtons = new List<CommandBarButton>();
      mPopups = new List<CommandBarPopup>();

      mCommandCaptions = new string[] { "Check Solution", "Check Current Project", "Check Current File", "Suspend", "Stop", "CppCheck it", "CppCheck it Item"};
      }

    public string[] CommandNames 
      {
      get
        {
        string[] cmdNames = new string[ mCommandCaptions.Length ];

        for (int i = 0; i < mCommandCaptions.Length; ++i)
          cmdNames[i] = GetCmdName(mCommandCaptions[i]);
 
        return cmdNames;
        }
      }

    public void SetupUI()
      {
      Commands comands = mApplication.Commands;

      #region Tools submenu
      CommandBars commandBars = (CommandBars)mApplication.CommandBars;
      CommandBar toolCommandBar = commandBars["Tools"];

      CommandBarPopup cppCheckMenu = CreatePopup(toolCommandBar, "CppCheck");
      CommandBar cppCheckCommandBar = cppCheckMenu.CommandBar;

      Command cmdCheckSolution = CreateCommand(mCommandCaptions[0]);
      CommandBarButton btnCheckSolution = CreateButton(cmdCheckSolution, cppCheckCommandBar, mCommandCaptions[0]);

      Command cmdCheckProject = CreateCommand(mCommandCaptions[1]);
      CommandBarButton btnCheckProject = CreateButton(cmdCheckProject, cppCheckCommandBar, mCommandCaptions[1]);

      Command cmdCheckFile = CreateCommand(mCommandCaptions[2]);
      CommandBarButton btnCheckFile = CreateButton(cmdCheckFile, cppCheckCommandBar, mCommandCaptions[2]);

      Command cmdSuspendResume = CreateCommand(mCommandCaptions[3]);
      CommandBarButton btnSuspendResume = CreateButton(cmdSuspendResume, cppCheckCommandBar, mCommandCaptions[3], true);

      Command cmdStop = CreateCommand(mCommandCaptions[4]);
      CommandBarButton btnStop = CreateButton(cmdStop, cppCheckCommandBar, mCommandCaptions[4]);
      #region Savins for destroying
      mPopups.Add(cppCheckMenu);
      mButtons.Add(btnCheckSolution);
      mButtons.Add(btnCheckProject);
      mButtons.Add(btnCheckFile);
      mButtons.Add(btnSuspendResume);
      mButtons.Add(btnStop);
      #endregion

      #endregion

      #region Tab context menu
      CommandBar tabCommandBar = commandBars["Easy MDI Document Window"];

      Command cmdTabCheckIt = CreateCommand(mCommandCaptions[5]);
      CommandBarButton btnTabCheckIt = CreateButton(cmdTabCheckIt, tabCommandBar, mCommandCaptions[5], true);

      CommandBar codeWindowCommandBar = commandBars["Code Window"];
      CommandBarButton btnCodeWindowCheckIt = CreateButton(cmdTabCheckIt, codeWindowCommandBar, mCommandCaptions[5], true);
      #region Savins for destroying
      mButtons.Add(btnCodeWindowCheckIt);
      #endregion
      #endregion

      #region Solution explorer context menus
      CommandBar itemCommandBar = commandBars["Item"];

      Command cmdItemCheckIt = CreateCommand(GetCmdName(mCommandCaptions[5]) + "Item", mCommandCaptions[5]);
      CommandBarButton btnItemCheckIt = CreateButton(cmdItemCheckIt, itemCommandBar, mCommandCaptions[5], true);
      
      #region Saving for destroying
      mButtons.Add(btnItemCheckIt);
      #endregion

      #endregion
      }

    private string GetCmdName(string iCaption)
      {
      while (iCaption.Contains(' '))
        iCaption = iCaption.Remove(iCaption.IndexOf(' '), 1);

      return "Cmd" + iCaption;
      }

    public void ClearUI()
      {
      foreach (CommandBarButton button in mButtons)
        if (button != null)
          button.Delete(true);

      foreach (CommandBarPopup popup in mPopups)
        if (popup != null)
          popup.Delete(true);
      }

    public void CreateCommands()
      {
      }

    private Command CreateCommand(string iCaption)
      {
      string cmdName = GetCmdName(iCaption);
      return CreateCommand(cmdName, iCaption);
      }

    private Command CreateCommand(string iName, string iCaption)
      {
      Command command = null;
      string cmdName = GetCmdName(iCaption);

      try
        {
        command = mApplication.Commands.Item(mAddIn.ProgID + "." + iName, -1);
        }
      catch {};

      if (command == null)
        {
        object[] contextUIGuids = new object[] {};
        command = mApplication.Commands.AddNamedCommand(mAddIn, iName, iCaption, iCaption, true, 59, ref contextUIGuids, (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled));
        }

      return command;
      }

    private CommandBarButton CreateButton(Command iCommand, CommandBar iCommandBar, string iCaption, bool iBeginGroup = false, int iPosition = -1)
      {
      int position = (iPosition == -1) ? iCommandBar.Controls.Count + 1 : iPosition;

      CommandBarButton button = (CommandBarButton)iCommand.AddControl(iCommandBar, position);

      button.Caption = iCaption;
      button.BeginGroup = iBeginGroup;

      return button;
      }

    private CommandBarPopup CreatePopup(CommandBar iCommandBar, string iCaption, bool iBeginGroup = false, int iPosition = -1)
      {
      int position = (iPosition == -1) ? iCommandBar.Controls.Count + 1 : iPosition;

      CommandBarPopup popup = (CommandBarPopup)iCommandBar.Controls.Add(MsoControlType.msoControlPopup, System.Type.Missing, System.Type.Missing, position, true);

      popup.Caption         = iCaption;
      popup.BeginGroup      = iBeginGroup;
      popup.CommandBar.Name = iCaption;

      return popup;
      }

    }
  }
