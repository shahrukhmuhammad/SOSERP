using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Logic
{
    public interface ITaxTypes
    {
        [Sql("TaxTypes_Insert")]
        Guid Insert(TaxTypes model);

        [Sql("TaxTypes_SelectAll")]
        IList<TaxTypes> SelectAll();

        [Sql("TaxTypes_Delete")]
        void Delete(Guid Id);
    }
}
