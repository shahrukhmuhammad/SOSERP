using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class AppSMTP
    {
        public AppSMTP()
        {
            appmodules = new List<AppModuleSMTP>();
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public AppSMTPServerType ServerType { get; set; }
        public string IncomingServer { get; set; }
        public string OutgoingServer { get; set; }
        public int IncomingPort { get; set; }
        public int OutgoingPort { get; set; }
        public bool SSL { get; set; }
        public string DefaultUsername { get; set; }
        public string DefaultPassword { get; set; }
        public string Description { get; set; }
        public string ModuleId { get; set; }
        public bool IsDefault { get; set; }
        public string ModuleName { get; set; }
        public ThirdParty ThirdParty { get; set; }
        public bool TLS { get; set; }
        public List<AppModuleSMTP> appmodules { get; set; }
    }

    public class AppModuleSMTP
    {
        public long Id { get; set; }
        public Guid SMTPId { get; set; }
        public string ModuleId { get; set; }
    }
    public enum AppSMTPServerType
    {
        POP = 1,
        IMAP = 2
    }

    public enum ThirdParty
    {
        ICloud = 1,
        Exchange = 2,
        Google = 3,
        Yahoo = 4,
        Outlook = 5,
        Other = 6
    }


}
