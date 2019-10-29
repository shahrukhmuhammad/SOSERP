using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Logic
{
    public interface IInsurance
    {
        [Sql("Insurances_Insert")]
        Guid Insert(Insurance model);

        [Sql("Insurances_SelectAll")]
        IList<Insurance> SelectAll();

        [Sql("Insurances_Delete")]
        void Delete(Guid Id);
    }
}
