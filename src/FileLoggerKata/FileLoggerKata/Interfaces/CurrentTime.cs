using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoggerKata.Interfaces
{
    public class CurrentTime : ICurrentTime
    {
        public DateTime Now => DateTime.Now;
    }
}
