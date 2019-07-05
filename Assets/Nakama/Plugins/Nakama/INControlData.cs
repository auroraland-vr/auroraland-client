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
    public interface INControlData
    {
        string UserId { get; set; }
        string EntityId { get; set; }
        bool RunKeyPressed { get; set; }
        bool SitKeyPressed { get; set; }
        bool KickKeyPressed { get; set; }
        float InputMove { get; set; }
        float InputTurn { get; set; }
        float InputStrafe { get; set; }
        bool Grounded { get; set; }
        bool Sitting { get; set; }
        bool FrontAgainstWall { get; set; }
        bool BackAgainstWall { get; set; }
        int IntensityOfHeadTurn { get; set; }
        int IntensityOfLeftHand { get; set; }
        int IntensityOfRightHand { get; set; }
        bool MinimumTurnReached { get; set; }

        INVector3 LeftHandPosition { get; set; }
        INVector4 LeftHandRotation { get; set; }
        INVector3 RightHandPosition { get; set; }
        INVector4 RightHandRotation { get; set; }
        INVector3 LookAt { get; set; }
        bool IkModeEnabled { get; set; }

        INVector3 Position { get; set; }
        INVector3 Rotation { get; set; }
        int PackedBoolean { get; set; }

        ControlData ToProto();
    }
}
