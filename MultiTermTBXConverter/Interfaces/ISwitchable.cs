using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTermTBXMapper
{
    public interface ISwitchable
    {
        void UtilizeState<T>(T state);
        //void UtilizeState<T>(ref T r);
        void UtilizeState<T1,T2>(T1 r, T2 state);
    }
}
