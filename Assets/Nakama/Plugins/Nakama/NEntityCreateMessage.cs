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
    public class NEntityCreateMessage : INCollatedMessage<INResultSet<INEntity>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NEntityCreateMessage()
        {
            payload = new Envelope {EntitiesCreate = new TEntitiesCreate { Entities =
            {
                new List<TEntitiesCreate.Types.EntityCreate>{
                    new TEntitiesCreate.Types.EntityCreate{}
                }
            }}};   
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NEntityCreateMessage(SpaceId={0},Position={1},Rotation={2},Scale={3},AssetId={4},Metadata={5})";
            var output = "";
            foreach (var p in payload.EntitiesCreate.Entities)
            {
                output += String.Format(f, p.SpaceId, p.Position.ToString(), p.Rotation.ToString(), p.Scale.ToString(), p.AssetId, p.Metadata); 
            }
            return output;
        }

        public class Builder
        {
            private NEntityCreateMessage message;

            public Builder()
            {
                message = new NEntityCreateMessage();
            }

            public Builder Owned(bool owned)
            {
                message.payload.EntitiesCreate.Owned = owned;
                return this;
            }
            public Builder SpaceId(string spaceId)
            {
                message.payload.EntitiesCreate.Entities[0].SpaceId = spaceId;
                return this;
            }

            public Builder Position(NVector3 position)
            {
                message.payload.EntitiesCreate.Entities[0].Position = position.ToProto();
                return this;
            }

            public Builder Rotation(NVector3 rotation)
            {
                message.payload.EntitiesCreate.Entities[0].Rotation = rotation.ToProto();
                return this;
            }

            public Builder Scale(NVector3 scale)
            {
                message.payload.EntitiesCreate.Entities[0].Scale = scale.ToProto();
                return this;
            }

            public Builder AssetId(string assetId)
            {
                message.payload.EntitiesCreate.Entities[0].AssetId = assetId;
                return this;
            }

            public Builder Metadata(string metadata)
            {
                message.payload.EntitiesCreate.Entities[0].Metadata = metadata;
                return this;
            }

            public NEntityCreateMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NEntityCreateMessage();
                message.payload.EntitiesCreate = new TEntitiesCreate(original.payload.EntitiesCreate);
                return original;
            }
        }
    }
}
