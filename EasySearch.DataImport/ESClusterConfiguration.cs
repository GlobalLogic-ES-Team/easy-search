using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySearch.DataImport
{
    public class ESClusterConfiguration
    {
        public ESConfigSection ESConfigSection
        {
            get
            {
                return (ESConfigSection)ConfigurationManager.GetSection("ESConfigSection");
            }
        }

        public IEnumerable<NodeElement> Nodes
        {
            get
            {
                foreach (NodeElement selement in this.Nodes)
                {
                    if (selement != null)
                        yield return selement;
                }
            }
        }
    }
}
