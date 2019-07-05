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
    public class NEntitySyncMessage : INUncollatedMessage
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NEntitySyncMessage()
        {
            payload = new Envelope {EntitiesSync = new TEntitiesSync()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public class Builder
        {
            private NEntitySyncMessage message;

            public Builder(string spaceId)
            {
                message = new NEntitySyncMessage();
                message.payload.EntitiesSync.SpaceId = spaceId;
            }

            public Builder EntityDelta(INEntityDelta delta)
            {
                message.payload.EntitiesSync.Deltas.Add(delta.ToProto());
                return this;
            }

            public NEntitySyncMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NEntitySyncMessage();
                message.payload.EntitiesSync = new TEntitiesSync(original.payload.EntitiesSync);
                return original;
            }
        }
    }
}
