using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public static class TimerAndPopupBuilder
{
    [MenuItem("Basketball/Update HUD with Timer and Popup")]
    public static void UpdateHUD()
    {
        GameObject hudCanvas = GameObject.Find("HUDCanvas");
        if (hudCanvas == null) {
            Debug.LogError("HUDCanvas not found in scene!");
            return;
        }

        HUDController hud = hudCanvas.GetComponent<HUDController>();
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        ScoreManager sm = Object.FindAnyObjectByType<ScoreManager>();

        if (hud == null) {
            Debug.LogError("HUDController not found on HUDCanvas!");
            return;
        }

        // Cleanup old if exists to avoid duplicates
        Transform oldTimer = hudCanvas.transform.Find("TimerText");
        if (oldTimer != null) Object.DestroyImmediate(oldTimer.gameObject);
        
        Transform oldPopup = hudCanvas.transform.Find("GameOverPopup");
        if (oldPopup != null) Object.DestroyImmediate(oldPopup.gameObject);

        // 1. Timer Text
        GameObject timerGO = new GameObject("TimerText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        timerGO.transform.SetParent(hudCanvas.transform, false);
        RectTransform timerRT = timerGO.GetComponent<RectTransform>();
        timerRT.anchorMin = new Vector2(0.5f, 1f);
        timerRT.anchorMax = new Vector2(0.5f, 1f);
        timerRT.pivot = new Vector2(0.5f, 1f);
        timerRT.anchoredPosition = new Vector2(0, -60); // Down a bit from top
        timerRT.sizeDelta = new Vector2(250, 70);

        TextMeshProUGUI timerTMP = timerGO.GetComponent<TextMeshProUGUI>();
        timerTMP.text = "01:00";
        timerTMP.fontSize = 54;
        timerTMP.fontStyle = FontStyles.Bold;
        timerTMP.alignment = TextAlignmentOptions.Center;
        timerTMP.color = Color.white;
        
        // Add shadow for readability
        timerGO.AddComponent<Shadow>().effectDistance = new Vector2(2, -2);

        // 2. Game Over Popup
        GameObject popupRoot = new GameObject("GameOverPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupRoot.transform.SetParent(hudCanvas.transform, false);
        RectTransform rootRT = popupRoot.GetComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;
        popupRoot.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);

        // Final Score Text
        GameObject finalScoreGO = new GameObject("FinalScoreText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        finalScoreGO.transform.SetParent(popupRoot.transform, false);
        RectTransform scoreRT = finalScoreGO.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRT.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRT.pivot = new Vector2(0.5f, 0.5f);
        scoreRT.anchoredPosition = new Vector2(0, 80);
        scoreRT.sizeDelta = new Vector2(800, 150);
        TextMeshProUGUI scoreTMP = finalScoreGO.GetComponent<TextMeshProUGUI>();
        scoreTMP.text = "FINAL SCORE: 0";
        scoreTMP.fontSize = 72;
        scoreTMP.fontStyle = FontStyles.Bold;
        scoreTMP.alignment = TextAlignmentOptions.Center;
        scoreTMP.color = new Color(1f, 0.8f, 0f, 1f); // Golden yellow

        // Replay Button
        GameObject buttonGO = new GameObject("ReplayButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(popupRoot.transform, false);
        RectTransform btnRT = buttonGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0.5f);
        btnRT.anchorMax = new Vector2(0.5f, 0.5f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = new Vector2(0, -80);
        btnRT.sizeDelta = new Vector2(240, 70);
        
        Image btnImg = buttonGO.GetComponent<Image>();
        btnImg.color = new Color(0.15f, 0.7f, 0.15f, 1f); // Nice green

        GameObject btnTextGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        btnTextGO.transform.SetParent(buttonGO.transform, false);
        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;
        
        TextMeshProUGUI btnTMP = btnTextGO.GetComponent<TextMeshProUGUI>();
        btnTMP.text = "REPLAY";
        btnTMP.fontSize = 36;
        btnTMP.fontStyle = FontStyles.Bold;
        btnTMP.color = Color.white;
        btnTMP.alignment = TextAlignmentOptions.Center;

        // 3. Assign References via SerializedObject
        SerializedObject soHUD = new SerializedObject(hud);
        soHUD.FindProperty("_timerText").objectReferenceValue = timerTMP;
        soHUD.FindProperty("_gameOverRoot").objectReferenceValue = popupRoot;
        soHUD.FindProperty("_finalScoreText").objectReferenceValue = scoreTMP;
        soHUD.FindProperty("_replayButton").objectReferenceValue = buttonGO.GetComponent<Button>();
        soHUD.FindProperty("_gameManager").objectReferenceValue = gm;
        soHUD.ApplyModifiedProperties();

        if (gm != null)
        {
            SerializedObject soGM = new SerializedObject(gm);
            soGM.FindProperty("_scoreManager").objectReferenceValue = sm;
            soGM.ApplyModifiedProperties();
        }

        popupRoot.SetActive(false);
        
        // Mark as dirty to ensure save
        EditorUtility.SetDirty(hud);
        if(gm != null) EditorUtility.SetDirty(gm);
        
        Debug.Log("HUD updated with Timer and Popup! Check the HUDCanvas in the scene.");
    }
}