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
    public class NCollideEvent : INCollideEvent
    {
        public IList<INCollideEventUnit> Units { get; set; }
        public string UserId { get; set; }
        public long CreatedAt { get; set; }
        public string Metadata { get; set; }

        internal NCollideEvent(CollideEvent message)
        {
            UserId = message.UserId;
            CreatedAt = message.CreatedAt;
            Metadata = message.Metadata;

            Units = new List<INCollideEventUnit>();
            foreach (var ceu in message.Units) {
                Units.Add(new NCollideEventUnit(ceu));
            }
        }

        public CollideEvent ToProto(){
            CollideEvent evt = new CollideEvent{UserId=UserId,CreatedAt=CreatedAt,Metadata=Metadata};
            foreach (var unit in Units){
                evt.Units.Add(unit.ToProto());
            }
            return evt;
        }

        public override string ToString()
        {
            var f = "NCollideEvent(Units={0},UserId={1},Metadata={2},CreatedAt={3})";
            return String.Format(f, Units, UserId, Metadata, CreatedAt);
        }
    }
}
