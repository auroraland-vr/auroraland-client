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

namespace Nakama
{
    public class NAvatar : INAvatar
    {
        public string Id { get; private set; }
        public string Url { get; private set; }
        public string Name { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string ThumbnailHdUrl { get; private set; }
        public string Metadata { get; private set; }
        public int Version { get; private set; }

        internal NAvatar(Avatar message)
        {
            Id = message.Id;
            Url = message.Url;
            Name = message.Name;
            ThumbnailUrl = message.ThumbnailUrl;
            ThumbnailHdUrl = message.ThumbnailHdUrl;
            Metadata = message.Metadata;
            Version = message.Version;
        }

        public override string ToString()
        {
            var f = "NAvatar(Id={0}, Name={1}, Url={2}, Version={3}, ThumbnailUrl={4}, ThumbnailHdUrl={5}, Metadata={6})";
            return String.Format(f, Id, Name, Url, Version, ThumbnailUrl, ThumbnailHdUrl, Metadata);
        }
    }
}
