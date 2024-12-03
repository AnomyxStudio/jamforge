using System;
using System.Collections;

namespace JamForge.Events
{
    public interface IDispatcher
    {
        void Dispatch(IEnumerator action);
        
        void Dispatch(Action action);
    }
}
