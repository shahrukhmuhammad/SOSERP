using Microsoft.AspNet.SignalR;

namespace WebApp.Hubs
{
    public class RealTimeHub : Hub
    {
        public string Activate()
        {
            return "Monitor Activated";
        }

        public void UpdateNotifications(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateNotifications(message);
        }

        public void UpdateMessages(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateMessages(message);
        }

        #region Base App
        public void UpdateAppUsers(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateAppUsers(message);
        }

        public void UpdateAppUserRoles(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateAppUserRoles(message);
        }

        public void UpdateSmtpControls(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateSmtpControls(message);
        }
        #endregion

        #region CMS
        public void UpdateCmsPages(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsPages(message);
        }

        public void UpdateCmsSlides(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsSlides(message);
        }

        public void UpdateCmsNews(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsNews(message);
        }

        public void UpdateCmsNewsletterSubscribers(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsNewsletterSubscribers(message);
        }

        public void UpdateCmsNewsletterPosts(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsNewsletterPosts(message);
        }

        public void UpdateCmsSocialMediaIcons(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsSocialMediaIcons(message);
        }

        public void UpdateCmsFileManager(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsFileManager(message);
        }

        public void UpdateCmsContents(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCmsContents(message);
        }
        #endregion

        #region DMS
        public void UpdateDMS(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateDMS(message);
        }
        #endregion

        #region Clinicul
        public void UpdateCliniculPatients(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCliniculPatients(message);
        }

        public void UpdateCliniculAppointments(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateCliniculAppointments(message);
        }
        #endregion

        #region Employee Management
        public void UpdateEmployees(string message)
        {
            GlobalHost
           .ConnectionManager
           .GetHubContext<RealTimeHub>().Clients.All.UpdateEmployees(message);
        }
        #endregion
    }
}