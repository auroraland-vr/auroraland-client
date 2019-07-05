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
    public class NEventSendMessage : INUncollatedMessage
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NEventSendMessage()
        {
            payload = new Envelope {EventSend = new TEventSend()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public class Builder
        {
            private NEventSendMessage message;

            public Builder(string spaceId)
            {
                message = new NEventSendMessage();
                message.payload.EventSend.SpaceId = spaceId;
            }

            public Builder Audience(Audience audience)
            {
                message.payload.EventSend.Audience = audience;
                return this;
            }

            public Builder Exclude(string userID)
            {
                message.payload.EventSend.Excludes.Add(userID);
                return this;
            }

            public Builder TargetUser(string userID)
            {
                message.payload.EventSend.TargetUsers.Add(userID);
                return this;
            }

            public Builder MoveEvent(INMoveEvent evt){
                MoveEvent moveEvent = evt.ToProto();
                Event e = new Event{MoveEvent=moveEvent};
                message.payload.EventSend.Events.Add(e);
                return this;
            }

            public Builder CollideEvent(INCollideEvent evt){
                CollideEvent collideEvent = evt.ToProto();
                Event e = new Event{CollideEvent=collideEvent};
                message.payload.EventSend.Events.Add(e);
                return this;
            }

            public Builder CustomEvent(INCustomEvent evt){
                CustomEvent customEvent = evt.ToProto();
                Event e = new Event{CustomEvent=customEvent};
                message.payload.EventSend.Events.Add(e);
                return this;
            }

            public Builder ReadyEvent(INReadyEvent evt){
                ReadyEvent readyEvent = evt.ToProto();
                Event e = new Event{ReadyEvent=readyEvent};
                message.payload.EventSend.Events.Add(e);
                return this;
            }

            public NEventSendMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NEventSendMessage();
                message.payload.EventSend = new TEventSend(original.payload.EventSend);
                return original;
            }
        }
    }
}
