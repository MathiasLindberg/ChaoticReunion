namespace LEGODeviceUnitySDK
{
    public interface ILEGOLEAFGameEngineDelegate : ILEGOServiceDelegate
    {
        //! TODO - when the game engine is defined, this will (most likely) need updating
        void DidUpdateChallenges(LEGOLEAFGameEngine gameEngine, LEGOValue oldChallenges, LEGOValue newChallenges);
        
        void DidUpdateVersions(LEGOLEAFGameEngine gameEngine, LEGOValue oldAssets, LEGOValue newAssets);
        
        void DidUpdateEvents(LEGOLEAFGameEngine gameEngine, LEGOValue oldEvents, LEGOValue newEvents);
    }
}