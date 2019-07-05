/**
 * Copyright 2017 The Nakama Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Nakama
{
    public class NControlDataSendMessage : INUncollatedMessage
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NControlDataSendMessage()
        {
            payload = new Envelope {ControlDataSend = new TControlDataSend()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public class Builder
        {
            private NControlDataSendMessage message;

            public Builder(string spaceId)
            {
                message = new NControlDataSendMessage();
                message.payload.ControlDataSend.SpaceId = spaceId;
            }

            public Builder Audience(Audience audience)
            {
                message.payload.ControlDataSend.Audience = audience;
                return this;
            }

            public Builder Exclude(string userID)
            {
                message.payload.ControlDataSend.Excludes.Add(userID);
                return this;
            }

            public Builder ControlData(NControlData controlData)
            {
                message.payload.ControlDataSend.ControlData = controlData.ToProto();
                return this;
            }

            public NControlDataSendMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NControlDataSendMessage();
                message.payload.ControlDataSend = new TControlDataSend(original.payload.ControlDataSend);
                return original;
            }
        }
    }
}
