using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
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
        "Echoloation",
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
        "Dextroamphetamine",
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
    void ResetWord()
    {
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
    void CreateVirtualKeyboard()
    {
        letterButtonPanel.transform.position += new Vector3(0f, 65f, 0f);
        for(int i = 0; i < letters.Length; i++)
        {
            GameObject newLetterButton = Instantiate(letterButtonPrefab, letterButtonPanel);
            LetterButton letterButton = new LetterButton(newLetterButton, letters[i], i);
            letterButtons.Add(letterButton);
            letterButton.button.onClick.AddListener(() => LetterButtonClickHandler(letterButton));
        }
    }
    private void Start()
    {
        lockoutText = lockoutPanel.GetComponentInChildren<TextMeshProUGUI>();
        lockoutPanel.SetActive(false);
        CreateVirtualKeyboard();
        ResetWord();
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
                ResetWord();
            }
        }
    }
    void LetterButtonClickHandler(LetterButton letterButtonPressed)
    {
        string letterGuessed = letters[letterButtonPressed.letterIndex];
        if (guessedLetters.Contains(letterGuessed))
            return;
        bool guessCorrect = wordLetters.Contains(letterGuessed);
        Debug.Log("guessed " + letterGuessed);
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
            lockoutTimer = lockoutTime;
            if(lockoutTime > 20f && lockoutTime < 30f)
            {
                lockoutTime += lockoutTime * 0.35f;
            }
            else
            {
                lockoutTime += lockoutTime * 0.75f;
            }
        }
        letterButtonPressed.Disable(guessCorrect);
    }
}
