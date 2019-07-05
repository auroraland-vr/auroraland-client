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
using Google.Protobuf;

namespace Nakama
{
    public class NSpaceUsersListMessage : INCollatedMessage<INResultSet<INUser>>
    {
        private Envelope payload;
        public IMessage Payload {
            get {
                return payload;
            }
        }

        private NSpaceUsersListMessage(string spaceId)
        {
            payload = new Envelope {SpaceUsersList = new TSpaceUsersList {
                SpaceId = spaceId
            }};
        }

        public void SetCollationId(string id)
        {
            payload.CollationId = id;
        }

        public override string ToString()
        {
            return String.Format("NSpaceUsersListMessage(SpaceId={0})", payload.SpaceUsersList.SpaceId);
        }

        public static NSpaceUsersListMessage Default(string spaceId)
        {
            return new NSpaceUsersListMessage(spaceId);
        }
    }
}
