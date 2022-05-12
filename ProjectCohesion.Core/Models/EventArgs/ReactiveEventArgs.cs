using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Models.EventArgs
{
    /// <summary>
    /// 属性变化事件参数
    /// </summary>
    public class ReactiveEventArgs : System.EventArgs
    {
        public Type DeclaringType { get; }
        public string PropertyName { get; }

        public ReactiveEventArgs(Type declaringType, string propertyName)
        {
            DeclaringType = declaringType;
            PropertyName = propertyName;
        }
    }
}
