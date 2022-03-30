using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] CharacterData[] GameCharacters;
    [SerializeField] AnimatorOverrideController[] CharacterAnimators;

    [Header("Caracter Selection Display")]    
    [SerializeField] SpriteRenderer CharacterDisplayImage;
    [SerializeField] TMP_Text NameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] GameObject characterLockedIcon;
    private int currentSelected;

    [Header("Character Swap Variables")]
    [SerializeField] Animator playerCharacterAnimator;

    [Header("Visual Feedback")]
    [SerializeField] GameObject UnlockAnnounceObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    #region CHARACTER DISPLAY METHODS
    /// <summary>
    /// Muestra el personaje escogido por el jugador
    /// </summary>
    public void ShowOnSelected()
    {
        if (!PlayerPrefs.HasKey("SelectedCharacter") || !GameCharacters[PlayerPrefs.GetInt("SelectedCharacter")].isUnlocked)
        {
            PlayerPrefs.SetInt("SelectedCharacter", 0);
        }
        
        currentSelected = PlayerPrefs.GetInt("SelectedCharacter");

        ShowOnCurrent();
    }
    /// <summary>
    /// Muestra al personaje actual que desee ver el jugador
    /// </summary>
    public void ShowOnCurrent()
    {
        CharacterData selectedCharacter = GameCharacters[currentSelected];

        LoadCharacterDisplay(selectedCharacter);

        if (selectedCharacter.isUnlocked)
        {
            PlayerPrefs.SetInt("SelectedCharacter", currentSelected);
            SetCurrentCharacter();
        }
    }

    public void LoadCharacterDisplay(CharacterData selectedCharacter)
    {   
        if (selectedCharacter.isUnlocked)
        {
            NameText.text = selectedCharacter.name;
            descriptionText.text = selectedCharacter.description;
            CharacterDisplayImage.sprite = selectedCharacter.displaySprite;
        }
        else
        {
            NameText.text = "???";
            descriptionText.text = selectedCharacter.unlockCondition; 
            CharacterDisplayImage.sprite = selectedCharacter.lockedSprite;
        }
    }

    /// <summary>
    /// Muestra al siguiente personaje
    /// </summary>
    public void RotateToNext()
    {
        if ((currentSelected + 1) < GameCharacters.Length)
        {
            currentSelected++;
        }
        else
        {
            currentSelected = 0;
        }

        ShowOnCurrent();
    }

    public void RotateToPrevious()
    {
        if ((currentSelected - 1) < 0)
        {
            currentSelected = GameCharacters.Length - 1;
        }
        else
        {
            currentSelected--;
        }

        ShowOnCurrent();
    }

    #endregion

    #region CHARACTER SWAP METHODS

    public void SetCurrentCharacter()
    {
        playerCharacterAnimator.runtimeAnimatorController = GameCharacters[PlayerPrefs.GetInt("SelectedCharacter")].inGameAnimator;
    }

    #endregion

    #region UNLOCK CHARACTER METHODS

    public void LookForNewCharacters()
    {
        if (AreNewCharactersUnlocked())
        {
            UnlockAnnounceObject.SetActive(true);
        }
        else
        {
            UnlockAnnounceObject.SetActive(false);
        }
    }

    public void CheckCharacters()
    {
        foreach (CharacterData character in GameCharacters)
        {
            switch (character.unlockConditionType)
            {
                case AchievementType.None:
                    character.isUnlocked = true;
                    break;

                case AchievementType.GamesPlayed:
                    if (SaveSystem.LoadPlayer().gamesPlayed >= character.unlockConditionGoal)
                    {
                        character.isUnlocked = true;
                    }
                    else
                    {
                        character.isUnlocked = false;
                    }
                    break;

                case AchievementType.Highscore:
                    if (SaveSystem.LoadPlayer().solidHighscore >= character.unlockConditionGoal)
                    {
                        character.isUnlocked = true;
                    }
                    else
                    {
                        character.isUnlocked = false;
                    }
                    break;

                case AchievementType.Hits:
                    if (SaveSystem.LoadPlayer().enemiesDestroyedHighscore >= character.unlockConditionGoal)
                    {
                        character.isUnlocked = true;
                    }
                    else
                    {
                        character.isUnlocked = false;
                    }
                    break;

                case AchievementType.MaxCombo:
                    if (SaveSystem.LoadPlayer().maxCombo >= character.unlockConditionGoal)
                    {
                        character.isUnlocked = true;
                    }
                    else
                    {
                        character.isUnlocked = false;
                    }
                    break;

                case AchievementType.MaxComboWhenDupershot:
                    if (SaveSystem.LoadPlayer().maxComboWhenDupershot >= character.unlockConditionGoal)
                    {
                        character.isUnlocked = true;
                    }
                    else
                    {
                        character.isUnlocked = false;
                    }
                    break;
            }
        }       
    }

    int CountUnlockedCharacters()
    {
        int count = 0;

        foreach (CharacterData c in GameCharacters)
        {
            if (c.isUnlocked) { count++; }
        }

        return count;
    }

    bool AreNewCharactersUnlocked()
    {
        int startValue = CountUnlockedCharacters();
        CheckCharacters();
        int lateValue = CountUnlockedCharacters();

        return lateValue > startValue;
    }

    #endregion

}

