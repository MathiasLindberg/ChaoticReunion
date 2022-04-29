using System.Text;


namespace CoreUnityBleBridge.ToNative.Bridge
{
    internal interface IUnityToNativeBridge
    {
        void SendToNative(string messageName, params Parameter[] parameters);
    }
    
    internal abstract class AbstractUnityToNativeBridge: IUnityToNativeBridge 
    {
        public void SendToNative(string messageName, params Parameter[] parameters)
        {
            SendMessageToNative(CreateMessage(messageName, parameters));
        }

        protected abstract void SendMessageToNative(string message);
        
        private static string CreateMessage(string messageName, params Parameter[] parameters)
        {
            var sb = new StringBuilder(messageName);
            foreach (var p in parameters)
            {
                sb.Append('|');
                sb.Append(p.Encode());
            }
            return sb.ToString();            
        }
    }
}

