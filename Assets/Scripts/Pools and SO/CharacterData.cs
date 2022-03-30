using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new_Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    [Header("Presentation Data")]
    public string name = "New Character";
    [TextArea] public string description = "Here goes the description of the awesome new character of the game. He likes pizza by defaut.";
    [TextArea] public string unlockCondition = "Achieve a super duper highscore to reveal this champ.";
    public Sprite displaySprite;
    public Sprite lockedSprite;
    [Header("Visual Modifiers")]
    public  AnimatorOverrideController inGameAnimator;

    [Header("Unlock Conditions")]
    public bool isUnlocked = false;
    public AchievementType unlockConditionType = AchievementType.None;
    public int unlockConditionGoal = 1;

}

public enum AchievementType {GamesPlayed, Highscore, MaxCombo, MaxComboWhenDupershot, DupershotsMade, Hits, None}
