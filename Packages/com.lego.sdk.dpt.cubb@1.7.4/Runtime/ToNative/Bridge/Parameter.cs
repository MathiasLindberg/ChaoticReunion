using System;
using System.Text;

namespace CoreUnityBleBridge.ToNative.Bridge
{
        internal abstract class Parameter
        {
            public abstract string Encode();

            public static IDParameter ID(string v)
            {
                return new IDParameter(v);
            }

            public static StringParameter String(string v)
            {
                return new StringParameter(v);
            }

            public static BytesParameter Bytes(byte[] v)
            {
                return new BytesParameter(v);
            }

            public static BoolParameter Bool(bool v)
            {
                return new BoolParameter(v);
            }

            public static IntParameter Int(int v)
            {
                return new IntParameter(v);
            }

            /// Plain ASCII string guaranteed to not contain special characters.
            public class IDParameter : Parameter
            {
                private readonly string v;

                public IDParameter(string v)
                {
                    this.v = v;
                }

                public override string Encode()
                {
                    return v;
                }
            }

            /// Any string.
            public class StringParameter : Parameter
            {
                private readonly string v;

                public StringParameter(string v)
                {
                    this.v = v;
                }

                public override string Encode()
                {
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(v));
                }
            }

            public class BytesParameter : Parameter
            {
                private readonly byte[] v;

                public BytesParameter(byte[] v)
                {
                    this.v = v;
                }

                public override string Encode()
                {
                    return Convert.ToBase64String(v);
                }
            }

            public class BoolParameter : Parameter
            {
                private readonly bool v;

                public BoolParameter(bool v)
                {
                    this.v = v;
                }

                public override string Encode()
                {
                    return v ? "T" : "F";
                }
            }

            public class IntParameter : Parameter
            {
                private readonly int v;

                public IntParameter(int v)
                {
                    this.v = v;
                }

                public override string Encode()
                {
                    return v.ToString();
                }
            }
        }
}