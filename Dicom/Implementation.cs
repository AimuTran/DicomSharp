#region Copyright

// 
// This library is based on Dicom# see http://sourceforge.net/projects/dicom-cs/
// Copyright (c) 2002 by TIANI MEDGRAPH AG. All rights reserved.
//
// Modifications Copyright (C) 2011 Nathan Dauber. All rights reserved.
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

namespace Dicom {
    public class Implementation {
        public static Hashtable rb = new Hashtable();

        static Implementation() {
            rb.Add("dicomcs.ImplementationClassUID", "1.2.40.1.6.8.168");
            rb.Add("dicomcs.ImplementationVersionName", "dicomcs20021018");
        }

        public static String ClassUID {
            get { return (String) rb["dicomcs.ImplementationClassUID"]; }
        }

        public static String VersionName {
            get { return (String) rb["dicomcs.ImplementationVersionName"]; }
        }
    }
}