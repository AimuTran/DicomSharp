#region Copyright

// 
// This library is based on dcm4che see http://www.sourceforge.net/projects/dcm4che
// Copyright (c) 2002 by TIANI MEDGRAPH AG. All rights reserved.
//
// Modifications Copyright (C) 2002 Fang Yang. All rights reserved.
// 
// This file is part of dicomcs, see http://www.sourceforge.net/projects/dicom-cs
//
// This library is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.                                 
// 
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
// Fang Yang (yangfang@email.com)
//

#endregion

using Dicom.Net;

namespace Dicom.Server {
    /// <summary>
    /// Create servers, i.e., active service/object
    /// </summary>
    public class ServerFactory {
        private static readonly ServerFactory s_instance = new ServerFactory();

        private ServerFactory() {}

        public static ServerFactory Instance {
            get { return s_instance; }
        }

        public virtual Server newServer(Server.HandlerI handler) {
            return new Server(handler);
        }

        public virtual DcmHandlerI newDcmHandler(AcceptorPolicy policy, DcmServiceRegistry services) {
            return new DcmHandler(policy, services);
        }
    }
}