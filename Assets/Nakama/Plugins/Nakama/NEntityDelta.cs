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
    public class NEntityDelta : INEntityDelta
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public INVector3 Rotation { get; set; }
        public INVector3 Position { get; set; }
        public INVector3 Scale { get; set; }
        public string AssetUrl { get; set; }
        public string AssetType { get; set; }
        public string Metadata { get; set; }
        public string[] UpdatedFields { get; set; }
        public string AuthorityUserId { get; set; }


        public NEntityDelta(){}

        internal NEntityDelta(EntityDelta message)
        {
            Id = message.Id;
            UserId = message.UserId;

            if (message.Position!=null){
                Position = new NVector3(message.Position);
            }
            if (message.Rotation != null){
                Rotation = new NVector3(message.Rotation);
            }
            if (message.Scale != null){
                Scale = new NVector3(message.Scale);
            }

            AssetUrl = message.AssetUrl;
            AssetType = message.AssetType;
            Metadata = message.Metadata;
            AuthorityUserId = message.AuthorityUserId;

            var fields = new List<string>();
            foreach (var field in message.UpdatedFields) {
                fields.Add(field);
            }
            UpdatedFields = fields.ToArray();
        }

        public EntityDelta ToProto(){
            EntityDelta d = new EntityDelta{Id=Id, 
                UserId=(UserId==null)?"":UserId, 
                AuthorityUserId=(AuthorityUserId==null)?"":AuthorityUserId,
                AssetUrl=(AssetUrl==null)?"":AssetUrl, 
                AssetType=(AssetType==null)?"":AssetType, 
                Metadata=(Metadata==null)?"":Metadata};
            d.Position = (Position != null) ? Position.ToProto() : new V3{};
            d.Rotation = (Rotation != null) ? Rotation.ToProto() : new V3{};
            d.Scale = (Scale !=null) ? Scale.ToProto(): new V3();
            foreach (var field in UpdatedFields) {
                d.UpdatedFields.Add(field);
            }
            return d;
        }

        public override string ToString()
        {
            var f = "NEntity(Id={0},UserId={1},AuthorityUserId={2},Position={3},Rotation={4}," +
                    "Scale={5}, AssetUrl={6},AssetType={7},Metadata={8},UpdatedFields={9})";
            return String.Format(f, Id, UserId, AuthorityUserId, Position, Rotation,
                    Scale, AssetUrl, AssetType, Metadata, String.Join(", ", UpdatedFields) );
        }
    }
}
