using System;
using Nakama;

namespace Auroraland
{
    public class NKErrorArgs : EventArgs
    {
        public readonly string action;
        public readonly ErrorCode code;
        public readonly string message;

        public NKErrorArgs(string action, INError err)
        {
            this.action = action;
            code = err.Code;
            message = err.Message;
        }

        public NKErrorArgs(INError err)
        {
            code = err.Code;
            message = err.Message;
        }
    }
}