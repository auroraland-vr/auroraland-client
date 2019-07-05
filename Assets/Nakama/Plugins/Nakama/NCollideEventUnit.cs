
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
    public class NCollideEventUnit : INCollideEventUnit
    {
        public INEntitySnapshot EntitySnapshotA { get; set; }
        public INEntitySnapshot EntitySnapshotB { get; set; }
        public IList<INVector3> ContactPoints { get; set; }
        public INVector3 Impulse { get; set; }
        public INVector3 RelativeVelocity { get; set; }

        internal NCollideEventUnit(CollideEventUnit message)
        {
            EntitySnapshotA = new NEntitySnapshot(message.EntityA);
            EntitySnapshotB = new NEntitySnapshot(message.EntityB);

            if (message.Impulse!=null){
                Impulse = new NVector3(message.Impulse);
            }

            if (message.RelativeVelocity!=null){
                RelativeVelocity = new NVector3(message.RelativeVelocity);
            }

            ContactPoints = new List<INVector3>();
            foreach (var cp in message.ContactPoints) {
                ContactPoints.Add(new NVector3(cp));
            }
        }

        public CollideEventUnit ToProto(){
            CollideEventUnit unit = new CollideEventUnit();
            unit.EntityA = EntitySnapshotA.ToProto();
            unit.EntityB = EntitySnapshotB.ToProto();
            unit.Impulse = Impulse.ToProto();
            unit.RelativeVelocity = RelativeVelocity.ToProto();
            foreach (var point in ContactPoints) {
                unit.ContactPoints.Add(point.ToProto());
            }
            return unit;
        }

        public override string ToString()
        {
            var f = "NCollideEventUnit(EntityA={0},EntityB={1},ContactPoints={2},Impulse={3},RelativeVelocity={4}";
            return String.Format(f, EntitySnapshotA, EntitySnapshotB, ContactPoints, Impulse, RelativeVelocity);
        }
    }
}
