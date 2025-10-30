using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Références UI")]
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject instructionsPanel;
    public GameObject feedbackPanel;

    [Header("Configuration circuit")]
    public float batteryVoltage = 9f;
    public float targetCurrent = 0.5f;

    [Header("État jeu")]
    public int currentStep = 0;
    public int score = 0;
    public bool circuitCompleted = false;
    public List<CircuitComponent> allComponents = new List<CircuitComponent>();

    private string logFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        logFilePath = Application.persistentDataPath + "/simulation_log.txt";
        LogAction("Début simulation");
        ShowInstructions(0);
        UpdateScoreUI();
    }

    public void ShowInstructions(int step)
    {
        string[] instructions = {
            "Étape 1: Faites glisser la résistance vers la zone de travail",
            "Étape 2: Connectez la batterie à la résistance",
            "Étape 3: Ajoutez l'ampoule au circuit",
            "ÉTAPE 4: Cliquez Valider pour vérifier votre circuit"
        };

        if (step < instructions.Length)
        {
            instructionsText.text = instructions[step];
            instructionsPanel.SetActive(true);
        }
    }

    public void ShowFeedback(string message)
    {
        feedbackText.text = message;
        feedbackPanel.SetActive(true);
        StartCoroutine(HideFeedbackAfterDelay(3f));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackPanel.SetActive(false);
    }

    public void ValidateCircuit()
    {
        LogAction("Bouton Valider cliqué");

        circuitCompleted = CheckCircuitCompletion();
        bool circuitCorrect = CheckCircuitCorrectness();

        if (circuitCompleted)
        {
            if (circuitCorrect)
            {
                score = 100;
                ShowFeedback("✅ BRAVO ! Circuit CORRECT !");
                LogAction("Circuit VALIDE - Score: 100/100");
            }
            else
            {
                score = 50;
                ShowFeedback("⚠️ Circuit complet mais valeurs incorrectes");
                LogAction("Circuit incomplet - Score: 50/100");
            }
        }
        else
        {
            ShowFeedback("❌ Circuit INCOMPLET. Connectez tous les composants.");
            LogAction("Circuit incomplet");
        }

        UpdateScoreUI();
    }

    private bool CheckCircuitCompletion()
    {
        foreach (var comp in allComponents)
        {
            if (!comp.IsConnected())
                return false;
        }
        return true;
    }

    private bool CheckCircuitCorrectness()
    {
        return true;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score + " / 100";
    }

    public void LogAction(string action)
    {
        string logEntry = System.DateTime.Now.ToString("HH:mm:ss") + " - " + action;

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logEntry);
        }

        Debug.Log("🔍 LOG: " + logEntry);
    }

    public void CompleteStep(int stepNumber, int points)
    {
        score += points;
        currentStep = stepNumber;
        ShowInstructions(currentStep);
        UpdateScoreUI();
        LogAction("Étape " + stepNumber + " complétée - +" + points + " points");
    }
}