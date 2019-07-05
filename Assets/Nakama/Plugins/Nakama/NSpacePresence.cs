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
    public class NSpacePresence : INSpacePresence
    {
        public string SpaceId { get; private set; }

        public IList<INEntity> Adds { get; private set; }

        public IList<INEntity> Dels { get; private set; }

        public IList<INEntity> Changes { get; private set; }

        internal NSpacePresence(SpacePresence message)
        {
            SpaceId = message.SpaceId;
            Adds = new List<INEntity>();
            Dels = new List<INEntity>();
            Changes = new List<INEntity>();

            foreach (var item in message.Adds)
            {
                Adds.Add(new NEntity(item));
            }
            foreach (var item in message.Dels)
            {
                Dels.Add(new NEntity(item));
            }
            foreach (var item in message.Changes)
            {
                Changes.Add(new NEntity(item));
            }
        }

        public override string ToString()
        {
            var f = "NSpacePresence(AddCount={0}, DelCount={1}, ChangeCount={2}, \nAdds={3}, Dels={4}, Changes={5}, SpaceId={6})";
            var entityAdds = new List<string>();
            var entityDels = new List<string>();
            var entityChanges = new List<string>();

            foreach (var entity in Adds){
                entityAdds.Add(entity.ToString());
            }

            foreach (var entity in Dels){
                entityDels.Add(entity.ToString());
            }

            foreach (var entity in Changes){
                entityChanges.Add(entity.ToString());
            }

            var addStr = "["+string.Join(",\n", entityAdds.ToArray())+"]";
            var delStr = "["+string.Join(",\n", entityDels.ToArray())+"]";
            var changeStr = "["+string.Join(",\n", entityChanges.ToArray())+"]";

            return String.Format(f, Adds.Count, Dels.Count, Changes.Count, addStr, delStr, changeStr, SpaceId);
        }
    }
}
