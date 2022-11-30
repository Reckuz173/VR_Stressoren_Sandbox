using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// DigitPad-Klasse.
/// Essentielle Funktionen für Steuerung und Logik des DigitPads.
/// 
/// author: Robin Staab
/// date: 31.05.2022 09:00
/// copyright: §69a Absatz 3 Satz 1 UrhG
/// last editor: Robin Staab
/// 
/// </summary>
public class DigitPad : MonoBehaviour
{
    [SerializeField] string actualWord = "";
    [SerializeField] EditorSettings settings;
    [SerializeField] Text monitorText;
    [SerializeField] Text submitText;
    [SerializeField] Text input;
    [SerializeField] bool increasingDifficulty = true;


    /// <summary>
    /// Start
    /// </summary>
    public void Start()
    {
        restartQuiz();
    }

    /// <summary>
    /// Funktion zur Realisierung der Zahleneingabe
    /// </summary>
    /// <param name="i">Gedrückte Zahl</param>
    public void Click(int i)
    {
        input.text += ""+i;
    }

    /// <summary>
    /// Funktion zum Überprüfen der Eingabe und Reaktion nach Richtigkeit
    /// </summary>
    public void submit()
    {
        if(input.text.Equals(actualWord))
        {
            submitText.color = Color.green;
            submitText.text = "Correct";
            settings.increaseScore();
            Reset();
            StartCoroutine(ClearSubmitText());
            restartQuiz();
            if(increasingDifficulty) settings.levelOfDifficulty++;
            Logger.Log("Correct Input.");
        }
        else
        {
            submitText.color = Color.red;
            submitText.text = "Wrong";
            Reset();
            StartCoroutine(ClearSubmitText());
            Logger.Log("Wrong Input");
        }
    }

    /// <summary>
    /// Funktion zum Reset der bisherigen Eingabe
    /// </summary>
    public void Reset()
    {
        input.text = "";
    }

    /// <summary>
    /// Funktion zum Wiederrufen der letzten Eingabe
    /// </summary>
    public void Back()
    {
        if(input.text.Length > 0)
            input.text = input.text.Remove(input.text.Length-1);
    }

    /// <summary>
    /// Funktion zum Neustart des Quiz
    /// </summary>
    private void restartQuiz()
    {
        actualWord = "";
        for (int i = 0; i <= settings.levelOfDifficulty; i++)
        {
            actualWord += Random.Range(0, 9) + "";
        }
        monitorText.text = actualWord;
        Logger.Log("New Word: " + actualWord);
    }

    /// <summary>
    /// Funktion zum zeitverzögerten Clearen der Ergebnisanzeige
    /// </summary>
    /// <returns></returns>
    public IEnumerator ClearSubmitText()
    {
        yield return new WaitForSeconds(1.0f);
        submitText.text = "";
    }
}
