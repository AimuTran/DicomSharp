#region Copyright

// 
// This library is based on Dicom# see http://sourceforge.net/projects/dicom-cs/
// Copyright (C) 2002 Fang Yang. All rights reserved.
// That library is based on dcm4che see http://www.sourceforge.net/projects/dcm4che
// Copyright (c) 2002 by TIANI MEDGRAPH AG. All rights reserved.
//
// Modifications Copyright (C) 2012 Nathan Dauber. All rights reserved.
// 
// This file is part of dicomSharp, see https://github.com/KnownSubset/DicomSharp
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
// Nathan Dauber (nathan.dauber@gmail.com)
//

#endregion

using System;
using DicomSharp.Data;
using DicomSharp.Dictionary;

namespace DicomSharp.Net {
    /// <summary> 
    /// </summary>
    public class DcmServiceException : Exception {
        private readonly int status;
        private int actionTypeID = - 1;
        private int errorID = - 1;
        private int eventTypeID = - 1;

        public DcmServiceException(int status) {
            this.status = status;
        }

        public DcmServiceException(int status, String msg) : base(msg) {
            this.status = status;
        }

        public DcmServiceException(int status, String msg, Exception cause) : base(msg, cause) {
            this.status = status;
        }

        public DcmServiceException(int status, Exception cause) : base("", cause) {
            this.status = status;
        }

        public virtual int Status {
            get { return status; }
        }

        public virtual int ErrorID {
            get { return errorID; }
            set { errorID = value; }
        }

        public virtual int EventTypeID {
            get { return eventTypeID; }
            set { eventTypeID = value; }
        }

        public virtual int ActionTypeID {
            get { return actionTypeID; }
            set { actionTypeID = value; }
        }

        public virtual void WriteTo(IDicomCommand cmd) {
            cmd.PutUS(Tags.Status, status);
            string msg = Message;
            if (!string.IsNullOrEmpty(msg)) {
                cmd.PutLO(Tags.ErrorComment, msg.Length > 64 ? msg.Substring(0, (64) - (0)) : msg);
            }
            if (errorID >= 0) {
                cmd.PutUS(Tags.ErrorID, errorID);
            }
            if (actionTypeID >= 0) {
                cmd.PutUS(Tags.ActionTypeID, actionTypeID);
            }
            if (eventTypeID >= 0) {
                cmd.PutUS(Tags.EventTypeID, eventTypeID);
            }
        }
    }
}