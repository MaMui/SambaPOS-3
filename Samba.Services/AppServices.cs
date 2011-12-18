﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Samba.Domain.Models.Settings;
using Samba.Infrastructure.Data;
using Samba.Infrastructure.ExceptionReporter;
using Samba.Infrastructure.Settings;
using Samba.Persistance.Data;

namespace Samba.Services
{
    public static class AppServices
    {
        public static Dispatcher MainDispatcher { get; set; }

        private static IWorkspace _workspace;
        public static IWorkspace Workspace
        {
            get { return _workspace ?? (_workspace = WorkspaceFactory.Create()); }
            set { _workspace = value; }
        }

        private static MainDataContext _mainDataContext;
        public static MainDataContext MainDataContext
        {
            get { return _mainDataContext ?? (_mainDataContext = new MainDataContext()); }
            set { _mainDataContext = value; }
        }

        private static DataAccessService _dataAccessService;
        public static DataAccessService DataAccessService
        {
            get { return _dataAccessService ?? (_dataAccessService = new DataAccessService()); }
        }

        private static MessagingService _messagingService;
        public static MessagingService MessagingService
        {
            get { return _messagingService ?? (_messagingService = new MessagingService()); }
        }

        private static SettingService _settingService;
        public static SettingService SettingService
        {
            get { return _settingService ?? (_settingService = new SettingService()); }
        }

        private static IEnumerable<Terminal> _terminals;
        public static IEnumerable<Terminal> Terminals { get { return _terminals ?? (_terminals = Workspace.All<Terminal>()); } }

        private static Terminal _terminal;
        public static Terminal CurrentTerminal { get { return _terminal ?? (_terminal = GetCurrentTerminal()); } set { _terminal = value; } }

        public static bool CanStartApplication()
        {
            return LocalSettings.CurrentDbVersion <= 0 || LocalSettings.CurrentDbVersion == LocalSettings.DbVersion;
        }

        private static Terminal GetCurrentTerminal()
        {
            if (!string.IsNullOrEmpty(LocalSettings.TerminalName))
            {
                var terminal = Terminals.SingleOrDefault(x => x.Name == LocalSettings.TerminalName);
                if (terminal != null) return terminal;
            }
            var dterminal = Terminals.SingleOrDefault(x => x.IsDefault);
            return dterminal ?? Terminal.DefaultTerminal;
        }

        public static void LogError(Exception e)
        {
            MessageBox.Show("Bir sorun tespit ettik.\r\n\r\nProgram çalışmaya devam edecek ancak en kısa zamanda teknik destek almanız önerilir. Lütfen teknik destek için program danışmanınız ile irtibat kurunuz.\r\n\r\nMesaj:\r\n" + e.Message, "Bilgi", MessageBoxButton.OK, MessageBoxImage.Stop);
            Logger.Log(e);
        }

        public static void LogError(Exception e, string userMessage)
        {
            MessageBox.Show(userMessage, "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            Logger.Log(e);
        }

        public static void Log(string message)
        {
            Logger.Log(message);
        }

        public static void ResetCache()
        {
            _terminal = null;
            _terminals = null;
            MainDataContext.ResetCache();
            SettingService.ResetCache();
            SerialPortService.ResetCache();
            Dao.ResetCache();
            Workspace = WorkspaceFactory.Create();
        }
    }
}