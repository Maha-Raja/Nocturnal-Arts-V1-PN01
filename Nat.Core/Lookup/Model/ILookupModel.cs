using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Lookup.Model
{
    public interface ILookupModel
    {
        string GetHiddenValue();
        string GetVisibleValue();
    }
}
