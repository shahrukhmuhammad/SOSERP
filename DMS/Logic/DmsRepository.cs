using BaseApp.Entity;
using DMS.Entity;
using Insight.Database;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DMS.Logic
{
    public abstract class DmsRepository
    {
        [Sql("Dms_SelectById")]
        public abstract IList<DmsItem> SelectById(Guid Id);

        [Sql("Dms_GetFilesById")]
        public abstract IList<DmsItem> GetFilesById(Guid Id);

        [Sql("DmsItem_Insert")]
        public abstract Guid Insert(DmsItem item);

        [Sql("DmsItem_Rename")]
        public abstract bool Update(DmsItem item);

        [Sql("Dms_GetFileById")]
        public abstract DmsItem GetFileById(Guid Id);

        [Sql("DmsItem_GetRevisions")]
        public abstract IList<DmsItem> GetRevisions(Guid Id);

        [Sql("DmsItem_RestoreRevision")]
        public abstract DmsItem RestoreRevision(Guid Id, Guid CreatedBy);

        [Sql("Dms_GetItemsWithPaths")]
        public abstract IList<DmsItem> GetItemsWithPaths(IList<Guid> Ids);

        [Sql("Dms_GetAllChildIdsByDirId")]
        public abstract IList<DmsItem> GetAllChildIdsByDirId(Guid Id);

        [Sql("Dms_BreadCrumbByDirId")]
        public abstract IList<DmsItem> BreadCrumbByDirId(Guid id);

        [Sql("Dms_GetSearchResults")]
        public abstract IList<DmsItem> GetSearchResults(Guid Id, string SearchPhrase);

        [Sql("Dms_GetPermissionsByItemId")]
        public abstract IList<AppUser> GetPermissionsByItemId(Guid Id);

        [Sql("Dms_GetSharedContentsByUserId")]
        public abstract IList<DmsItem> GetSharedContentsByUserId(Guid Id);

        public abstract IDbConnection GetConnection();


        public string AddDir(string path, string dirName, Guid? userId = null, bool isSystem = false)
        {
            var conn = GetConnection();

            Guid newGuid;
            var root = GetRouteId(path);
            if (Guid.TryParse(root, out newGuid))
            {
                int dirExists = conn.QuerySql<int>("SELECT Count(1) FROM DmsRepository WHERE ParentId=@ParentId AND Title=@Title AND ResourceType=1", new { Title = dirName, ParentId = newGuid }).SingleOrDefault();
                if (dirExists < 1)
                {
                    Insert(new DmsItem { ParentId = newGuid, Title = dirName, ResourceType = DmsResourceType.Folder, CreatedBy = userId, IsSystem = isSystem });
                    return "OK";
                }
                else
                {
                    return "Directry already exists.";
                }
            }
            else
            {
                return root;
            }
        }

        public string DeleteDir(string path, bool forceDelete = false)
        {
            Guid newGuid;
            var root = GetRouteId(path);
            if (Guid.TryParse(root, out newGuid))
            {
                if (forceDelete)
                {
                    DeleteDir(newGuid);
                    return "OK";
                }
                else
                {
                    var conn = GetConnection();
                    bool isSystem = conn.QuerySql<bool>("SELECT IsSystem FROM DmsRepository WHERE Id=@Id", new { Id = newGuid }).SingleOrDefault();
                    if (!isSystem)
                    {
                        DeleteDir(newGuid);
                        return "OK";
                    }
                    else return "System directory can not be deleted.";
                }
            }
            else
            {
                return root;
            }
        }

        private void DeleteDir(Guid id)
        {
            List<string> AllFiles = new List<string>();
            AllFiles.Add(id.ToString());
            foreach (var m in GetAllChildIdsByDirId(id))
            {
                AllFiles.Add(m.Id.ToString());
            }

            DeleteFiles(AllFiles);
        }

        public string AddFile(string path, byte[] file, string fileName, string contentType, Guid? userId = null)
        {
            if (file.Length > 0)
            {
                Guid newGuid;
                var root = GetRouteId(path);
                if (Guid.TryParse(root, out newGuid))
                {
                    var dmsItem = new DmsItem
                    {
                        Title = Path.GetFileNameWithoutExtension(fileName),
                        Extension = Path.GetExtension(fileName).Replace(".", null),
                        ResourceType = DmsResourceType.File,
                        FileSize = file.Length,
                        CreatedBy = userId,
                        ContentType = contentType,
                        ParentId = newGuid,
                        IsSystem = false
                    };
                    Guid itemId = Insert(dmsItem);

                    var dmsPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Dms\\";
                    File.WriteAllBytes(dmsPath + itemId, file);

                    return itemId.ToString();
                }
                else
                {
                    return root;
                }
            }
            else
            {
                return "Invalid file size.";
            }
        }

        public void DeleteFile(Guid id)
        {
            List<string> AllFiles = new List<string>();

            foreach (var fId in GetRevisions(id))
            {
                AllFiles.Add(fId.Id.ToString());
            }
            DeleteFiles(AllFiles);
        }

        private void DeleteFiles(List<string> AllFiles)
        {
            var conn = GetConnection();
            conn.QuerySql("DELETE FROM DmsRepository WHERE Id IN(@Id); DELETE FROM DmsRepositoryPermissions WHERE DocId IN (@Id)", new { Id = AllFiles.ToArray() });

            var dmsPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Dms\\";
            foreach (string i in AllFiles)
            {
                if (File.Exists(dmsPath + i)) File.Delete(dmsPath + i);
            }
        }

        public string RenderPath(string path, string viewMode)
        {
            Guid newGuid;
            var rootId = GetRouteId(path);
            if (Guid.TryParse(rootId, out newGuid))
            {
                return @"<iframe src='/dms/docs/index/" + rootId + "/?p=" + rootId + "&viewMode=" + viewMode + "' name='" + GetRandomFrameName() + "' class='dms-frame' frameborder='0' scrolling='no' onload='resizeIframe(this)'></iframe>";
            }
            return rootId;
        }

        public string RenderShared()
        {
            return @"<iframe src='/DMS/Docs/SharedDocs' name='" + GetRandomFrameName() + "' class='dms-frame' frameborder='0' scrolling='no' onload='resizeIframe(this)'></iframe>";
        }

        public byte[] GetZip(string path)
        {
            Guid newGuid;
            var root = GetRouteId(path);

            if (Guid.TryParse(root, out newGuid))
            {
                return ZipBytes(newGuid.ToString());
            }
            else
            {
                throw new Exception();
            }
        }

        public byte[] ZipBytes(string ids)
        {
            var allIds = ids.Split(',').Select(x => Guid.Parse(x)).ToArray();

            var dmsPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Dms\\";

            var items = GetItemsWithPaths(allIds);

            using (ZipFile zip = new ZipFile())
            {
                foreach (DmsItem item in items)
                {
                    if (item.ResourceType == DmsResourceType.File)
                    {
                        if (File.Exists(dmsPath + item.Id)) zip.AddFile(dmsPath + item.Id).FileName = item.Path;
                    }
                    else zip.AddDirectoryByName(item.Path);
                }

                byte[] bytZip;

                using (MemoryStream stream = new MemoryStream())
                {
                    zip.Save(stream);
                    bytZip = stream.ToArray();
                }

                return bytZip;
            }
        }

        public IList<DmsItem> GetFiles(string path)
        {
            var conn = GetConnection();
            Guid newGuid;
            var rootId = GetRouteId(path);
            if (Guid.TryParse(rootId, out newGuid))
            {
                return GetFilesById(newGuid);
            }
            return null;
        }

        public string GetRouteId(string root)
        {
            var conn = GetConnection();
            var dirs = root.TrimStart('/').TrimEnd('/').Split('/');
            Guid dirId = new Guid("00000000-0000-0000-0000-000000000000");

            foreach (var title in dirs)
            {
                dirId = conn.QuerySql<Guid>("SELECT TOP 1 Id FROM DmsRepository WHERE Title=@Title AND ParentId=@Id", new { Id = dirId, Title = title }).SingleOrDefault();
                if (dirId == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    return "Error rendering Dms root.";
                }
            }

            return dirId.ToString();
        }

        private string GetRandomFrameName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];
            return new string(stringChars);
        }
    }
}
