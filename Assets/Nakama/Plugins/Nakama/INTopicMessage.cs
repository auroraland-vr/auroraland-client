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
    public enum TopicMessageType : uint { Chat = 0, GroupJoin, GroupAdd, GroupLeave, GroupKick, GroupPromoted, SpaceJoin=11, SpaceLeave=13, MasterClient=30, ToMasterClient=99}

    public interface INTopicMessage
    {
        INTopicId Topic { get; }
        string UserId { get; }
        string MessageId { get; }
        long CreatedAt { get; }
        long ExpiresAt { get; }
        string Handle { get; }
        TopicMessageType Type { get; }
        string Data { get; }
    }
}
