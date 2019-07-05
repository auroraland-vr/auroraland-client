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
    public class NSpaceCreateMessage : INCollatedMessage<INResultSet<INSpace>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NSpaceCreateMessage()
        {
            payload = new Envelope {SpacesCreate = new TSpacesCreate { Spaces =
            {
                new List<TSpacesCreate.Types.SpaceCreate>()
            }}};   
        }

        private NSpaceCreateMessage(string displayName)
        {
            payload = new Envelope {SpacesCreate = new TSpacesCreate { Spaces =
            {
                new List<TSpacesCreate.Types.SpaceCreate>
                {
                    new TSpacesCreate.Types.SpaceCreate {DisplayName = displayName}
                }
            }}};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            var f = "NSpaceCreateMessage(DisplayName={0},Description={1},AssetId={2},Lang={3},Private={4},Metadata={5}";
            var output = "";
            foreach (var p in payload.SpacesCreate.Spaces)
            {
                output += String.Format(f, p.DisplayName, p.Description, p.AssetId, p.Lang, p.Private, p.Metadata); 
            }
            return output;
        }

        public class Builder
        {
            private NSpaceCreateMessage message;

            public Builder(string displayName)
            {
                message = new NSpaceCreateMessage(displayName);
            }

            public Builder Description(string description)
            {
                message.payload.SpacesCreate.Spaces[0].Description = description;
                return this;
            }

            public Builder AssetId(string assetId)
            {
                message.payload.SpacesCreate.Spaces[0].AssetId = assetId;
                return this;
            }

            public Builder Lang(string lang)
            {
                message.payload.SpacesCreate.Spaces[0].Lang = lang;
                return this;
            }

            public Builder Private(bool isPrivate)
            {
                message.payload.SpacesCreate.Spaces[0].Private = isPrivate;
                return this;
            }

            public Builder Metadata(string metadata)
            {
                message.payload.SpacesCreate.Spaces[0].Metadata = metadata;
                return this;
            }

            public NSpaceCreateMessage Build()
            {
                // Clone object so builder now operates on new copy.
                var original = message;
                message = new NSpaceCreateMessage();
                message.payload.SpacesCreate = new TSpacesCreate(original.payload.SpacesCreate);
                return original;
            }
        }
    }
}
