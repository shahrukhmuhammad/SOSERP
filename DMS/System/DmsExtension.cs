using DMS.Entity;
using System;
using System.IO;
using System.Web;

namespace DMS.System
{
    #region Dms
    public static class FileSizeExtension
    {
        public static string SizeSuffix(this Int64 value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.00 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n2} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
    public static class DmsExtensions
    {
        public static string IconImg(this DmsItem item)
        {
            if (item.ResourceType == DmsResourceType.File)
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Content/ext/") + item.Extension + ".png"))
                {
                    return "/content/ext/" + item.Extension + ".png";
                }
                else
                {
                    return "/content/ext/unknown.png";
                }
            }
            else
            {
                return "/content/ext/folder.png";
            }
        }
    }

    public static class BaseUrl
    {
        public static string GetBaseUrl()
        {
            string BaseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;

            #if DEBUG
            BaseUrl += ":" + HttpContext.Current.Request.Url.Port;
            #endif

            return BaseUrl;
        }
    }

    #endregion
}
