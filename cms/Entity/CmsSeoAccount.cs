using System;
using System.Collections.Generic;

namespace CMS.Entity
{
    public class CmsSeoAccount
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public CmsSeoAccountStatus Status { get; set; }
        public string ClientId { get; set; }

        public List<CmsSeoAccountDetail> Services { get; set; }
    }
    public enum CmsSeoAccountStatus
    {
        Configured = 1,
        UnConfigured = 2
    }

    public class CmsSeoAccountDetail
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string Title { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenType { get; set; }
        public string AccessTokenExpiresIn { get; set; }

        public virtual CmsSeoAccount Provider { get; set; }
    }
}
