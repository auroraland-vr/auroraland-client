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
    public class NVector3 : INVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
  
        internal NVector3(V3 message)
        {
            X = message.X;
            Y = message.Y;
            Z = message.Z;
        }

        public NVector3(float x, float y, float z){
            X=x;
            Y=y;
            Z=z;
        }

        public V3 ToProto(){
            return new V3{X=X, Y=Y, Z=Z};
        }

        public double DistanceTo(INVector3 v){
            return Math.Pow(X-v.X, 2f) + Math.Pow(Y-v.Y,2f) + Math.Pow(Z-v.Z,2f);
        }

        public NVector3(){}

        public override string ToString()
        {
            var f = "NVector3(X={0},Y={1},Z={2})";
            return String.Format(f, X,Y,Z);
        }
    }
}
