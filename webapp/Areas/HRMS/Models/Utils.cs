using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Areas.HRM
{
    public static class Utils
    {
        public static bool IsFileExist(string file)
        {
            try
            {
                var absolutePath = HttpContext.Current.Server.MapPath(file);
                if (File.Exists(absolutePath))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }


}