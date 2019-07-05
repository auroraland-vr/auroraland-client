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
using System.Reflection;
using System.Collections.Generic;

namespace Nakama
{
    public class NAsset : INAsset
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Metadata { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public int Version { get; set; }
        public int ThumbnailVersion { get; set; }

        public NAsset(){}

        internal NAsset(Asset message)
        {
            Id = message.Id;
            Url = message.Url;
            Name = message.Name;
            ThumbnailUrl = message.ThumbnailUrl;
            Type = message.Type;
            Category = message.Category;
            Metadata = message.Metadata;
            CreatedAt = message.CreatedAt;
            UpdatedAt = message.UpdatedAt;
            Version = message.Version;
            ThumbnailVersion = message.ThumbnailVersion;
        }

        public Asset ToProto(){
            Asset e = new Asset{Id=Id, Name=Name, Url=Url, Version=Version, ThumbnailUrl=ThumbnailUrl, Type=Type, Category=Category, Metadata=Metadata, CreatedAt=CreatedAt, UpdatedAt=UpdatedAt, ThumbnailVersion=ThumbnailVersion};
            return e;
        }
        
        public INAsset Clone(){
            return new NAsset(this.ToProto());
        }

        public override string ToString()
        {
            var f = "NAsset(Id={0}, Name={1}, Url={2}, Version={3}, ThumbnailUrl={4}, ThumbnailVersion={5}, Type={6},Category={7},Metadata={8},CreatedAt={9},UpdatedAt={10})";
            return String.Format(f, Id, Name, Url, Version, ThumbnailUrl, ThumbnailVersion, Type, Category, Metadata, CreatedAt, UpdatedAt);
        }
    }
}
