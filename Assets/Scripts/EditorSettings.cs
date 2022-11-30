using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Editor-Klasse.
/// Kontrolle der Einstellungen auf Änderungen, Laden der Einstellungsdatei.
/// 
/// author: Robin Staab
/// date: 31.05.2022 09:00
/// copyright: §69a Absatz 3 Satz 1 UrhG
/// last editor: Robin Staab
/// 
/// </summary>
public class EditorSettings : MonoBehaviour
{

    [Header("Settings - Plank")]
    [SerializeField] [Range(0.1f, 1f)] public float plankSize = 1f;
    [SerializeField] [Range(1f, 12.5f)] public float plankLength = 2f;
    [SerializeField] Transform plankTransform;
    [Header("Settings - Platforms")]
    [SerializeField] [Range(1f, 12.5f)] public float distance = 2f;
    [SerializeField] [Range(0.5f, 17f)] public float height = 1f;
    [SerializeField] Transform startPlatformTransform;
    [SerializeField] Transform endPlatformTransform;
    [SerializeField] Transform endPlatformBuildingTransform;
    [SerializeField] Transform moveableEnvironmentTransform;
    [Header("Settings - Environment")]
    [SerializeField] GameObject dlight;
    [SerializeField] bool daylight = true;
    bool lightCheck = true;
    [SerializeField] GameObject rain;
    [SerializeField] bool raining = true;
    bool rainingCheck = true;
    [SerializeField] AudioSource windAudio;
    [Header("Settings - Effects")]
    [SerializeField] AudioSource sirenAudio;
    [SerializeField] bool siren = false;
    bool sirenCheck = false;
    [Header("Settings - Simulation")]
    [SerializeField] [Range(1, 10)] public int levelOfDifficulty = 1;
    [Header("Settings - Other")]
    [SerializeField] [Range(0f, 0.5f)] public float fogDensity = 0.0f;
    [SerializeField] bool developerMode = false;
    [SerializeField] GameObject deactivatedObjectsOnFinish;
    [SerializeField] Text sessionCompleteText;
    [SerializeField] GameObject developerMenu;
    [Header("Infos - Progress")]
    [SerializeField] int countdown = 90;
    [SerializeField] Text countdownText;
    [SerializeField] int score = 0;
    [SerializeField] Text scoreText;

    //TEMP
    int _countdown;
    SettingsData data;
    string fullPath;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        string s = DateTime.Now.Date.Day + "_" + DateTime.Now.TimeOfDay.Hours + "_" + DateTime.Now.TimeOfDay.Minutes + "_" + DateTime.Now.TimeOfDay.Seconds + "_" + "log.txt";
        Logger.logPath = Path.Combine(Path.Combine(Application.dataPath, "logs"), s);
        if (siren) sirenAudio.Play();
        data = new SettingsData();
        fullPath = Path.Combine(Application.dataPath, "settings.json");
        developerMode = false;
        deactivatedObjectsOnFinish.SetActive(true);
        sessionCompleteText.gameObject.SetActive(false);
        _countdown = countdown;
        StartCoroutine(StartCountdown());
        if (!File.Exists(fullPath))
        {
            WriteSettingsFileIfNotExists();
            LoadSettingsFile();
        }
        else
        {
            LoadSettingsFile();
        }
        Logger.Log("Initialized Session with parameters stored in " + fullPath);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        CheckForChanges();
    }

    /// <summary>
    /// Funktion zum Kontrollieren der Einstellungen
    /// </summary>
    void CheckForChanges()
    {

        // HOTFIX_2: Plank Offset
        float plankOffset = 0.15f;


        // Plank
        if (plankTransform.localScale.x != plankSize || plankTransform.localScale.z != plankLength)
        {
            // HOTFIX_1: startPlatformTranform verschieben für Dachgibel
            startPlatformTransform.localPosition =
                new Vector3(startPlatformTransform.localPosition.x, //- 1.5f,
                startPlatformTransform.localPosition.y,
                startPlatformTransform.localPosition.z);




            // Anpassungen der Planken dicke für eine realistischere Abbildung der echten Planke
            plankTransform.localScale = 
                new Vector3(
                    plankSize,
                    // Added: / 4
                    plankTransform.localScale.y / 4 , 
                    plankLength);
            plankTransform.localPosition = 
                new Vector3(
                    // Added: - plankOffset HOTFIX_2
                    startPlatformTransform.localPosition.x + 4.35f + (plankLength - 1) / 2 - plankOffset,
                    // Added: + plankTransform.localScale.y * 1.5f
                    plankTransform.localPosition.y + plankTransform.localScale.y * 1.5f, 
                    plankTransform.localPosition.z);
        }

        // Platforms


        // Plattformen sollten am Hausrand enden.
        /* Ursprünglich 200cm x 200cm x 50cm Plattformen mit Planke beginnend an den Enden der Plattformen
         * Idee: Planke liegt wenige Centimeter auf der Plattform (muss erst beschritten werden)
         * Plattformen enden genau mit der Häuserkante
         * 
         * if (endPlatformTransform.localPosition.x - startPlatformTransform.localPosition.x != distance + 2)
         *  besser: if (ePT.lP.x - sPT.lP.x != distance + ePT.localeScale.x)
         * 
         * Start Planke x - 1.5 für Dachgibel am Start.
         *  Problem Planke muss auch um 1.5 nach vorne gezogen werden. Siehe HOTFIX_1
         * 
         * End Planke sollte mit Dachgibel des Endhauses abschließen, nicht überhängen!
         * 
         * Planke sollte auf Plattformen aufliegen, daher Offset in HOTFIX_2
         * 
         */

        // Changed: distance + endPlatformTransform.localScale.x
        // Before: distance + 2
        if (endPlatformTransform.localPosition.x - startPlatformTransform.localPosition.x != distance + endPlatformTransform.localScale.x)
        {
            endPlatformTransform.localPosition = 
                new Vector3(
                    // Changed: distance + endPlatformTransform.localScale.x - plankOffset * 2
                    // Before: distance + 2
                    startPlatformTransform.localPosition.x + distance + endPlatformTransform.localScale.x - plankOffset * 2, 
                    endPlatformTransform.localPosition.y, 
                    endPlatformTransform.localPosition.z);

            endPlatformBuildingTransform.position = 
                new Vector3(
                    // Changed: distance - 0.75f
                    // Before: startPlatformTransform.position.x - distance - 2.25f
                    endPlatformTransform.position.x + 4.10f, 
                    endPlatformBuildingTransform.position.y, 
                    endPlatformBuildingTransform.position.z);
        }

        if (moveableEnvironmentTransform.localPosition.y != height)
        {
            moveableEnvironmentTransform.localPosition = 
                new Vector3(moveableEnvironmentTransform.localPosition.x, 
                height, 
                moveableEnvironmentTransform.localPosition.z);
            windAudio.volume = height / 17f;
        }

        //Environment
        if (daylight != lightCheck)
        {
            lightCheck = daylight;
            dlight.SetActive(daylight);
            Logger.Log("Daylight: " + daylight);
        }
        if (raining != rainingCheck)
        {
            rainingCheck = raining;
            rain.SetActive(raining);
            Logger.Log("Raining: " + raining);
        }
        if (siren != sirenCheck)
        {
            sirenCheck = siren;
            if (siren) sirenAudio.Play();
            else sirenAudio.Stop();
            Logger.Log("Siren: " + siren);
        }
        //Other
        if (fogDensity != RenderSettings.fogDensity) RenderSettings.fogDensity = fogDensity;
        if (developerMode != developerMenu.activeInHierarchy) developerMenu.SetActive(developerMode);
    }

    /// <summary>
    /// Funktion zum Starten des Testcountdowns
    /// </summary>
    /// <returns>--</returns>
    public IEnumerator StartCountdown()
    {
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "Time: " + countdown + "s";
            countdown--;
        }
        if (countdown == 0)
        {
            countdownText.text = "Time: 0s";
            Debug.Log("Results of Training:\nTime: " + _countdown + "s\nScore: " + score);
            sessionCompleteText.gameObject.SetActive(true);
            deactivatedObjectsOnFinish.SetActive(false);
            Logger.Log("Finished session.");
        }
    }

    /// <summary>
    /// Funktion zum Erhöhen des Scores.
    /// </summary>
    public void increaseScore()
    {
        score++;
        scoreText.text = "Score: " + score;
        Logger.Log("New Score: " + score);
    }

    /// <summary>
    /// Erstellen der Einstellungsdatei, sofern sie nicht existiert.
    /// </summary>
    private void WriteSettingsFileIfNotExists()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            String dataToStore = JsonUtility.ToJson(data, true);

            Debug.Log(dataToStore);

            FileStream stream = new FileStream(fullPath, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(dataToStore);
            writer.Flush();
            writer.Close();

            Debug.Log("Created a file at " + fullPath);
            Logger.Log("Created a file at " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong while generating the settings file." + e.Message);
            Logger.Log("Something went wrong while generating the settings file." + e.Message);
        }

    }

    /// <summary>
    /// Laden der Einstellungsdatei und Setzen der Faktoren
    /// </summary>
    void LoadSettingsFile()
    {
        SettingsData loadedSettings = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedSettings = JsonUtility.FromJson<SettingsData>(dataToLoad);

                this.plankSize = loadedSettings.plankSize;
                this.plankLength = loadedSettings.plankLength;
                this.distance = loadedSettings.distance;
                this.height = loadedSettings.height;
                this.daylight = loadedSettings.daylight;
                this.raining = loadedSettings.raining;
                this.siren = loadedSettings.siren;
                this.levelOfDifficulty = loadedSettings.levelOfDifficulty;
                this.fogDensity = loadedSettings.fogDensity;
                this.countdown = loadedSettings.countdown;
                this.score = loadedSettings.score;

            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong while loading the settings file. " + e.Message);
                Logger.Log("Something went wrong while loading the settings file. " + e.Message);
            }
        }
    }

}
