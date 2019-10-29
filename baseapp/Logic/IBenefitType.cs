using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Logic
{
    public interface IBenefitType
    {
        [Sql("BenefitTypes_Insert")]
        Guid Insert(BenefitType model);

        [Sql("BenefitTypes_SelectAll")]
        IList<BenefitType> SelectAll();

        [Sql("BenefitTypes_Delete")]
        void Delete(Guid Id);
    }

    public interface IBonusType
    {
        [Sql("BonusTypes_Insert")]
        Guid Insert(BonusType model);

        [Sql("BonusTypes_SelectAll")]
        IList<BonusType> SelectAll();

        [Sql("BonusTypes_Delete")]
        void Delete(Guid Id);
    }
}
