using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Interfaces
{
    public interface IWpfWindow
    {
        event EventHandler Activated;
        event EventHandler Deactivated;
        bool IsActive { get; }
    }
}
