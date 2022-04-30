using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Models.EventArgs
{
    public class ModuleEventArgs
    {
        public Guid Guid { get; set; }

        public object Module { get; set; }
    }
}
