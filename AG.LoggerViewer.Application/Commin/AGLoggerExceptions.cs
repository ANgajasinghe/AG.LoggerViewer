using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.LoggerViewer.Application.Commin
{
    public class AGLoggerExceptions : Exception
    {
       
        public AGLoggerExceptions()
        {
        }

        public AGLoggerExceptions(string message) : base(message)
        {
        }

        public AGLoggerExceptions(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
