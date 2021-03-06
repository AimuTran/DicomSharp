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
using DicomSharp.Data;

namespace DicomSharp.Net {
    /// <summary>
    /// </summary>
    public sealed class DimseWriter {
        private readonly Fsm fsm;
        private bool cmd;
        private PDataTF pDataTF;
        private int pcid;

        /// <summary>
        /// Creates a new instance of PDataTFWriteAdapter 
        /// </summary>
        public DimseWriter(Fsm fsm) {
            this.fsm = fsm;
        }

        public void Write(IDimse dimse) {
            lock (this) {
                pcid = dimse.PresentationContextId();
                String tsUID = fsm.GetAcceptedTransferSyntaxUID(pcid);
                if (tsUID == null) {
                    throw new SystemException();
                }
                (dimse).TransferSyntaxUniqueId = tsUID;
                fsm.FireWrite(dimse);
                if (pDataTF == null) {
                    pDataTF = new PDataTF(fsm.WriteMaxLength);
                }
                pDataTF.OpenPDV(pcid, cmd = true);
                Stream outs = new PDataTFOutputStream(this);
                IDicomCommand c = dimse.DicomCommand;
                try {
                    c.Write(outs);
                }
                finally {
                    outs.Close();
                }
                if (c.HasDataset()) {
                    pDataTF.OpenPDV(pcid, cmd = false);
                    outs = new PDataTFOutputStream(this);
                    try {
                        dimse.WriteTo(outs, tsUID);
                    }
                    finally {
                        outs.Close();
                    }
                }
                FlushPDataTF();
            }
        }

        public void FlushPDataTF() {
            bool open = pDataTF.IsOpenPDV();
            if (open) {
                pDataTF.ClosePDV(false);
            }
            if (!pDataTF.IsEmpty()) {
                fsm.Write(pDataTF);
            }
            pDataTF.Clear();
            //        pDataTF = new PDataTF(fsm.GetMaxLength());
            if (open) {
                pDataTF.OpenPDV(pcid, cmd);
            }
        }

        private void CloseStream() {
            pDataTF.ClosePDV(true);
            if (!cmd) {
                FlushPDataTF();
            }
        }

        #region Nested type: PDataTFOutputStream

        /// <summary>
        /// PDataTFOutputStream
        /// </summary>
        private class PDataTFOutputStream : MemoryStream {
            private readonly DimseWriter writer;

            public PDataTFOutputStream(DimseWriter writer) {
                this.writer = writer;
            }

            public override void WriteByte(byte b) {
                if (writer.pDataTF.Free() == 0) {
                    writer.FlushPDataTF();
                }
                writer.pDataTF.Write(b);
            }

            public override void Write(byte[] b, int off, int len) {
                if (len == 0) {
                    return;
                }
                int n = 0;
                for (;;) {
                    int c = Math.Min(writer.pDataTF.Free(), len - n);
                    writer.pDataTF.Write(b, off + n, c);
                    n += c;
                    if (n == len) {
                        return;
                    }
                    writer.FlushPDataTF();
                }
            }

            public override void Close() {
                writer.CloseStream();
            }
        }

        #endregion
    }
}