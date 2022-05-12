using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Models.EventArgs
{
    /// <summary>
    /// 模块变化事件参数
    /// </summary>
    public class ModuleEventArgs : System.EventArgs
    {
        public Guid Guid { get; }
        public Module Module { get; }
        public ModuleEventArgs(Guid guid, Module module)
        {
            Guid = guid;
            Module = module;
        }
    }
}
