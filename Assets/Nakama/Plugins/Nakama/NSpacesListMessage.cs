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
    public class NSpacesListMessage : INCollatedMessage<INResultSet<INSpace>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NSpacesListMessage()
        {
            payload = new Envelope {SpacesList = new TSpacesList()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NSpacesListMessage(PageLimit={0},OrderByAsc={1},Cursor={2},Lang={3},CreatedAt={4},Count={5},FilterCase={6})";
            var p = payload.SpacesList;
            return String.Format(f, p.PageLimit, p.OrderByAsc, p.Cursor, p.Lang, p.CreatedAt, p.Count, p.FilterCase);
        }

        public class Builder
        {
            private NSpacesListMessage message;

            public Builder()
            {
                message = new NSpacesListMessage();
            }

            public Builder PageLimit(long pageLimit)
            {
                message.payload.SpacesList.PageLimit = pageLimit;
                return this;
            }

            public Builder OrderByAsc(bool orderByAsc)
            {
                message.payload.SpacesList.OrderByAsc = orderByAsc;
                return this;
            }

            public Builder Cursor(INCursor cursor)
            {
                message.payload.SpacesList.Cursor = cursor.Value;
                return this;
            }

            public Builder FilterByLang(string lang)
            {
                message.payload.SpacesList.ClearFilter();
                message.payload.SpacesList.Lang = lang;
                return this;
            }

            public Builder FilterByCreatedAt(long createdAt)
            {
                message.payload.SpacesList.ClearFilter();
                message.payload.SpacesList.CreatedAt = createdAt;
                return this;
            }

            public Builder FilterByCount(long count)
            {
                message.payload.SpacesList.ClearFilter();
                message.payload.SpacesList.Count = count;
                return this;
            }

            public Builder FilterByCreatorId(string creatorId)
            {
                message.payload.SpacesList.ClearFilter();
                message.payload.SpacesList.CreatorId = creatorId;
                return this;
            }

            public NSpacesListMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NSpacesListMessage();
                message.payload.SpacesList = new TSpacesList(original.payload.SpacesList);
                return original;
            }
        }
    }
}
