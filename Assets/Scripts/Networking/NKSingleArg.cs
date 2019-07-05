using System;

namespace Auroraland
{
    public class NKSingleArg<T> : EventArgs
    {
        public readonly T value;

        public NKSingleArg(T obj)
        {
            this.value = obj;
        }
    }
}