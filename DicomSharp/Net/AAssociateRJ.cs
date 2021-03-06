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
using System.IO;
using System.Text;
using DicomSharp.Utility;

namespace DicomSharp.Net {
    /// <summary>
    /// </summary>
    public class AAssociateRJ : IPdu {
        public const int REJECTED_PERMANENT = 1;
        public const int REJECTED_TRANSIENT = 2;
        public const int SERVICE_USER = 1;
        public const int SERVICE_PROVIDER_ACSE = 2;
        public const int SERVICE_PROVIDER_PRES = 3;
        public const int NO_REASON_GIVEN = 1;
        public const int APPLICATION_CONTEXT_NAME_NOT_SUPPORTED = 2;
        public const int CALLING_AE_TITLE_NOT_RECOGNIZED = 3;
        public const int CALLED_AE_TITLE_NOT_RECOGNIZED = 7;
        public const int PROTOCOL_VERSION_NOT_SUPPORTED = 2;
        public const int TEMPORARY_CONGESTION = 1;
        public const int LOCAL_LIMIT_EXCEEDED = 2;

        private readonly byte[] buf;

        public AAssociateRJ(byte[] buf) {
            this.buf = buf;
        }

        internal AAssociateRJ(int result, int source, int reason) {
            buf = new byte[] {3, 0, 0, 0, 0, 4, 0, (byte) result, (byte) source, (byte) reason};
        }

        #region IPdu Members

        public void WriteTo(Stream outs) {
            outs.Write(buf, 0, buf.Length);
            StringUtils.dumpBytes("AAssociateRJ", buf, 0, buf.Length);
        }

        public String ToString(bool verbose) {
            return ToString();
        }

        #endregion

        internal static AAssociateRJ Parse(UnparsedPdu raw) {
            if (raw.Length() != 4) {
                throw new PduException("Illegal A-ASSOCIATE-RJ " + raw, new AAbort(AAbort.SERVICE_PROVIDER, AAbort.INVALID_PDU_PARAMETER_VALUE));
            }
            return new AAssociateRJ(raw.Buffer());
        }

        /// <summary>
        /// Returns Result field value.
        /// </summary>
        /// <returns>
        /// Result field value. 
        /// </returns>
        public int Result() {
            return buf[7] & 0xff;
        }

        /// <summary>
        /// Returns Source field value.
        /// </summary>
        /// <returns>
        /// Source field value. 
        /// </returns>
        public int Source() {
            return buf[8] & 0xff;
        }

        /// <summary>
        /// Returns Reason field value.
        /// </summary>
        /// <returns>
        /// Reason field value. 
        /// </returns>
        public int Reason() {
            return buf[9] & 0xff;
        }

        public override String ToString() {
            return ToStringBuffer(new StringBuilder()).ToString();
        }

        internal StringBuilder ToStringBuffer(StringBuilder sb) {
            return
                sb.Append("A-ASSOCIATE-RJ\n\tresult=").Append(ResultAsString()).Append("\n\tsource=").Append(
                    SourceAsString()).Append("\n\treason=").Append(ReasonAsString());
        }

        private String ResultAsString() {
            switch (Result()) {
                case REJECTED_PERMANENT:
                    return "1 - rejected-permanent";

                case REJECTED_TRANSIENT:
                    return "2 - rejected-transient";

                default:
                    return Result().ToString();
            }
        }

        private String SourceAsString() {
            switch (Source()) {
                case SERVICE_USER:
                    return "1 - service-user";

                case SERVICE_PROVIDER_ACSE:
                    return "2 - service-provider (ACSE)";

                case SERVICE_PROVIDER_PRES:
                    return "3 - service-provider (Presentation)";

                default:
                    return Source().ToString();
            }
        }

        public String ReasonAsString() {
            switch (Source()) {
                case SERVICE_USER:
                    switch (Reason()) {
                        case NO_REASON_GIVEN:
                            return "1 - no-reason-given";

                        case APPLICATION_CONTEXT_NAME_NOT_SUPPORTED:
                            return "2 - application-context-name-not-supported";

                        case CALLING_AE_TITLE_NOT_RECOGNIZED:
                            return "3 - calling-AE-title-not-recognized";

                        case CALLED_AE_TITLE_NOT_RECOGNIZED:
                            return "7 - called-AE-title-not-recognized";
                    }
                    goto case SERVICE_PROVIDER_ACSE;

                case SERVICE_PROVIDER_ACSE:
                    switch (Reason()) {
                        case NO_REASON_GIVEN:
                            return "1 - no-reason-given";

                        case PROTOCOL_VERSION_NOT_SUPPORTED:
                            return "2 - protocol-version-not-supported";
                    }
                    goto case SERVICE_PROVIDER_PRES;

                case SERVICE_PROVIDER_PRES:
                    switch (Reason()) {
                        case TEMPORARY_CONGESTION:
                            return "1 - temporary-congestion";

                        case LOCAL_LIMIT_EXCEEDED:
                            return "2 - local-limit-exceeded";
                    }
                    break;
            }
            return Reason().ToString();
        }
    }
}