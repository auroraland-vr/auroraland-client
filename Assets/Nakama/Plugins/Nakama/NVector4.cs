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
    public class NVector4 : INVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
  
        internal NVector4(V4 message)
        {
            X = message.X;
            Y = message.Y;
            Z = message.Z;
            W = message.W;
        }

        public NVector4(float x, float y, float z, float w){
            X=x;
            Y=y;
            Z=z;
            W=w;
        }

        public V4 ToProto(){
            return new V4{X=X, Y=Y, Z=Z, W=W};
        }

        public double DistanceTo(INVector4 v){
            return Math.Pow(X-v.X, 2f) + Math.Pow(Y-v.Y,2f) + Math.Pow(Z-v.Z,2f) + Math.Pow(W-v.W, 2f);
        }

        public NVector4(){}

        public override string ToString()
        {
            var f = "NVector4(X={0},Y={1},Z={2},W={3})";
            return String.Format(f, X,Y,Z,W);
        }
    }
}
