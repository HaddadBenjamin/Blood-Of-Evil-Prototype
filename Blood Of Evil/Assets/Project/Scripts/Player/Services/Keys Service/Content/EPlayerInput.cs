using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Keys
{
    [System.Serializable]
    public enum EPlayerInput
    {
        Move,
        CloseAllMenus,
        AttributesEditorMenu,
        LanguageEditorMenu,

        MainMenu,
        SettingsMenu,
        LanguageMenu,
        SoundConfigurationMenu,
        VideoConfigurationMenu,
        KeysConfigurationMenu,
        EnableOrDisableHealthBars,
        AttackWithoutMove,
        Portal,
        GameMenu,
        Select,
        StopToMove,

        MoveForward,
        MoveLeft,
        MoveRight,
        MoveBackward,

        MoveForward2,
        MoveLeft2,
        MoveRight2,
        MoveBackward2,

        LevelUpMenu
        //LifeManaExperienceMenu,
    }
}