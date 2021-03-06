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
using System.Collections.Generic;
using DicomSharp.Dictionary;
using DicomSharp.Utility;

namespace DicomSharp.Net {
    /// <summary> 
    /// Defines association acceptance/rejection behavior.
    /// </summary>
    public class AcceptorPolicy {
        private readonly Hashtable appCtxMap = new Hashtable();
        private readonly Hashtable extNegotiaionMap = new Hashtable();
        private readonly Hashtable policyForCalledAET = new Hashtable();
        private readonly Hashtable policyForCallingAET = new Hashtable();
        private readonly IDictionary<string, PresentationContext> presentationContexts = new Dictionary<string, PresentationContext>();
        private readonly Hashtable roleSelectionMap = new Hashtable();
        private String Vers = Implementation.VersionName;
        private AsyncOpsWindow aow;
        private ArrayList calledAETs;
        private ArrayList callingAETs;
        private String classUID = Implementation.ClassUID;
        private int maxLength = PDataTF.DEF_MAX_PDU_LENGTH;

        /// <summary>
        /// Constructor
        /// </summary>		
        public AcceptorPolicy() {
            PutPresentationContext(UIDs.Verification, new[] {UIDs.ImplicitVRLittleEndian});
        }

        public virtual int MaxPduLength {
            get { return maxLength; }
            set {
                if (value < 0) {
                    throw new ArgumentException("maxLength:" + value);
                }

                maxLength = value;
            }
        }

        public virtual AsyncOpsWindow AsyncOpsWindow {
            get { return aow; }
        }

        public virtual String ClassUID {
            get { return classUID; }
            set { classUID = StringUtils.CheckUID(value); }
        }

        public virtual String VersionName {
            get { return Vers; }
            set { Vers = value != null ? StringUtils.CheckAET(value) : null; }
        }

        public virtual String[] CalledAETs {
            get { return calledAETs != null ? (String[]) calledAETs.ToArray() : null; }
            set { calledAETs = value != null ? new ArrayList(StringUtils.CheckAETs(value)) : null; }
        }

        public virtual String[] CallingAETs {
            get { return callingAETs != null ? (String[]) callingAETs.ToArray() : null; }
            set { callingAETs = value != null ? new ArrayList(StringUtils.CheckAETs(value)) : null; }
        }

        public virtual void setAsyncOpsWindow(int maxOpsInvoked, int maxOpsPerformed) {
            if (maxOpsInvoked == 1 && maxOpsPerformed == 1) {
                aow = null;
            }
            else if (aow == null || aow.MaxOpsInvoked != maxOpsInvoked || aow.MaxOpsPerformed != maxOpsPerformed) {
                aow = new AsyncOpsWindow(maxOpsInvoked, maxOpsPerformed);
            }
        }

        public virtual void PutApplicationContextName(String proposed, String returned) {
            appCtxMap.Add(StringUtils.CheckUID(proposed), StringUtils.CheckUID(returned));
        }

        public virtual bool AddCalledAET(String aet) {
            StringUtils.CheckAET(aet);

            if (calledAETs == null) {
                calledAETs = new ArrayList();
            }

            return (calledAETs.Add(aet) >= 0);
        }

        public virtual void RemoveCalledAET(String aet) {
            if (calledAETs != null) {
                calledAETs.Remove(aet);
            }
        }


        public virtual bool AddCallingAET(String aet) {
            StringUtils.CheckAET(aet);

            if (callingAETs == null) {
                callingAETs = new ArrayList();
            }

            return (callingAETs.Add(aet) >= 0);
        }

        public virtual void RemoveCallingAET(String aet) {
            if (callingAETs != null) {
                callingAETs.Remove(aet);
            }
        }

        public virtual AcceptorPolicy GetPolicyForCallingAET(String aet) {
            return (AcceptorPolicy) policyForCallingAET[aet];
        }

        public virtual void PutPolicyForCallingAET(String aet, AcceptorPolicy policy) {
            PutPolicyForXXXAET(aet, policy, policyForCallingAET);
        }

        public virtual AcceptorPolicy GetPolicyForCalledAET(String aet) {
            return (AcceptorPolicy) policyForCalledAET[aet];
        }

        public virtual void PutPolicyForCalledAET(String aet, AcceptorPolicy policy) {
            PutPolicyForXXXAET(aet, policy, policyForCalledAET);
        }

        private void PutPolicyForXXXAET(String aet, AcceptorPolicy policy, Hashtable map) {
            if (policy != null) {
                map.Add(StringUtils.CheckAET(aet), policy);
            }
            else {
                map.Remove(aet);
            }
        }

        public void PutPresentationContext(String asuid, String[] tsuids) {
            if (tsuids != null) {
                if (!presentationContexts.ContainsKey(asuid)) {
                    presentationContexts.Add(asuid, new PresentationContext(0x020, 1, 0, StringUtils.CheckUID(asuid), StringUtils.CheckUIDs(tsuids)));
                }
            } else {
                presentationContexts.Remove(asuid);
            }
        }

        public virtual PresentationContext GetPresContext(String syntax) {
            return (PresentationContext) presentationContexts[syntax];
        }

        public virtual void PutRoleSelection(String uid, bool scu, bool scp) {
            roleSelectionMap.Add(StringUtils.CheckUID(uid), new RoleSelection(uid, scu, scp));
        }

        public virtual RoleSelection GetRoleSelection(String uid) {
            return (RoleSelection) roleSelectionMap[uid];
        }

        public virtual void RemoveRoleSelection(String uid) {
            roleSelectionMap.Remove(uid);
        }

        public virtual void PutExtNegPolicy(String uid, IExtNegotiator en) {
            if (en != null) {
                extNegotiaionMap.Add(uid, en);
            }
            else {
                extNegotiaionMap.Remove(uid);
            }
        }

        public virtual IExtNegotiator GetExtNegPolicy(String uid) {
            return (IExtNegotiator) extNegotiaionMap[uid];
        }

        public virtual IPdu Negotiate(AAssociateRQ rq) {
            if ((rq.ProtocolVersion & 0x0001) == 0) {
                return new AAssociateRJ(AAssociateRJ.REJECTED_PERMANENT, AAssociateRJ.SERVICE_PROVIDER_ACSE,
                                        AAssociateRJ.PROTOCOL_VERSION_NOT_SUPPORTED);
            }
            String calledAET = rq.ApplicationEntityTitle;
            if (calledAETs != null && !calledAETs.Contains(calledAET)) {
                return new AAssociateRJ(AAssociateRJ.REJECTED_PERMANENT, AAssociateRJ.SERVICE_USER,
                                        AAssociateRJ.CALLED_AE_TITLE_NOT_RECOGNIZED);
            }
            AcceptorPolicy policy1 = GetPolicyForCalledAET(calledAET);
            if (policy1 == null) {
                policy1 = this;
            }

            String callingAET = rq.ApplicationEntityTitle;
            if (policy1.callingAETs != null && !policy1.callingAETs.Contains(callingAET)) {
                return new AAssociateRJ(AAssociateRJ.REJECTED_PERMANENT, AAssociateRJ.SERVICE_USER,
                                        AAssociateRJ.CALLING_AE_TITLE_NOT_RECOGNIZED);
            }
            AcceptorPolicy policy2 = policy1.GetPolicyForCallingAET(callingAET);
            if (policy2 == null) {
                policy2 = policy1;
            }

            return policy2.doNegotiate(rq);
        }

        private IPdu doNegotiate(AAssociateRQ rq) {
            String appCtx = NegotiateAppCtx(rq.ApplicationContext);
            if (appCtx == null) {
                return new AAssociateRJ(AAssociateRJ.REJECTED_PERMANENT, AAssociateRJ.SERVICE_USER,
                                        AAssociateRJ.APPLICATION_CONTEXT_NAME_NOT_SUPPORTED);
            }
            var ac = new AAssociateAC();
            ac.ApplicationContext = appCtx;
            ac.ApplicationEntityTitle = rq.ApplicationEntityTitle;
            ac.Name = rq.Name;
            ac.MaxPduLength = maxLength;
            ac.ClassUID = ClassUID;
            ac.VersionName = Vers;
            ac.AsyncOpsWindow = NegotiateAOW(rq.AsyncOpsWindow);
            NegotiatePresCtx(rq, ac);
            NegotiateRoleSelection(rq, ac);
            NegotiateExt(rq, ac);
            return ac;
        }

        private String NegotiateAppCtx(String proposed) {
            var retval = (String) appCtxMap[proposed];
            if (retval != null) {
                return retval;
            }

            if (UIDs.DICOMApplicationContextName.Equals(proposed)) {
                return proposed;
            }

            return null;
        }

        private void NegotiatePresCtx(AAssociateRQ rq, AAssociateAC ac) {
            for (IEnumerator enu = rq.ListPresContext().GetEnumerator(); enu.MoveNext();) {
                ac.AddPresContext(NegotiatePresCtx((PresentationContext) enu.Current));
            }
        }

        private PresentationContext NegotiatePresCtx(PresentationContext offered) {
            int result = PresentationContext.ABSTRACT_SYNTAX_NOT_SUPPORTED;
            String tsuid = UIDs.ImplicitVRLittleEndian;

            PresentationContext accept = GetPresContext(offered.AbstractSyntaxUID);
            if (accept != null) {
                result = PresentationContext.TRANSFER_SYNTAXES_NOT_SUPPORTED;
                for (IEnumerator enu = accept.TransferSyntaxUIDs.GetEnumerator(); enu.MoveNext();) {
                    tsuid = (String) enu.Current;
                    if (offered.TransferSyntaxUIDs.IndexOf(tsuid) != - 1) {
                        result = PresentationContext.ACCEPTANCE;
                        break;
                    }
                }
            }
            return new PresentationContext(0x021, offered.pcid(), result, null, new[] {tsuid});
        }

        private void NegotiateRoleSelection(AAssociateRQ rq, AAssociateAC ac) {
            for (IEnumerator enu = rq.ListRoleSelections().GetEnumerator(); enu.MoveNext();) {
                ac.AddRoleSelection(NegotiateRoleSelection((RoleSelection) enu.Current));
            }
        }

        private RoleSelection NegotiateRoleSelection(RoleSelection offered) {
            bool scu = offered.IsServiceClassUser;
            bool scp = false;

            RoleSelection accept = GetRoleSelection(offered.SOPClassUID);
            if (accept != null) {
                scu = offered.IsServiceClassUser && accept.IsServiceClassUser;
                scp = offered.IsServiceClassProvider && accept.IsServiceClassProvider;
            }
            return new RoleSelection(offered.SOPClassUID, scu, scp);
        }

        private void NegotiateExt(AAssociateRQ rq, AAssociateAC ac) {
            for (IEnumerator enu = rq.ListExtNegotiations().GetEnumerator(); enu.MoveNext();) {
                var offered = (ExtNegotiation) enu.Current;
                String uid = offered.SOPClassUID;
                IExtNegotiator enp = GetExtNegPolicy(uid);
                if (enp != null) {
                    ac.AddExtNegotiation(new ExtNegotiation(uid, enp.Negotiate(offered.info())));
                }
            }
        }

        private AsyncOpsWindow NegotiateAOW(AsyncOpsWindow offered) {
            if (offered == null) {
                return null;
            }

            if (aow == null) {
                return AsyncOpsWindow.DEFAULT;
            }

            return new AsyncOpsWindow(minAOW(offered.MaxOpsInvoked, aow.MaxOpsInvoked),
                                      minAOW(offered.MaxOpsPerformed, aow.MaxOpsPerformed));
        }

        internal static int minAOW(int a, int b) {
            return a == 0 ? b : b == 0 ? a : Math.Min(a, b);
        }

        public virtual ICollection<PresentationContext> ListPresContext() {
            return new List<PresentationContext>(presentationContexts.Values);
        }

        // Inner classes -------------------------------------------------
    }
}