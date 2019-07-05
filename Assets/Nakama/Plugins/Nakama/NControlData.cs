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
    public class NControlData : INControlData
    {
        public string UserId { get; set; }
        public string EntityId { get; set; }
        public bool RunKeyPressed { get; set; }
        public bool SitKeyPressed { get; set; }
        public bool KickKeyPressed { get; set; }
        public float InputMove { get; set; }
        public float InputTurn { get; set; }
        public float InputStrafe { get; set; }
        public bool Grounded { get; set; }
        public bool Sitting { get; set; }
        public bool FrontAgainstWall { get; set; }
        public bool BackAgainstWall { get; set; }
        public int IntensityOfHeadTurn { get; set; }
        public int IntensityOfLeftHand { get; set; }
        public int IntensityOfRightHand { get; set; }
        public bool MinimumTurnReached { get; set; }
        public INVector3 LeftHandPosition { get; set; }
        public INVector4 LeftHandRotation { get; set;  }
        public INVector3 RightHandPosition { get; set;  }
        public INVector4 RightHandRotation { get; set;  }
        public INVector3 LookAt { get; set;  }
        public bool IkModeEnabled { get; set; }

        public INVector3 Position { get; set; }
        public INVector3 Rotation { get; set; }

        public int PackedBoolean{ get; set; }

        public NControlData(){}

        internal NControlData(ControlData message)
        {
            UserId = message.UserId;
            EntityId = message.EntityId;
            RunKeyPressed = message.RunKeyPressed;
            SitKeyPressed = message.SitKeyPressed;
            KickKeyPressed = message.KickKeyPressed;
            InputMove = message.InputMove;
            InputTurn = message.InputTurn;
            InputStrafe = message.InputStrafe;
            Grounded = message.Grounded;
            Sitting = message.Sitting;
            FrontAgainstWall = message.FrontAgainstWall;
            BackAgainstWall = message.BackAgainstWall;
            IntensityOfHeadTurn = message.IntensityOfHeadTurn;
            IntensityOfLeftHand = message.IntensityOfLeftHand;
            IntensityOfRightHand = message.IntensityOfRightHand;
            MinimumTurnReached = message.MinimumTurnReached;
            IkModeEnabled = message.IkModeEnabled;
            PackedBoolean = message.PackedBoolean;

            if (message.LeftHandPosition != null) {
                LeftHandPosition = new NVector3(message.LeftHandPosition);
            } else {
                LeftHandPosition = new NVector3(0,0,0);
            }

            if (message.LeftHandRotation != null) {
                LeftHandRotation = new NVector4(message.LeftHandRotation);
            }else{
                LeftHandRotation = new NVector4(0,0,0,0);
            }

            if (message.RightHandPosition != null) {
                RightHandPosition =  new NVector3(message.RightHandPosition);
            } else {
                RightHandPosition = new NVector3(0,0,0);
            }

            if (message.RightHandRotation != null) {
                RightHandRotation = new NVector4(message.RightHandRotation);
            }else{
                RightHandRotation = new NVector4(0,0,0,0);
            }
            
            if (message.LookAt != null) {
                LookAt = new NVector3(message.LookAt);
            } else {
                LookAt = new NVector3(0,0,0);
            }

            if (message.Position != null){
                Position = new NVector3(message.Position);
            } else{
                Position = new NVector3(0,0,0);
            }

            if (message.Rotation != null){
                Rotation = new NVector3(message.Rotation);
            } else{
                Rotation = new NVector3(0,0,0);
            }
        }

        public ControlData ToProto(){
            ControlData message = new ControlData();
            message.UserId = UserId;
            message.EntityId = EntityId;
            message.RunKeyPressed = RunKeyPressed;
            message.SitKeyPressed = SitKeyPressed;
            message.KickKeyPressed = KickKeyPressed;
            message.InputMove = InputMove;
            message.InputTurn = InputTurn;
            message.InputStrafe = InputStrafe;
            message.Grounded = Grounded;
            message.Sitting = Sitting;
            message.FrontAgainstWall = FrontAgainstWall;
            message.BackAgainstWall = BackAgainstWall;
            message.IntensityOfHeadTurn = IntensityOfHeadTurn;
            message.IntensityOfLeftHand = IntensityOfLeftHand;
            message.IntensityOfRightHand = IntensityOfRightHand;
            message.MinimumTurnReached = MinimumTurnReached;
            message.IkModeEnabled = IkModeEnabled;
            message.PackedBoolean = PackedBoolean;

            if (LeftHandPosition != null) {
                message.LeftHandPosition = LeftHandPosition.ToProto();
            }
            if (LeftHandRotation != null) {
                message.LeftHandRotation = LeftHandRotation.ToProto();
            }
            if (RightHandPosition != null) {
                message.RightHandPosition = RightHandPosition.ToProto();
            }
            if (RightHandRotation != null) {
                message.RightHandRotation = RightHandRotation.ToProto();
            }
            if (LookAt != null){
                message.LookAt = LookAt.ToProto();
            }
            if (Position != null){
                message.Position = Position.ToProto();
            }
            if (Rotation != null){
                message.Rotation = Rotation.ToProto();
            }

            return message;
        }

        public override string ToString()
        {
            var f = "NControlData(UserId={0},EntityId={1},RunKeyPressed={2},SitKeyPressed={3},KickKeyPressed={4},"
            +"InputMove={5},InputTurn={6},InputStrafe={7},Grounded={8},Sitting={9},FrontAgainstWall={10},BackAgainstWall={11},"
            +"IntensityOfHeadTurn={12},IntensityOfLeftHand={13},IntensityOfRightHand={14},MinimumTurnReached={15},"
            +"LeftHandPosition={16},LeftHandRotation={17},RightHandPosition={18},RightHandRotation={19},"
            +"LookAt={20},IkModeEnabled={21},Position={22},Rotation={23},PackedBoolean={24})";
            return String.Format(f, UserId, EntityId, RunKeyPressed,SitKeyPressed,KickKeyPressed,InputMove,InputTurn,InputStrafe,
                                 Grounded,Sitting,FrontAgainstWall,BackAgainstWall,IntensityOfHeadTurn,IntensityOfLeftHand,IntensityOfRightHand,
                                 MinimumTurnReached, LeftHandPosition, LeftHandRotation, RightHandPosition, RightHandRotation,
                                 LookAt, IkModeEnabled, Position, Rotation, PackedBoolean);
        }
    }
}
