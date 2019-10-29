using System.Collections.Generic;
using System.Linq;

namespace BaseApp.Entity
{
    public class EmailTemplate
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string BodyContent { get; set; }
        public string DefaultContent { get; set; }
    }

    public class EmailTemplateType
    {
        #region Contacts
        public const string ContactSignup = "Contact Signup";
        public const string ContactRequest = "Contact Request";
        #endregion

        public const string FormSubmission = "Form Submission";
        public const string PasswordRecovery = "Password Recovery";
        public const string UsernameRecovery = "Username Recovery";
        public const string MessageReceived = "Message Received";
        public const string UserSignup = "User Signup";
        public const string PasswordChange = "Password Change";

        #region Scheduling
        public const string CreateSchedule = "Create Schedule";
        public const string UpdateSchedule = "Update Schedule";
        #endregion

        #region Dms
        public const string ShareDmsLinks = "Share Dms Links";
        public const string ShareDmsAttachment = "Share Dms Attachment";
        #endregion

        public static Dictionary<string, string> List()
        {
            return typeof(EmailTemplateType).GetFields().ToDictionary(k => k.Name, v => v.GetValue(null).ToString());
        }
    }
}
