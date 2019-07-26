namespace Wayfarer.Core
{
    public enum GameContext
    {
        Null,
        Loading,
        
        MainMenu,
        PauseMenu,
        PausePopup,
        
        Level,
        LevelMap,
        LevelDialogue,
        LevelTrading,
        LevelBattle,
        
        War,
        WarDialogue,
        
        WorldMap,
        WorldMapDialogue,
        
        CutsceneAnimation,
        CutsceneAnimationWithQuickEvents,
        CutsceneBookAdventure,
        
        CharacterPage,
        InventoryPage,
        AbilityPage,
        ClassPage,
        JournalPage,
        ArchivePage,
    }
}