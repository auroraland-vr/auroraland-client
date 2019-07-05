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
using Google.Protobuf;

namespace Nakama
{
    public class NEntitiesListMessage : INCollatedMessage<INResultSet<INEntity>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NEntitiesListMessage()
        {
            payload = new Envelope {EntitiesList = new TEntitiesList()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NEntitiesListMessage(SpaceId={0},PageLimit={1},OrderByAsc={2},Cursor={3})";
            var p = payload.EntitiesList;
            return String.Format(f, p.SpaceId, p.PageLimit, p.OrderByAsc, p.Cursor);
        }

        public class Builder
        {
            private NEntitiesListMessage message;

            public Builder()
            {
                message = new NEntitiesListMessage();
            }

            public Builder PageLimit(long pageLimit)
            {
                message.payload.EntitiesList.PageLimit = pageLimit;
                return this;
            }

            public Builder OrderByAsc(bool orderByAsc)
            {
                message.payload.EntitiesList.OrderByAsc = orderByAsc;
                return this;
            }

            public Builder Cursor(INCursor cursor)
            {
                message.payload.EntitiesList.Cursor = cursor.Value;
                return this;
            }

            public Builder SpaceId(string spaceId)
            {
                message.payload.EntitiesList.SpaceId = spaceId;
                return this;
            }

            public NEntitiesListMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NEntitiesListMessage();
                message.payload.EntitiesList = new TEntitiesList(original.payload.EntitiesList);
                return original;
            }
        }
    }
}
