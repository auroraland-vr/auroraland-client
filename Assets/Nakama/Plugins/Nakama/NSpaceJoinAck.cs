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
    public class NSpaceJoinAck : INSpaceJoinAck
    {
        public string SpaceId { get; private set; }
        public IList<INEntity> Entities { get; private set; }
        public long SpaceSeq { get; private set; }
        public int UserSeq { get; private set; }

        internal NSpaceJoinAck(TSpaceJoinAck message)
        {
            SpaceId = message.SpaceId;

            var entities = new List<INEntity>();
            foreach (var entity in message.Entities)
            {
                entities.Add(new NEntity(entity));
            }
            Entities = entities.ToArray();

            SpaceSeq = message.SpaceSeq;
            UserSeq = message.UserSeq;
        }

        public override string ToString()
        {
            var entities = new List<string>();
            foreach (var entity in Entities){
                entities.Add(entity.ToString());
            }
            var entityStr = "["+string.Join(",\n", entities.ToArray())+"]";
            var f = "NSpaceJoinAck(SpaceId={0},SpaceSeq={1},UserSeq={2},NEntities={3}";
            return String.Format(f, SpaceId, SpaceSeq, UserSeq, entityStr);
        }
    }
}
