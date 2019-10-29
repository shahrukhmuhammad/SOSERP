using System;

namespace CMS.Entity
{
    public class CmsSocialMedia
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LinkOrder { get; set; }
        public string LinkURL { get; set; }
        public string Icon { get; set; }
    }
}
