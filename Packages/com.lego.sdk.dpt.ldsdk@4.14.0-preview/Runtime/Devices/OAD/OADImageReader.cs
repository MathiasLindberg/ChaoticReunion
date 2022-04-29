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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LEGO.Logger;
using UnityEngine;

// ReSharper disable BuiltInTypeReferenceStyle

// ReSharper disable once CheckNamespace
namespace LEGODeviceUnitySDK
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OADSegmentInformation
    {
        public readonly byte OADSegmentType;                             //1
        public readonly ushort OADWirelessTechnology;                    //3
        private readonly byte TOADReserved;                                //4
        public readonly uint OADPayloadLength;                            //8
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OADSignPayload
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] OADSignerInformation;                      //8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public readonly byte[] OADSignature;                              //72
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // ReSharper disable once InconsistentNaming
    public struct OADSecureFWInformation
    {
        private readonly OADSegmentInformation SegmentInfo;                          //8
        public readonly byte OADVersion;                                  //9
        public readonly UInt32 OADTimeStamp;                               //13
        public OADSignPayload OADSignPayload;                            //85
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OADContiguousImageInformation
    {
        private readonly OADSegmentInformation SegmentInfo;                //8
        public readonly UInt32 OADStackEntryAddress;                      //12
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OADBoundaryInformation
    {
        private readonly OADSegmentInformation segmentInfo;                //8
        public readonly UInt32 OADBoundaryStackEntryAddress;              //12
        public readonly UInt32 OADBoundaryICALL_STACK0_ADDR;              //16
        public readonly UInt32 OADBoundaryRAM_START_ADDR;                 //20
        public readonly UInt32 OADBoundaryRAM_END_ADDR;                   //24
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OADImageHeader
    {
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=8)] 
        public readonly byte[] OADImageIdentificationValue;             //8
        public readonly UInt32 OADImageCRC32;                           //12
        public readonly byte OADImageBIMVersion;                        //13
        public readonly byte OADImageHeaderVersion;                     //14
        public readonly UInt16 OADImageWirelessTechnology;              //16
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=4)]
        public readonly byte[] OADImageInformation;                     //20
        public readonly UInt32 OADImageValidation;                      //24
        public readonly UInt32 OADImageLength;                          //28
        public readonly UInt32 OADImageProgramEntryAddress;             //32
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=4)]
        public readonly byte[] OADImageSoftwareVersion;                 //36
        public readonly UInt32 OADImageEndAddress;                      //40
        public readonly UInt16 OADImageHeaderLength;                    //42
        private readonly UInt16 OADImageReservedLongField;               //44 Not in documentation
    }
    

    public class OADImageReader : IOADImageReader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OADImageReader));

        private readonly byte[] imgData;
        private OADImageHeader fileHeader;
        private readonly List<OADSection> sections = new List<OADSection>();

        public OADImageReader(byte[] imgData)
        {
            this.imgData = imgData;

            if (!ValidateImage()) 
            {
                //Handle invalid image
                Logger.Error("Invalid image");
            }
        }

        public OADImageHeader GetHeader()
        {
            return fileHeader;
        }

        public OADSection[] GetSections()
        {
            return sections.ToArray();
        }

        public IOADSection GetSection(int sectionIndex)
        {
            if (sectionIndex > sections.Count - 1) 
                return null;
            
            return sections[sectionIndex];
        }

        public byte[] GetRAWData()
        {
            return imgData;
        }

        public string GetCircuitString()
        {
            return System.Text.Encoding.UTF8.GetString(GetHeader().OADImageIdentificationValue);
        }
        

        public bool ValidateImage()
        {
            var imageHeaderSize = Marshal.SizeOf(typeof(OADImageHeader));
            var segmentInfoSize = Marshal.SizeOf(typeof(OADSegmentInformation));
            var boundaryInfoSize = Marshal.SizeOf(typeof(OADBoundaryInformation));
            var range = new Range {Location = 0, Length = imageHeaderSize + segmentInfoSize};
            
            if (imgData == null || imgData.Length < range.Length)
            {
                return false;
            }

            range.Length = imageHeaderSize;
            fileHeader = ImageHeaderFromBytes(imgData);

            while (range.Location + range.Length < imgData.Length)
            {
                var section = new OADSection {Header = fileHeader};

                //Start to parse the rest of the header !
                range.Location += range.Length;
                range.Length = segmentInfoSize;
                
                var tmpSegment = ImageSegmentInformationFromBytes(GetBytes(imgData, range));

                switch (tmpSegment.OADSegmentType)
                {
                    case OADDefines.SEGMENT_TYPE_BOUNDARY_INFO:
                    {
                        if(tmpSegment.OADPayloadLength != boundaryInfoSize)
                            Logger.Warn("Boundary info segment not of right size");
                        section.SegmentInfo = tmpSegment;
                        range.Location += range.Length;
                        range.Length = (int) tmpSegment.OADPayloadLength - segmentInfoSize;
                        if (range.Location + range.Length > imgData.Length)
                        {
                            Logger.Warn("The segment length field was longer than the actual data !");
                            return false;
                        }

                        section.ImgData = GetBytes(imgData, range);
                        sections.Add(section);
                        break;
                    }
                    
                    case OADDefines.SEGMENT_TYPE_CONTIGUOUS_INFO:
                    {
                        var contImgInfoSize = Marshal.SizeOf(typeof(OADContiguousImageInformation));
                        if(tmpSegment.OADPayloadLength != contImgInfoSize)
                            Logger.Warn("Contiguous info segment not of right size. Payload length: " + tmpSegment.OADPayloadLength + " sizeOf OADContiguousImageInformation: " + contImgInfoSize);
                        
                        section.SegmentInfo = tmpSegment;
                        range.Location += range.Length;
                        range.Length = (int) tmpSegment.OADPayloadLength - segmentInfoSize;
                        if (range.Length > imgData.Length - range.Location)
                        {
                            Logger.Warn("Not contiguous image info, length too long !");
                            return false;
                        }

                        section.ImgData = GetBytes(imgData, range);
                        sections.Add(section);
                        break;
                    }
                    
                    case OADDefines.SEGMENT_TYPE_SECURITY_INFO:
                    {
                        section.SegmentInfo = tmpSegment;
                        range.Location += range.Length;
                        range.Length = (int)tmpSegment.OADPayloadLength - segmentInfoSize;
                        if (range.Length > imgData.Length - range.Location)
                        {
                            Logger.Warn("Security info out of bounds, length too long !");
                            return false;
                        }

                        section.ImgData = GetBytes(imgData, range);
                        sections.Add(section);
                        break;
                    }
                    default:
                        Logger.Warn("Unknown OAD segment type ! Got " + tmpSegment.OADSegmentType + " but expected value in range 0-2");
                        break;
                }
            }

            Logger.Debug(this);
            Logger.Debug("Total Sections in file : " + sections.Count);
            var ii = 0;
            foreach (var sect in sections)
            {
                Logger.Debug("Section " + ii + "\n" + sect);
                ii++;
            }
           
            return true;
        }

        private byte[] GetBytes(byte[] src, Range range)
        {
            if (range.Location + range.Length > src.Length)
            {
                Logger.Error("Trying to get bytes of range");
                return new byte[0];
            } 
                
            return src.Skip(range.Location).Take(range.Length).ToArray();
        }
        
        private struct Range
        {
            public int Location;
            public int Length;
        }

        private OADSegmentInformation ImageSegmentInformationFromBytes(byte[] data)
        {
            var pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
            var msg = (OADSegmentInformation)Marshal.PtrToStructure( pinnedPacket.AddrOfPinnedObject(), typeof(OADSegmentInformation));        
            pinnedPacket.Free();
            return msg;
        }
        
        private OADImageHeader ImageHeaderFromBytes(byte[] data)
        {
            var pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
            var msg = (OADImageHeader)Marshal.PtrToStructure( pinnedPacket.AddrOfPinnedObject(), typeof(OADImageHeader));        
            pinnedPacket.Free();
            return msg;
        }
        
        public override string ToString()
        {
            var str = "Image info:\n";
            str += "Image identification: " +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[0]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[1]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[2]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[3]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[4]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[5]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[6]) +
               Convert.ToChar(fileHeader.OADImageIdentificationValue[7]) + "\n";
            
            str += "Image CRC32: 0x" + fileHeader.OADImageCRC32.ToString("X8") + "\n";
            str += "Image BIM Version: " + fileHeader.OADImageBIMVersion + "\n";
            str += "Image Header Version: " + fileHeader.OADImageHeaderVersion + "\n";
            str += "Image Wireless Standard: " + WirelessTechnologyToString(fileHeader.OADImageWirelessTechnology) + "\n";
            str += "Image Information:\n" + ImageInfoToString(fileHeader.OADImageInformation) + "\n";
            str += "Image Validation: 0x" + fileHeader.OADImageValidation.ToString("X8") + "\n";
            str += "Image Length: "+ fileHeader.OADImageLength + "(0x" + fileHeader.OADImageLength.ToString("X8") + ") Bytes\n";
            str += "Image Program Entry Address: 0x" + fileHeader.OADImageProgramEntryAddress.ToString("X8") + "\n";
            str += "Image Software Version: App:" +(fileHeader.OADImageSoftwareVersion[3] & 0x0F) +
                ((fileHeader.OADImageSoftwareVersion[3] & 0xF0) >> 4) + "." +
                (fileHeader.OADImageSoftwareVersion[2] & 0x0F) +
                ((fileHeader.OADImageSoftwareVersion[2] & 0xF0) >> 4) + " Stack: " +
                (fileHeader.OADImageSoftwareVersion[1] & 0x0F) +
                ((fileHeader.OADImageSoftwareVersion[1] & 0xF0) >> 4) + "." +
                (fileHeader.OADImageSoftwareVersion[0] & 0x0F) + 
                ((fileHeader.OADImageSoftwareVersion[0] & 0xF0) >> 4) + "\n";
            str += "Image End Address: 0x" + fileHeader.OADImageEndAddress.ToString("X8") + "\n";
            str += "Image Header Length: " + fileHeader.OADImageHeaderLength + "(0x" + fileHeader.OADImageHeaderLength.ToString("X8") + ")\n";
            
            return str;
        }

        private string ImageInfoToString(byte[] imgInfo)
        {
            string str = "  - Image Copy Status: ";
            switch (imgInfo[0])
            {
                case OADDefines.TOAD_IMAGE_COPY_STATUS_NO_ACTION_NEEDED:
                    str += "Default status, no action needed";
                    break;
                case OADDefines.TOAD_IMAGE_COPY_STATUS_IMAGE_TO_BE_COPIED:
                    str += "Image to be copied to on-chip flash at location indicated in the image header";
                    break;
                case OADDefines.TOAD_IMAGE_COPY_STATUS_IMAGE_COPIED:
                    str += "Image copied";
                    break;
                default:
                    str += "Unknown, byte value: " + imgInfo;
                    break;
            }

            str += "\n";

            str += "  - Image CRC Status: ";
            switch (imgInfo[1])
            {
                case OADDefines.TOAD_IMAGE_CRC_STATUS_INVALID:
                    str += "CRC Invalid (0x00)";
                    break;
                case OADDefines.TOAD_IMAGE_CRC_STATUS_VALID:
                    str += "CRC Valid (0x01)";
                    break;
                case OADDefines.TOAD_IMAGE_CRC_STATUS_NOT_CALCULATED_YET:
                    str += "CRC Not Calculated Yet (0x03)";
                    break;
                default:
                    str += "Unknown, byte value: " + imgInfo[1];
                    break;
            }

            str += "\n";

            str += "  - Image Type: ";
            switch (imgInfo[2])
            {
                case OADDefines.TOAD_IMAGE_TYPE_PERSISTENT_APP:
                    str += "Persistent Application (0x00)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_APP:
                    str += "Application (0x01)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_STACK:
                    str += "Stack (0x02)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_APP_STACK_MERGED:
                    str += "App + Stack Merged (0x03)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_NETWORK_PROC:
                    str += "Network Processor (0x04)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_BLE_FACTORY_IMAGE:
                    str += "Bluetooth Low Energy Factory Image (0x05)";
                    break;
                case OADDefines.TOAD_IMAGE_TYPE_BIM:
                    str += "Boot Loader Image (0x06)";
                    break;
                default:
                    str += "Unknown, byte value: " + imgInfo[2];
                    break;
            }
            str += "\n  - Image Number: " + imgInfo[3].ToString("X2") + "\n";

            return str;

        }

        public static string WirelessTechnologyToString(ushort wirelessTech)
        {
            var str = "";
            
            if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_BLE) == 0x00) {
                str += " [Bluetooth Low Energy]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_802_15_4_SUB_ONE) == 0x00) {
                str += " [802.15.4 Sub-One]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_802_15_4_2_POINT_4) == 0x00) {
                str += " [802.15.4 2.4GHz]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_ZIGBEE) == 0x00) {
                str += " [ZigBee]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_RF4CE) == 0x00) {
                str += " [RF4CE]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_THREAD) == 0x00) {
                str += " [Thread]";
            }
            else if ((wirelessTech & OADDefines.TOAD_WIRELESS_STD_EASY_LINK) == 0x00) {
                str += " [Easy Link]";
            }
            return str;
        }
        
        private string VerificationStatusToString(byte verificationStatus)
        {
            switch (verificationStatus)
            {
                case OADDefines.TOAD_IMAGE_VERIFICATION_STATUS_STD:
                return "Default";
                case OADDefines.TOAD_IMAGE_VERIFICATION_STATUS_FAILED:
                return "Failed";
                case OADDefines.TOAD_IMAGE_VERIFICATION_STATUS_SUCCESS:
                return "Success";
                default:
                return "Unknown";
            }
        }
    }
}