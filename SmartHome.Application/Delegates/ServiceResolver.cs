using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Application.Delegates
{
    public delegate T ServiceResolver<T>(ComponentHealthChecks key);
}
