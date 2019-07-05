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
    public class NSpaceUpdateMessage : INCollatedMessage<bool>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NSpaceUpdateMessage()
        {
            payload = new Envelope {SpacesUpdate = new TSpacesUpdate { Spaces =
            {
                new List<TSpacesUpdate.Types.SpaceUpdate>()
            }}};
        }

        private NSpaceUpdateMessage(string spaceId)
        {
            payload = new Envelope {SpacesUpdate = new TSpacesUpdate { Spaces =
            {
                new List<TSpacesUpdate.Types.SpaceUpdate>
                {
                    new TSpacesUpdate.Types.SpaceUpdate {SpaceId = spaceId}
                }
            }}};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NSpaceUpdateMessage(SpaceId={0},DisplayName={1},Description={2},ThumbnailUrl={3},Lang={4},Metadata={5},Private={6})";
            var output = "";
            foreach (var p in payload.SpacesUpdate.Spaces)
            {
                output += String.Format(f, p.SpaceId, p.DisplayName, p.Description, p.ThumbnailUrl, p.Lang, p.Metadata, p.Private); 
            }
            return output;
        }

        public class Builder
        {
            private NSpaceUpdateMessage message;

            public Builder(string spaceId)
            {
                message = new NSpaceUpdateMessage(spaceId);
            }

            public Builder DisplayName(string displayName)
            {
                message.payload.SpacesUpdate.Spaces[0].DisplayName = displayName;
                return this;
            }

            public Builder Description(string description)
            {
                message.payload.SpacesUpdate.Spaces[0].Description = description;
                return this;
            }

            public Builder ThumbnailUrl(string thumbnailUrl)
            {
                message.payload.SpacesUpdate.Spaces[0].ThumbnailUrl = thumbnailUrl;
                return this;
            }

            public Builder Lang(string lang)
            {
                message.payload.SpacesUpdate.Spaces[0].Lang = lang;
                return this;
            }

            public Builder Metadata(string metadata)
            {
                message.payload.SpacesUpdate.Spaces[0].Metadata = metadata;
                return this;
            }

            public Builder Private(bool isPrivate)
            {
                message.payload.SpacesUpdate.Spaces[0].Private = isPrivate;
                return this;
            }

            public NSpaceUpdateMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NSpaceUpdateMessage();
                message.payload.SpacesUpdate = new TSpacesUpdate(original.payload.SpacesUpdate);
                return original;
            }
        }
    }
}
