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
using System.Reflection;
using System.Collections.Generic;

namespace Nakama
{
    public class NEntity : INEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string SpaceId { get; set; }
        public INVector3 Rotation { get; set; }
        public INVector3 Position { get; set; }
        public INVector3 Scale { get; set; }
        public string AssetUrl { get; set; }
        public string AssetType { get; set; }
        public string Metadata { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public bool Online { get; set; }
        public string AuthorityUserId { get; set; }
        public int Version { get; set; }

        public string GroupId { get; set; }
        public long SegmentId { get; set; }
        public bool IsSegment { get; set; }

        public NEntity(){}

        internal NEntity(Entity message)
        {
            Id = message.Id;
            UserId = message.UserId;
            DisplayName = message.DisplayName;
            SpaceId = message.SpaceId;

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
            CreatedAt = message.CreatedAt;
            UpdatedAt = message.UpdatedAt;
            Online = message.Online;
            AuthorityUserId = message.AuthorityUserId;
            Version = message.Version;
            GroupId = message.GroupId;
            SegmentId = message.SegmentId;
            IsSegment = message.IsSegment;
        }

        public Entity ToProto(){
            Entity e = new Entity{Id=Id, UserId=UserId, DisplayName=DisplayName, SpaceId=SpaceId, AssetUrl=AssetUrl, AssetType=AssetType, CreatedAt=CreatedAt, UpdatedAt=UpdatedAt, Metadata=Metadata, Online=Online, AuthorityUserId=AuthorityUserId, Version=Version, GroupId=GroupId, SegmentId=SegmentId, IsSegment=IsSegment};
            e.Position = Position.ToProto();
            e.Rotation = Rotation.ToProto();
            e.Scale = Scale.ToProto();
            return e;
        }

        public bool Equals(INEntity another){
            PropertyInfo[] properties = this.GetType().GetProperties ();
            for (var i =0; i<properties.Length; i++) {
                var prop = properties [i];
                var propName = properties [i].Name;

                var va = prop.GetValue (this, null);
                var vb = prop.GetValue (another, null);

                if (Convert.ToString(va) != Convert.ToString(vb)) {
                    return true;
                } 
            }
            return false;
        }
        
        public INEntityDelta GetDelta(INEntity another) {
            var delta = new NEntityDelta ();

            PropertyInfo[] properties = this.GetType().GetProperties ();

            List<string> updatedFields = new List<string>();
            for (var i =0; i<properties.Length; i++) {
                var prop = properties [i];
                var propName = properties [i].Name;

                var va = prop.GetValue (this, null);
                var vb = prop.GetValue (another, null);

                var deltaProperty = delta.GetType ().GetProperty (propName);

                if (deltaProperty != null) {
                    if (Convert.ToString(va) != Convert.ToString(vb)) {
                        deltaProperty.SetValue (delta, vb, null);
                        updatedFields.Add (propName);
                    } else {
                        // deltaProperty.SetValue (delta, va, null);  
                    }

                }
            }
            delta.Id = another.Id;
            delta.UpdatedFields = updatedFields.ToArray();
            return delta;
        }

        public INEntity Clone(){
            return new NEntity(this.ToProto());
        }

        public override string ToString()
        {
            var f = "NEntity(Id={0}, DisplayName={1}, AssetType={2}, AssetUrl={3}, Version={4}, Position={5},Rotation={6}, UserId={7}, AuthorityUserId={8}," +
                    "Metadata={9}, Scale={10}, SpaceId={11}, CreatedAt={12}, UpdatedAt={13},Online={14}, GroupId={15}, SegmentId={16}, IsSegment={17})";
            return String.Format(f, Id, DisplayName, AssetType, AssetUrl, Version, Position, Rotation, UserId, AuthorityUserId,
                   Metadata, Scale, SpaceId, CreatedAt, UpdatedAt, Online, GroupId, SegmentId, IsSegment );
        }
    }
}
