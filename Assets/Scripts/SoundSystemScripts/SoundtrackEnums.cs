namespace SoundSystemScripts
{
    public enum TypeOfSoundtrack
    {
        OST=0, SFX=1
    }
    
    public enum TypeOfSFXByNumberOfSounds
    {
        Single=0, Multiple=1
    }

    public enum TypeOfSFXByItsNature //Add only to end
    {
        None = 0, UI_Click_long = 1, Concrete_Step_Run = 2, UI_Click_short = 3, Concrete_Slide = 4, Concrete_Land = 5,
        Heavy_armor_foley = 6, Light_armor_foley = 7, Archer_armor_foley = 8, Sword_swoosh = 9, Dagger_swoosh = 10, Blood_spaltter = 11, Male_grunt = 12,
        Armor_hit = 13, Without_Armor_hit = 14, Female_grunt = 15,
    }
    
    public enum TypeOfOSTByItsNature
    {
        None = 0, MainMenu=1, FightScene = 2
    }

    public enum TypeOfSFXByPlace
    {
        Local=0, Global=1
    }
}
