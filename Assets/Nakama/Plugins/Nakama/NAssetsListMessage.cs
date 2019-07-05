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
    public class NAssetsListMessage : INCollatedMessage<INResultSet<INAsset>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NAssetsListMessage()
        {
            payload = new Envelope {AssetsList = new TAssetsList()};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NAssetsListMessage(PageLimit={0},OrderByAsc={1},Cursor={2},Name={3},FilterCase={4})";
            var p = payload.AssetsList;
            return String.Format(f, p.PageLimit, p.OrderByAsc, p.Cursor, p.Name, p.FilterCase);
        }

        public class Builder
        {
            private NAssetsListMessage message;

            public Builder()
            {
                message = new NAssetsListMessage();
            }

            public Builder PageLimit(long pageLimit)
            {
                message.payload.AssetsList.PageLimit = pageLimit;
                return this;
            }

            public Builder OrderByAsc(bool orderByAsc)
            {
                message.payload.AssetsList.OrderByAsc = orderByAsc;
                return this;
            }

            public Builder Cursor(INCursor cursor)
            {
                message.payload.AssetsList.Cursor = cursor.Value;
                return this;
            }

            public Builder FilterByName(string name)
            {
                message.payload.AssetsList.ClearFilter();
                message.payload.AssetsList.Name = name;
                return this;
            }

            public Builder FilterByCategory(string category)
            {
                message.payload.AssetsList.ClearFilter();
                message.payload.AssetsList.Category = category;
                return this;
            }
            
            public Builder FilterByType(string type)
            {
                message.payload.AssetsList.ClearFilter();
                message.payload.AssetsList.Type = type;
                return this;
            }

            public NAssetsListMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NAssetsListMessage();
                message.payload.AssetsList = new TAssetsList(original.payload.AssetsList);
                return original;
            }
        }
    }
}
