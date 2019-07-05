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

namespace Nakama
{
    public class NReadyEvent : INReadyEvent
    {
        public string UserId { get; set; }
        public string EntityId { get; set; }
        public string Metadata { get; set; }
        public long CreatedAt { get; set; }

        public NReadyEvent(){}

        internal NReadyEvent(ReadyEvent message)
        {
            UserId = message.UserId;
            EntityId = message.EntityId;
            CreatedAt = message.CreatedAt;
            Metadata = message.Metadata;
        }

        public ReadyEvent ToProto(){
            return new ReadyEvent{UserId=UserId, EntityId=EntityId, CreatedAt=CreatedAt, Metadata=Metadata};
        }

        public override string ToString()
        {
            var f = "NReadyEvent(UserId={0},EntityId={1},Metadata={2},CreatedAt={3})";
            return String.Format(f, UserId, EntityId, Metadata, CreatedAt);
        }
    }
}
