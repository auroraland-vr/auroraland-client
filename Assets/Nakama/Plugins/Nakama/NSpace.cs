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
    public class NSpace : INSpace
    {
        public string Id { get; set; }
        public string CreatorId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Lang { get; set; }
        public long UtcOffsetMs { get; set; }
        public string Metadata { get; set; }
        public long Count { get; set; }
        public bool Private { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }

        public string Url { get;set; }
        public int Version { get;set; }
        public int ThumbnailVersion { get;set; }

        public string Theme { get; set; }

        public string CreatorFullname { get; set; }

        internal NSpace(Space message)
        {
            Id = message.Id;
            CreatorId = message.CreatorId;
            DisplayName = message.DisplayName;
            Description = message.Description;
            ThumbnailUrl = message.ThumbnailUrl;
            Lang = message.Lang;
            UtcOffsetMs = message.UtcOffsetMs;
            Metadata = message.Metadata;
            Count = message.Count;
            Private = message.Private;
            CreatedAt = message.CreatedAt;
            UpdatedAt = message.UpdatedAt;
            Url = message.Url;
            Version = message.Version;
            ThumbnailVersion = message.ThumbnailVersion;
            Theme = message.Theme;
            CreatorFullname = message.CreatorFullname;
        }

        public override string ToString()
        {
            var f = "NSpace(Id={0},CreatorId={1},DisplayName={2},Description={3},Url={4},Version={5},ThumbnailUrl={6},ThumbnailVersion={7}" +
                    "Lang={8},UtcOffsetMs={9}, Metadata={10},Count={11},Private={12},CreatedAt={13},UpdatedAt={14},Theme={15}, CreatorFullname={16})";
            return String.Format(f, Id, CreatorId, DisplayName, Description, Url, Version, ThumbnailUrl, ThumbnailVersion,
                    Lang, UtcOffsetMs, Metadata, Count, Private, CreatedAt, UpdatedAt, Theme, CreatorFullname);
        }
    }
}
