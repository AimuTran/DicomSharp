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
using System.Collections;
using DicomSharp.Dictionary;
using DicomSharp.Utility;

namespace DicomSharp.Net {
    /// <summary>
    /// 
    /// </summary>
    public class DcmServiceRegistry : Hashtable {
        public DcmServiceRegistry() {
            Add(UIDs.Verification, DcmServiceBase.VERIFICATION_SCP);
        }

        public virtual bool Bind(String uid, IDcmService service) {
            if (service == null) {
                throw new NullReferenceException();
            }

            if (Contains(StringUtils.CheckUID(uid))) {
                return false;
            }

            Add(uid, service);
            return true;
        }

        public virtual void UnBind(String uid) {
            Remove(uid);
        }

        public virtual IDcmService Lookup(String uid) {
            var retval = (IDcmService) this[StringUtils.CheckUID(uid)];
            return retval != null ? retval : DcmServiceBase.NO_SUCH_SOP_CLASS_SCP;
        }
    }
}