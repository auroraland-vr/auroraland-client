using System;
using System.Collections.Generic;
using Nakama;

namespace Auroraland
{
    public class NKListArgs<T> : EventArgs
    {
        public readonly List<T> values;
        public NKListArgs(INResultSet<T> inputs)
        {
            List<T> temp = new List<T>();
            for (int i = 0; i < inputs.Results.Count; i++)
            {
                temp.Add(inputs.Results[i]);
            }
            this.values = temp;
        }

        public NKListArgs(IList<T> inputs)
        {
            List<T> temp = new List<T>();
            for (int i = 0; i < inputs.Count; i++)
            {
                temp.Add(inputs[i]);
            }
            this.values = temp;
        }
    }
}