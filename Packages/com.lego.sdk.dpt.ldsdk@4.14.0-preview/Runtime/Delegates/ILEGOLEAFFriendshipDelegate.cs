namespace LEGODeviceUnitySDK
{
    public interface ILEGOLEAFFriendshipDelegate : ILEGOServiceDelegate
    {
        void DidUpdateFriendshipCode(LEGOLEAFFriendship friendship, LEGOValue oldFriendshipCode, LEGOValue newFriendshipCode);

        void DidUpdateEvents(LEGOLEAFFriendship friendship, LEGOValue oldEvents, LEGOValue newEvents);
    }
}