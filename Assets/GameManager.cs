using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    public Player player;
    static List<string> words = new(){ 
        "Orchestra",
        "Structure",
        "Pose",
        "Export",
        "Wrathful",
        "Knave",
        "Vicarious",
        "Mitosis",
        "Apparently",
        "Exploitative",
        "Exportation",
        "Violence",
        "Volition",
        "Encyclopaedia",
        "Question",
        "Quartz",
        "Amethyst",
        "Emerald",
        "Heatstroke",
        "Refridgerator",
        "Psychological",
        "Psychosomatic",
        "Lovers",
        "Programming",
        "Languages",
        "Roulette",
        "Terminal",
        "Console",
        "Effortless",
        "Cumulative",
        "Exponentiation",
        "Coordinate",
        "Transformation",
        "Linearisation",
        "Bonfire",
        "Summation",
        "Photography",
        "Geographical",
        "Metaphor",
        "Motherboard",
        "Integrity",
        "Moonstone",
        "Echolocation",
        "Diamond",
        "Monochromatic",
        "Hypothermia",
        "Schizophrenia",
        "Behaviour",
        "Lovesick",
        "Existence",
        "Hangover",
        "Cache",
        "Caffeine",
        "Extension",
        "Sunlight",
        "Rose",
        "Daffodil",
        "Dandelion",
        "Wattle",
        "Sunflower",
        "Lavender",
        "Blossom",
        "Bloom",
        "Crystalisation",
        "Monetisation",
    };
    static List<string> wordsUsed = new();
    public GameObject letterPrefab;
    public RectTransform wordGuessPanel;
    public GameObject letterButtonPrefab;
    public RectTransform letterButtonPanel;
    public string currentWord;
    List<TextMeshProUGUI> textMeshes = new();
    static string[] letters = new string[26] {
        "Q",
        "W",
        "E",
        "R",
        "T",
        "Y",
        "U",
        "I",
        "O",
        "P",
        "A",
        "S",
        "D",
        "F",
        "G",
        "H",
        "J",
        "K",
        "L",
        "Z",
        "X",
        "C",
        "V",
        "B",
        "N",
        "M",
    };
    List<string> wordLetters = new();
    List<string> guessedLetters = new();
    public class LetterButton
    {
        public Button button;
        public int letterIndex;
        public TextMeshProUGUI textMesh;
        public Image buttonBlockImage;
        public LetterButton(GameObject buttonObject, string letter, int letterIndex)
        {
            button = buttonObject.GetComponent<Button>();
            this.letterIndex = letterIndex;
            textMesh = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = letter;
            Image[] images = buttonObject.GetComponentsInChildren<Image>();
            buttonBlockImage = images[1];
        }
        public void Disable(bool guessCorrect)
        {
            button.interactable = false;
            if(!guessCorrect)
            {
                buttonBlockImage.color = Color.black;
            }
        }
        public void Enable()
        {
            button.interactable = true;
            buttonBlockImage.color = Color.clear;
        }
    }
    List<LetterButton> letterButtons = new();
    public float lockoutTime;
    float lockoutTimer;
    public GameObject lockoutPanel;
    TextMeshProUGUI lockoutText;
    EnemySpawnManager enemySpawnManager;
    public GameObject gameOverPanel;
    public static bool upgradeScreenActive;
    float[] upgradeTierChances = new float[5] { 
        0.5f,
        0.35f,
        0.10f,
        0.035f,
        0.015f
    };
    public Color[] upgradeTierColors;
    public List<Upgrade> common;
    public List<Upgrade> uncommon;
    public List<Upgrade> rare;
    public List<Upgrade> epic;
    public List<Upgrade> legendary;
    Dictionary<UpgradeTier, List<Upgrade>> upgrades = new Dictionary<UpgradeTier, List<Upgrade>>();
    Dictionary<Upgrade, int> collectedUpgrades = new Dictionary<Upgrade, int>();
    public GameObject upgradePanel;
    public RectTransform upgradeChoicePanel;
    public GameObject upgradeCardPrefab;
    List<UpgradeCard> upgradeCards = new();
    Camera mainCamera;

    public static List<Action<Upgrade, int>> upgradeCollectActions = new();
    public static int playerDamage = 1;

    public TextMeshProUGUI waveDifficultyText;
    public TextMeshProUGUI wordsCompletedText;
    public TextMeshProUGUI freeGuessesText;
    public TextMeshProUGUI enemiesKilledText;

    public static int enemiesKilled;
    public static int wordsCompleted;
    public static int maxFreeGuesses;
    public static int freeGuessesLeft;
    
    private void Start()
    {
        playerDamage = 1;
        mainCamera = Camera.main;
        lockoutText = lockoutPanel.GetComponentInChildren<TextMeshProUGUI>();
        lockoutPanel.SetActive(false);
        CreateVirtualKeyboard();
        ResetWord();
        enemySpawnManager = GetComponent<EnemySpawnManager>();
        upgrades.TryAdd(UpgradeTier.Common, common);
        upgrades.TryAdd(UpgradeTier.Uncommon, uncommon);
        upgrades.TryAdd(UpgradeTier.Rare, rare);
        upgrades.TryAdd(UpgradeTier.Epic, epic);
        upgrades.TryAdd(UpgradeTier.Legendary, legendary);
        for(int i = 0; i < 3; i++)
        {
            GameObject upgradeCardObject = Instantiate(upgradeCardPrefab, upgradeChoicePanel);
            UpgradeCard upgradeCard = new();
            upgradeCard.button = upgradeCardObject.GetComponent<Button>();
            upgradeCard.button.onClick.AddListener(() => SelectUpgrade(upgradeCard.upgrade));
            upgradeCard.textMesh = upgradeCardObject.GetComponentInChildren<TextMeshProUGUI>();
            upgradeCard.buttonImage = upgradeCardObject.GetComponent<Image>();
            upgradeCards.Add(upgradeCard);
        }
        player.RegisterPlayerLostHealthCallback(PlayerLostHealthHandler);
    }
    void ResetWord()
    {
        foreach (KeyValuePair<Upgrade, int> kvp in collectedUpgrades)
        {
            if(kvp.Key.upgradeFlag.HasFlag(UpgradeFlag.CorrectWordLowerWaveDifficulty))
            {
                enemySpawnManager.waveDifficulty -= kvp.Value;
                if(enemySpawnManager.waveDifficulty < 1)
                {
                    enemySpawnManager.waveDifficulty = 1;
                }
            }
        }
        freeGuessesLeft = 3;
        int wordSelected = Random.Range(0, words.Count);
        string word = words[wordSelected];
        currentWord = word.ToUpper();
        wordLetters.Clear();
        foreach (char letter in currentWord)
        {
            string letterString = letter.ToString().ToUpper();
            if(!wordLetters.Contains(letterString))
            {
                wordLetters.Add(letterString);
            }
        }
        int wordLength = word.Length;
        textMeshes.Clear();
        for(int i = 0; i < wordLength; i++)
        {
            if(i >= wordGuessPanel.childCount)
            {
                GameObject newLetter = Instantiate(letterPrefab, wordGuessPanel);
                TextMeshProUGUI textMesh = newLetter.GetComponent<TextMeshProUGUI>();
                textMesh.text = "_";
                textMeshes.Add(textMesh);
            }
            else
            {
                GameObject letter = wordGuessPanel.GetChild(i).gameObject;
                letter.SetActive(true);
                TextMeshProUGUI textMesh = letter.GetComponent<TextMeshProUGUI>();
                textMesh.text = "_";
                textMeshes.Add(textMesh);
            }
        }
        for(int i = 0; i < wordGuessPanel.childCount; i++)
        {
            if(i >= wordLength)
            {
                wordGuessPanel.GetChild(i).gameObject.SetActive(false);
            }
        }

        words.RemoveAt(wordSelected);
        wordsUsed.Add(currentWord);
        if(letterButtons != null && letterButtons.Count > 0)
        {
            foreach (LetterButton letterButton in letterButtons)
            {
                letterButton.Enable();
            }
        }
    }
    //1440 -> cellheight = 100 --> takes up 200
    //1440:200 -> 
    //grid must take up 5*pixelHeight/36
    void CreateVirtualKeyboard()
    {
        float cameraWidth = mainCamera.pixelWidth;
        float cameraHeight = mainCamera.pixelHeight;
        GridLayoutGroup keyboardLayout = letterButtonPanel.GetComponent<GridLayoutGroup>();
        Debug.Log($"Size should be {letterButtonPanel.transform.position.y}");
        float gridLayoutSize = 5 * (cameraHeight / 36f);
        keyboardLayout.cellSize = new(gridLayoutSize/2f, gridLayoutSize/2f);
        letterButtonPanel.transform.position += new Vector3(0f, 65f, 0f);
        for(int i = 0; i < letters.Length; i++)
        {
            GameObject newLetterButton = Instantiate(letterButtonPrefab, letterButtonPanel);
            LetterButton letterButton = new LetterButton(newLetterButton, letters[i], i);
            letterButtons.Add(letterButton);
            letterButton.button.onClick.AddListener(() => LetterButtonClickHandler(letterButton));
        }
    }

    void Update()
    {
        if(lockoutTimer > 0)
        {
            lockoutTimer -= Time.deltaTime;
            lockoutPanel.SetActive(true);
            if(lockoutTimer <= 0)
            {
                lockoutTimer = 0;
            }
            lockoutText.text = $"LOCKED OUT \n TIME LEFT = {Math.Round((double)lockoutTimer, 2)}";
        }
        else
        {
            lockoutPanel.SetActive(false);
        }
        if(wordLetters == null || wordLetters.Count == 0)
        {
            if(currentWord != null || currentWord.Length > 0)
            {
                if(!upgradeScreenActive)
                {
                    upgradePanel.SetActive(true);
                    ResetUpgradeScreen();
                    wordsCompleted++;
                }
            }
        }

        waveDifficultyText.text = $"Wave Difficulty \n {enemySpawnManager.waveDifficulty}";
        wordsCompletedText.text = $"Words Completed \n {wordsCompleted}";
        enemiesKilledText.text = $"Enemies Killed \n {enemiesKilled}";
        freeGuessesText.text = $"Free Guesses \n {freeGuessesLeft}";
    }
    void LetterButtonClickHandler(LetterButton letterButtonPressed)
    {
        string letterGuessed = letters[letterButtonPressed.letterIndex];
        if (guessedLetters.Contains(letterGuessed))
            return;
        bool guessCorrect = wordLetters.Contains(letterGuessed);
        Debug.Log("guessed " + letterGuessed);
        foreach(KeyValuePair<Upgrade, int> kvp in collectedUpgrades)
        {
            if(kvp.Key.upgradeFlag.HasFlag(UpgradeFlag.CorrectLetterWipeEnemies))
            {
                enemySpawnManager.KillAllEnemies();
            }
        }
        if(guessCorrect)
        {
            wordLetters.Remove(letterGuessed);
            for (int i = 0; i < currentWord.Length; i++)
            {
                string letter = currentWord[i].ToString().ToUpper();
                if(letter == letterGuessed)
                {
                    textMeshes[i].text = letterGuessed;
                }
            }
        }
        else
        {
            if(freeGuessesLeft <= 0)
            {
                lockoutTimer = lockoutTime;
                lockoutTime += lockoutTime * 0.75f;
                if (lockoutTime > 20f)
                {
                    lockoutTime = 20f;
                }
                enemySpawnManager.waveDifficulty++;
                freeGuessesLeft += maxFreeGuesses;
            }
            else
            {
                freeGuessesLeft--;
            }
        }
        letterButtonPressed.Disable(guessCorrect);
    }
    void PlayerLostHealthHandler(int healthLeft)
    {
        if(healthLeft <= 0)
        {
            gameOverPanel.SetActive(true);
        }
    }
    public void ResetGame()
    {
        player.ResetGame();
        enemySpawnManager.ResetGame();
        gameOverPanel.SetActive(false);
        lockoutTimer = 0;
        lockoutTime = 1;
        ResetWord();
        playerDamage = 1;
        enemiesKilled = 0;
        wordsCompleted = 0;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    void ResetUpgradeScreen()
    {
        List<Upgrade> selectedUpgrades = new();
        for(int cardIndex = 0; cardIndex < 3; cardIndex++)
        {
            UpgradeTier highestTierRolled = UpgradeTier.Common;
            for (int i = 0; i < upgradeTierChances.Length; i++)
            {
                float roll = Random.Range(0f, 1f);
                if (roll <= upgradeTierChances[i])
                {
                    if (i > (int)highestTierRolled)
                    {
                        highestTierRolled = (UpgradeTier)i;
                    }
                }
            }

            List<Upgrade> upgradesToChooseFromInTier = new();
            foreach (Upgrade upgrade in upgrades[highestTierRolled])
            {
                if(selectedUpgrades.Contains(upgrade))
                {
                    continue;
                }
                upgradesToChooseFromInTier.Add(upgrade);
            }
            int selectedUpgradeIndex = Random.Range(0, upgradesToChooseFromInTier.Count);
            Upgrade selectedUpgrade = upgradesToChooseFromInTier[selectedUpgradeIndex];
            selectedUpgrades.Add(selectedUpgrade);
            //Debug.Log(highestTierRolled + "   index = " + cardIndex + selectedUpgrade.description);
            upgradeCards[cardIndex].upgrade = selectedUpgrade;
            upgradeCards[cardIndex].textMesh.text = selectedUpgrade.description;
            upgradeCards[cardIndex].buttonImage.color = upgradeTierColors[(int)highestTierRolled];
        }
        upgradeScreenActive = true;
    }
    void SelectUpgrade(Upgrade upgradeSelected)
    {
        upgradeScreenActive = false;
        if(collectedUpgrades.ContainsKey(upgradeSelected))
        {
            collectedUpgrades[upgradeSelected] = collectedUpgrades[upgradeSelected] + 1;
        }
        else
        {
            collectedUpgrades.Add(upgradeSelected, 1);
        }
        foreach (Action<Upgrade, int> action in upgradeCollectActions)
        {
            action(upgradeSelected, collectedUpgrades[upgradeSelected]);
        }
        if(upgradeSelected.upgradeFlag.HasFlag(UpgradeFlag.BulletDamageUp))
        {
            playerDamage = 1 + collectedUpgrades[upgradeSelected];
        }
        if(upgradeSelected.upgradeFlag.HasFlag(UpgradeFlag.FreeGuessesUp))
        {
            maxFreeGuesses = collectedUpgrades[upgradeSelected];
            freeGuessesLeft = collectedUpgrades[upgradeSelected];
        }
        if(upgradeSelected.upgradeFlag.HasFlag(UpgradeFlag.CorrectLetterWipeEnemies))
        {
            legendary.Remove(upgradeSelected);
        }
        upgradePanel.SetActive(false);
        ResetWord();
    }
    public static void RegisterUpgradeCollectCallback(Action<Upgrade, int> a)
    {
        upgradeCollectActions.Add(a);
    }
}
[Serializable]
public class Upgrade
{
    public UpgradeFlag upgradeFlag;
    public string description;
    public UpgradeTier tier;
}
public enum UpgradeTier
{
    Common, Uncommon, Rare, Epic, Legendary
}
[Flags]
public enum UpgradeFlag
{
    None = 0,
    Everything = 1,
    EmissionRateUp = 1 << 1,
    MoveSpeedUp = 1 << 2,
    BulletDamageUp = 1 << 3,
    Vampire = 1 << 4,
    KnockbackShot = 1 << 5,
    ExplosiveEnemies = 1 << 6,
    BounceShotNormal = 1 << 7,
    FreeGuessesUp = 1 << 8,
    CorrectWordLowerWaveDifficulty = 1 << 9,
    CorrectLetterEmissionUp = 1 << 10,
    CorrectLetterWipeEnemies = 1 << 11,
    BounceShotPiercing = 1 << 12,
}
public class UpgradeCard
{
    public Upgrade upgrade;
    public Button button;
    public TextMeshProUGUI textMesh;
    public Image buttonImage;
}
//Common examples
//emission rate per second increase
//bullet damage up
//Uncommon examples
//every 5 kills get health back
//bullets push back enemies
//Rare
//dead enemies explode into bullets which damage other enemies
//every 5 seconds fire a bullet which bounces around damaging enemies
//Epic
//you can guess incorrectly once without lockout. getting locked out or guessing correctly resets this.
//guessing word correctly decreases wave difficulty
//Legendary
//guessing letter correctly increases bullets per second permanently (until death)
//guessing letter correctly kills all enemies
//every 5 seconds fire a bullet which bounces and pierces enemies