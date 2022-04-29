/**
 *
 * -- Ported Code --
 * Ported C# version of SimpleLink BLE OAD iOS from Texas Instruments.
 * 
 * Ported SimpleLink Version: 1.0.1
 * 
 * Source URL: http://git.ti.com/simplelink-ble-oad-ios/simplelink-ble-oad-ios/trees/master/ti_oad/Classes
 *
 */

using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LEGODeviceUnitySDK
{
    public class OADSection : IOADSection
    {
        public OADImageHeader Header { [UsedImplicitly] get; set; }
        public OADSegmentInformation SegmentInfo { private get; set; }
        public byte[] ImgData { private get; set; }
        
        private OADBoundaryInformation GetDataAsBoundaryInformation()
        {
            var pinnedPacket = GCHandle.Alloc(ImgData, GCHandleType.Pinned);
            var boundaryInfo = (OADBoundaryInformation)Marshal.PtrToStructure( pinnedPacket.AddrOfPinnedObject(), typeof(OADBoundaryInformation));        
            pinnedPacket.Free();
            
            return boundaryInfo;
        }
        
        private OADContiguousImageInformation GetDataAsContiguousImageInformation()
        {
            var pinnedPacket = GCHandle.Alloc(ImgData, GCHandleType.Pinned);
            var contiguousImgInfo = (OADContiguousImageInformation)Marshal.PtrToStructure( pinnedPacket.AddrOfPinnedObject(), typeof(OADContiguousImageInformation));        
            pinnedPacket.Free();
            
            return contiguousImgInfo;
        }
        
        private OADSecureFWInformation GetDataAsSecurityImageInformation()
        {
            var pinnedPacket = GCHandle.Alloc(ImgData, GCHandleType.Pinned);
            var securityImgInfo = (OADSecureFWInformation)Marshal.PtrToStructure( pinnedPacket.AddrOfPinnedObject(), typeof(OADSecureFWInformation));        
            pinnedPacket.Free();
            
            return securityImgInfo;
        }

        public override string ToString()
        {
            var str = "Segment type: ";
            switch (SegmentInfo.OADSegmentType)
            {
                case OADDefines.SEGMENT_TYPE_BOUNDARY_INFO:
                    str += "Boundary information";
                    break;
                case OADDefines.SEGMENT_TYPE_CONTIGUOUS_INFO:
                    str += "Contiguous information";
                    break;
                default:
                    str += "Unknown";
                    break;
            }
            str += " (0x" + SegmentInfo.OADSegmentType.ToString("X2") + ")\n";
            str += "Wireless Standard:" + OADImageReader.WirelessTechnologyToString(SegmentInfo.OADWirelessTechnology) + " 0x"+ SegmentInfo.OADWirelessTechnology.ToString("X4") + "\n";
            str += "Payload Length: "+SegmentInfo.OADPayloadLength+" (0x"+SegmentInfo.OADPayloadLength.ToString("X8")+")\n";

            if (SegmentInfo.OADSegmentType == OADDefines.SEGMENT_TYPE_BOUNDARY_INFO)
            {
                var boundaryInfo = GetDataAsBoundaryInformation();
                str += "Stack Entry Address: 0x" + boundaryInfo.OADBoundaryStackEntryAddress.ToString("X8") + "\n";
                str += "ICALL_STACK0_ADDR: 0x"+boundaryInfo.OADBoundaryICALL_STACK0_ADDR.ToString("X8")+"\n";
                str += "RAM_START_ADDR: 0x"+boundaryInfo.OADBoundaryRAM_START_ADDR.ToString("X8")+"\n";
                str += "RAM_END_ADDR: 0x"+boundaryInfo.OADBoundaryRAM_END_ADDR.ToString("X8")+"\n";
            }
            else if (SegmentInfo.OADSegmentType == OADDefines.SEGMENT_TYPE_CONTIGUOUS_INFO)
            {
                var contInfo = GetDataAsContiguousImageInformation();
                str += "Entry Address: 0x" + contInfo.OADStackEntryAddress.ToString("X8") + "\n";
            }
            else if (SegmentInfo.OADSegmentType == OADDefines.SEGMENT_TYPE_SECURITY_INFO)
            {
                var secInfo = GetDataAsSecurityImageInformation();
                str += "Security Version: "+ secInfo.OADVersion +" (%@)\n" + (secInfo.OADVersion == 0x01 ? @"ECDSA P-256 Signature" :
                    secInfo.OADVersion == 0x02 ? @"AES 128-CBC Signature" : @"Reserved"); 
                str += "Time-stamp: "+secInfo.OADTimeStamp+" (0x"+secInfo.OADTimeStamp.ToString("X8")+")\n";
                str += "Sign Payload: \n";
                str += "Signer Information : "+ secInfo.OADSignPayload.OADSignerInformation[0] +","+
                secInfo.OADSignPayload.OADSignerInformation[1].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[2].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[3].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[4].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[5].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[6].ToString("X2") +","+
                secInfo.OADSignPayload.OADSignerInformation[7].ToString("X2") + "\n";

                str += "Signature : ";
                for (var ii = 0; ii <  Buffer.ByteLength(secInfo.OADSignPayload.OADSignature); ii++) {
                    str += "0x"+secInfo.OADSignPayload.OADSignature[ii].ToString("X2");
                    if ((ii != 0) && ((ii % 16) == 0)) {
                        str +="\n";
                    }
                }
            }
            
            str += "\n";

            return str;
        }
    }
}