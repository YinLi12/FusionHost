using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinEps.Parser.Attributes
{
    /// <summary>
    /// Modules marked with this will be automatically loaded into parsing process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class ModuleTarget : System.Attribute
    {
        public ModuleTarget(Target target)
        {
            this.Target = target;
        }

        public Target Target { get; private set; }
    }

    internal enum Target
    {
        /// <summary>
        /// for validate/re-org the bytes message from network, and will be parsed to WinEps message object
        /// </summary>
        Incoming,

        /// <summary>
        /// for validate/re-org the WinEps message object to bytes message.
        /// </summary>
        Outgoing
    }
}
