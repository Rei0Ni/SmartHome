using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Application.Enums;

namespace SmartHome.Application.Delegates
{
    public delegate T ServiceResolver<T>(ComponentHealthChecks key);
}
