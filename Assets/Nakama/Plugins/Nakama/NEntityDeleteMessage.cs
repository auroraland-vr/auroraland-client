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
    public class NEntityDeleteMessage : INUncollatedMessage
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NEntityDeleteMessage()
        {
            payload = new Envelope {EntitiesDelete = new TEntitiesDelete {}};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NEntityDeleteMessage(SpaceId={0},EntityIds={1})";
            return String.Format(f, payload.EntitiesDelete.SpaceId, payload.EntitiesDelete.EntityIds.ToString());
        }

        public class Builder
        {
            private NEntityDeleteMessage message;

            public Builder(string spaceId)
            {
                message = new NEntityDeleteMessage();
                message.payload.EntitiesDelete.SpaceId = spaceId;
            }

            public Builder EntityId(string entityId)
            {
                message.payload.EntitiesDelete.EntityIds.Add(entityId);
                return this;
            }

            public NEntityDeleteMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NEntityDeleteMessage();
                message.payload.EntitiesDelete = new TEntitiesDelete(original.payload.EntitiesDelete);
                return original;
            }
        }
    }
}
