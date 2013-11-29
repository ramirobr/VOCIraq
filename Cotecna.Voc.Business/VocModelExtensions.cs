using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotecna.Voc.Business
{
    public partial class VocEntities
    {
        public List<Certificate> ExtendedQuery() {
            return this.Certificates.Where(x => x.Sequential == "VOCIRQ0001").ToList();
        }
    }
}
