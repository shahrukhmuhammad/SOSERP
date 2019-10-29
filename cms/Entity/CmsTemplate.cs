using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Entity
{
    public class CmsTemplate
    {
        public Guid Id { get; set; }
        public string ThemeTitle { get; set; }
        public bool IsActive { get; set; }
        public string HeaderMarkup { get; set; }
        public string FooterMarkup { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedBy { get; set; }
    }
}
