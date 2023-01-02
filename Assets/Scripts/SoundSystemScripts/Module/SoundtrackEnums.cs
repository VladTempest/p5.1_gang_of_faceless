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
        Heavy_armor_foley = 6, Light_armor_foley = 7, Archer_armor_foley = 8, 
        Sword_swoosh = 9, Dagger_swoosh = 10,
        Blood_spaltter = 11, Male_grunt = 12,
        Armor_hit = 13, Without_Armor_hit = 14, 
        Female_grunt = 15, Dismembering = 16,
        ArrowHit = 17, Hit_bone = 18, Knockdown_hit = 19, Push_hit = 20, 
        Bow_stretch = 21, Bow_launch = 22, Arrow_take_new = 23, Paralyze_Arrow_Launch = 24,  Paralyze_Arrow_Setup = 25, 
        UI_CLick_OnActionChoose = 26, UI_Click_OnTargetChoose = 27, UI_Click_OnUnitChoose = 28,
        Backstab_sound = 29,
        Throw_bomb = 30, Take_out_bomb = 31, Bomb_explosion = 32
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
