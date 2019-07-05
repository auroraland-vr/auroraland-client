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

namespace Nakama
{
    public interface INSpace
    {
        string Id { get; set; }
        string CreatorId { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        string ThumbnailUrl { get; set; }
        string Lang { get; set; }
        long UtcOffsetMs{ get; set; }
        string Metadata { get; set; }
        long Count { get; set; }
        bool Private { get; set; }
        long CreatedAt { get; set; }
        long UpdatedAt { get; set; }

        string Url { get; set; }
        int Version { get; set; }
        int ThumbnailVersion { get; set; }

        string Theme { get; set; }
        string CreatorFullname {get; set;}
    }
}
