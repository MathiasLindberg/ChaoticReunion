using System;
using System.Collections.Generic;
using System.IO;
using dk.lego.devicesdk.bluetooth.V3.messages;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
    public abstract class ICommand
    {
        internal abstract LegoMessage AsMessage(uint hubID, uint portID);
    }

    public abstract class LEGOServiceCommand : ICommand
    {
        public bool ExecuteImmediately = true;
        public bool RequestFeedback = true;

        protected const byte MOTOR_POWER_DRIFT = 0x00;
        protected const byte MOTOR_POWER_BRAKE = 0x7F;

        internal override LegoMessage AsMessage(uint hubID, uint portID)
        {
            const byte REQUEST_FEEDBACK = 0x01;
            const byte EXECUTE_IMMEDIATELY = 0x10;
            byte startupAndCompletion = (byte)(REQUEST_FEEDBACK | (ExecuteImmediately ? EXECUTE_IMMEDIATELY : (byte)0));
            var payload = MakeCommandPayload();
            return new MessagePortOutputCommand(hubID, portID, startupAndCompletion, payload.subCommand, payload.parameterPayload);
        }

        protected abstract CommandPayload MakeCommandPayload();

        public bool MayBeReplaced { get; set; }
        protected LEGOServiceCommand()
        {
            MayBeReplaced = true; //can not do public bool MayBeReplaced { get; set; } = true until C# 6
        }

        public abstract ICollection<IOType> SupportedIOTypes { get; } 

        #region Handling of common value types
        protected byte EncodePower(int v)
        {
            return (byte)Mathf.Clamp(v, 0, 100);
        }

        protected byte EncodeSignedPower(int v)
        {
            return (byte)Mathf.Clamp(v, -100, 100);
        }

        protected byte EncodeSignedSpeed(int v)
        {
            return (byte)Mathf.Clamp(v, -100, 100);
        }

        protected UInt16 EncodeDuration(int v, UInt16 max=UInt16.MaxValue) {
            return (UInt16)Mathf.Clamp(v, 0, max);
        }

        protected UInt32 EncodeUnsignedDegrees(int v) {
            return (UInt32)Mathf.Clamp(v, 0, 10000000);
        }

        protected Int32 EncodePosition(int v) {
            return v;
        }

        protected byte EncodeEndState(MotorWithTachoEndState v) {
            return (byte)v;
        }

        protected byte EncodeProfileConfig(MotorWithTachoProfileConfiguration v) {
            return (byte)v;
        }
        #endregion

        protected struct CommandPayload {
            public PortOutputCommandSubCommandTypeEnum subCommand;
            public byte[] parameterPayload;

            public CommandPayload(PortOutputCommandSubCommandTypeEnum subCommand, byte[] parameterPayload)
            {
                this.subCommand = subCommand;
                this.parameterPayload = parameterPayload;
            }

        }

        protected class PayloadBuilder {
            private readonly BinaryWriter buffer;
            public PayloadBuilder(int size) {
                this.buffer = new BinaryWriter(new MemoryStream(size));
            }

            public PayloadBuilder Put(byte v) {
                buffer.Write(v);
                return this;
            }

            public PayloadBuilder PutUInt16(UInt16 v) {
                buffer.Write(v);
                return this;
            }

            public PayloadBuilder PutInt16(Int16 v) {
                buffer.Write(v);
                return this;
            }

            public PayloadBuilder PutUInt32(UInt32 v) {
                buffer.Write(v);
                return this;
            }

            public PayloadBuilder PutInt32(Int32 v) {
                buffer.Write(v);
                return this;
            }

            public byte[] GetBytes() {
                return ((MemoryStream)buffer.BaseStream).ToArray();
            }
        }
    }
}
