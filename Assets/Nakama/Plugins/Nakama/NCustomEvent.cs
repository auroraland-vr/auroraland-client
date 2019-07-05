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

namespace Nakama
{
    public class NCustomEvent : INCustomEvent
    {
        public string UserId { get; set; }
        public long CreatedAt { get; set; }
        public string Metadata { get; set; }

        public NCustomEvent(){}

        internal NCustomEvent(CustomEvent message)
        {
            UserId = message.UserId;
            CreatedAt = message.CreatedAt;
            Metadata = message.Metadata;
        }

        public CustomEvent ToProto(){
            return new CustomEvent{UserId=UserId, CreatedAt=CreatedAt, Metadata=Metadata};
        }

        public override string ToString()
        {
            var f = "NCustomEvent(UserId={0},Metadata={1},CreatedAt={2})";
            return String.Format(f, UserId, Metadata, CreatedAt);
        }
    }
}
