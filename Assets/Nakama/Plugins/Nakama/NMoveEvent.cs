
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
    public class NMoveEvent : INMoveEvent
    {
        public INEntitySnapshot EntitySnapshot { get; set; }
        public INVector3 Force { get; set; }
        public ForceModeType ForceMode { get; set; }
        public string UserId { get; set; }
        public long CreatedAt { get; set; }
        public string Metadata { get; set; }

        internal NMoveEvent(MoveEvent message )
        {
            EntitySnapshot = new NEntitySnapshot(message.EntitySnapshot);

            UserId = message.UserId;
            CreatedAt = message.CreatedAt;
            Metadata = message.Metadata;
            
            if (message.Force!=null){
                Force = new NVector3(message.Force);
            }


            switch (message.ForceMode)
            {
                case 0:
                    ForceMode = ForceModeType.Acceleration;
                    break;
                case 1:
                    ForceMode = ForceModeType.Force;
                    break;
                case 2:
                    ForceMode = ForceModeType.Impulse;
                    break;
                case 3:
                    ForceMode = ForceModeType.VelocityChange;
                    break;
            }
        }

        public MoveEvent ToProto(){
            MoveEvent evt = new MoveEvent{UserId=UserId, CreatedAt=CreatedAt, Metadata=Metadata};
            evt.Force = Force.ToProto();
            evt.ForceMode = (int) ForceMode;
            evt.EntitySnapshot = EntitySnapshot.ToProto();
            return evt;
        }

        public NMoveEvent(){}

        public override string ToString()
        {
            var f = "NMoveEvent(EntitySnapshot={0},Force={1},ForceMode={2},UserId={3},Metadata={4},CreatedAt={5}";
            return String.Format(f, EntitySnapshot, Force, ForceMode, UserId, Metadata, CreatedAt);
        }
    }
}
