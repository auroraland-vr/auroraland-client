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
    public class NEntitySnapshot : INEntitySnapshot
    {
        public string EntityId { get; set; }
        public INVector3 Position { get; set; }
        public INVector3 Velocity { get; set; }
        public INVector3 Rotation { get; set; }

        public NEntitySnapshot(){}

        internal NEntitySnapshot(EntitySnapshot message)
        {
            EntityId = message.EntityId;

            if (message.Position!=null){
                Position = new NVector3(message.Position);
            }
            if (message.Velocity != null){
                Velocity = new NVector3(message.Velocity);
            }
            if (message.Rotation != null){
                Rotation = new NVector3(message.Rotation);
            }
        }

        public EntitySnapshot ToProto(){
            EntitySnapshot snapshot = new EntitySnapshot{EntityId=EntityId};
            snapshot.Position = Position.ToProto();
            snapshot.Velocity = Velocity.ToProto();
            snapshot.Rotation = Rotation.ToProto();
            return snapshot;
        }

        public override string ToString()
        {
            var f = "NEntitySnapshot(EntityId={0},Position={1},Velocity={2},Rotation={3}";
            return String.Format(f, EntityId, Position, Velocity, Rotation);
        }
    }
}
