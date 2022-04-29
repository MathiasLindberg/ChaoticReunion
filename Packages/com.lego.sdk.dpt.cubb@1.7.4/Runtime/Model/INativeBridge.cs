namespace CoreUnityBleBridge.Model
{
    internal interface INativeBridge
    {
        IUnityToNative ToNative { get; }
        
        void Update();
        
        void Dispose();
    }
}