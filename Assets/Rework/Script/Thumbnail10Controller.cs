using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Thumbnail10Controller : MonoBehaviour
{
    public Transform questionPanel;
    public Transform panel1, panel2;
    public QuestionOptionData[] questionData;
    public TextMeshProUGUI questionTextDisplay,
                            option1Text,
                            option2Text;
    public TextMeshProUGUI counterText;
    public GameObject activityCompleted;
    public AudioClip wrongClip;
    int currentIndex = 0;
#region QA
    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public bool isActivityCompleted = false;
    public Dictionary<string, Component> additionalFields;
    // Component[] questions;
    Component question;
    Component[] options;
    Component[] answers;
#endregion

    void Start()
    {
        ResetQuestionPanel();
        ShowQuestionPanel();
#region DataSetter
        Main_Blended.OBJ_main_blended.levelno = 9;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(currentIndex);
        // GetAdditionalData();
#endregion
    }

    void ResetQuestionPanel(float resetTime = 0f, Action callback = null)
    {
        Utilities.Instance.ANIM_ShrinkOnPosition(questionPanel, new Vector3(1, 0, 1), resetTime, callback: () => {
            callback?.Invoke();
        });
        Utilities.Instance.ANIM_RotateObj(panel1, new Vector3(90, 0, 0), resetTime);
        Utilities.Instance.ANIM_RotateObj(panel2, new Vector3(90, 0, 0), resetTime);
    }

    void ShowQuestionPanel()
    {
        UpdateCounter();
        questionTextDisplay.text = questionData[currentIndex].question;
        option1Text.text = questionData[currentIndex].options[0].optionSTR;
        option2Text.text = questionData[currentIndex].options[1].optionSTR;

        Utilities.Instance.ANIM_BounceEffect(questionPanel, callback : () => {
            Utilities.Instance.ANIM_BoardHangingEffect(panel1);
            Utilities.Instance.ANIM_BoardHangingEffect(panel2, callback: () => {
                AudioManager.PlayAudio(questionData[currentIndex].questionAudioClip);
            });
        });
    }

    void UpdateCounter() => counterText.text = $"{currentIndex + 1}/{questionData.Length}";

    void DisplayRightAnswer(string rightOption)
    {
        questionTextDisplay.text = Regex.Replace(questionData[currentIndex].question, "_+", $"<u>{rightOption}</u>");
    }

    bool IsRightAnswer(string selectedOptionSTR)
    {
        return questionData[currentIndex].CheckRightAnswer(selectedOptionSTR);
    }

    void ChangeQuestion()
    {
        currentIndex++;
        if(currentIndex >= questionData.Length) { ActivityCompleted(); return; } 
        GetData(currentIndex);
        ResetQuestionPanel(0.5f, ShowQuestionPanel);
    }

    void ActivityCompleted()
    {
        BlendedOperations.instance.NotifyActivityCompleted();
        activityCompleted.SetActive(true);
    }

#region LISTENERS

    public void OnOptionClick()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;
        string selectedOptionSTR = selectedObj.transform.GetComponentInChildren<TextMeshProUGUI>().text;
        if(IsRightAnswer(selectedOptionSTR))
        {
            ScoreManager.instance.RightAnswer(currentIndex, questionID: question.id, answerID: GetOptionID(selectedOptionSTR));
            DisplayRightAnswer(selectedOptionSTR);
            Invoke(nameof(ChangeQuestion), 1.5f);
        }else{
            ScoreManager.instance.WrongAnswer(currentIndex, questionID: question.id, answerID: GetOptionID(selectedOptionSTR));
            AudioManager.PlayAudio(wrongClip);
        }
    }

#endregion

#region QA

    int GetOptionID(string selectedOption)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Debug.Log($"Option : {options[i].text}");
            if (options[i].text.Contains(selectedOption))
            {
                return options[i].id;
            }
        }
        return -1;
    }

    void GetData(int questionIndex)
    {
        question = QAManager.instance.GetQuestionAt(0, questionIndex);
        // questions = QAManager.instance.GetQuestionAt(0);
        options = QAManager.instance.GetOption(0, questionIndex);
        answers = QAManager.instance.GetAnswer(0, questionIndex);
    }

    void GetAdditionalData()
    {
        additionalFields = QAManager.instance.GetAdditionalField(0);
    }
#endregion
}

[System.Serializable]
public class QuestionOptionData
{
    public string question;
    public AudioClip questionAudioClip;
    public OptionData[] options;

    public bool CheckRightAnswer(string selectedOption)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if(options[i].optionSTR == selectedOption)
            {
                AudioManager.PlayAudio(options[i].optionClip);
                return options[i].isRight;
            } 
        }
        return false;
    }
}

[System.Serializable]
public class OptionData
{
    public string optionSTR;
    public AudioClip optionClip;
    public bool isRight;
}
