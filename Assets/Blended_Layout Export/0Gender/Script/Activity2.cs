using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class Activity2 : MonoBehaviour
{
    public GenderDataStructure[] genderData;
    public GameObject G_final;
    public Button nextBTN;
    public Image questionDisplayIMG;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI counterText;
    public AudioClip rightSFX, wrongSFX;
    GameObject selectedOption;
    int I_Qcount;
    bool B_Canclick;
    Vector3 questionTextPanelPosition;
#region QA
    private int qIndex;
    public GameObject questionGO;
    public GameObject[] optionsGO;
    public bool isActivityCompleted = false;
    public Dictionary<string, Component> additionalFields;
    Component[] questions;
    Component[] options;
    Component[] answers;
#endregion

    void Start()
    {
        I_Qcount = -1;
        questionTextPanelPosition = questionText.transform.parent.position;
        ShowQuestion();
#region DataSetter
        Main_Blended.OBJ_main_blended.levelno = 8;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        // GetAdditionalData();
#endregion
    }

    void EnableClicking() => B_Canclick = true;
    void DisableClicking() => B_Canclick = false;
    void UpdateCounter() => counterText.text = $"{I_Qcount + 1}/{genderData.Length}";

    public void ShowQuestion()
    {
        I_Qcount++;
        UpdateCounter();
        EnableClicking();
        nextBTN.interactable = false;

        if (I_Qcount >= genderData.Length)
        {
            BlendedOperations.instance.NotifyActivityCompleted();
            G_final.SetActive(true);
            return;
        }

        questionDisplayIMG.sprite = genderData[I_Qcount].genderSprite;
        questionText.text = genderData[I_Qcount].genderName;
        questionText.transform.parent.GetComponent<HoverAudio>().clip = genderData[I_Qcount].genderNameClip;

        Utilities.Instance.ANIM_Move(questionText.transform.parent, questionTextPanelPosition, callBack: () => {
            AudioManager.PlayAudio(genderData[I_Qcount].genderNameClip);
        });
        Utilities.Instance.ANIM_ShowNormal(questionDisplayIMG.transform);
    }

    bool EvaluateAnswer(string selectedOption) {
        // Debug.Log(genderData[I_Qcount].gender);
        switch(selectedOption)
        {
            case "Male":
                return genderData[I_Qcount].gender == Genders.Masculine;
            case "Female":
                return genderData[I_Qcount].gender == Genders.Feminine;
            case "Common":
                return genderData[I_Qcount].gender == Genders.Common;
            default:
                return false;
        }
    }

    public void BUT_clicking()
    {
        if(!B_Canclick) return;

        selectedOption = EventSystem.current.currentSelectedGameObject;
        int childCount = selectedOption.transform.childCount;
        var selOptTxt = selectedOption.transform.GetComponentInChildren<TextMeshProUGUI>().text;

        DisableClicking();

        bool answerEvaluated = EvaluateAnswer(selOptTxt);

        if (answerEvaluated)
        {
            selectedOption.transform.GetChild(0).gameObject.SetActive(true);
            selectedOption.transform.GetChild(1).gameObject.SetActive(true);

            selectedOption.transform.GetChild(0).GetComponent<Image>().color = Color.green;
            selectedOption.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            AudioManager.PlayAudio(rightSFX);

            ScoreManager.instance.RightAnswer(I_Qcount, questionID: GetQuestionID(genderData[I_Qcount].genderName), answerID: GetOptionID(selOptTxt));

            nextBTN.interactable = true;
        } else {
            selectedOption.transform.GetChild(0).gameObject.SetActive(true);
            selectedOption.transform.GetChild(1).gameObject.SetActive(true);

            selectedOption.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            selectedOption.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            AudioManager.PlayAudio(wrongSFX);

            ScoreManager.instance.WrongAnswer(I_Qcount, questionID: GetQuestionID(genderData[I_Qcount].genderName), answerID: GetOptionID(selOptTxt));

            Invoke(nameof(THI_normal), 1f);
        }
    }

    public void THI_normal()
    {
        selectedOption.transform.GetChild(0).gameObject.SetActive(false);
        selectedOption.transform.GetChild(1).gameObject.SetActive(false);

        EnableClicking();
    }

    public void BUT_next()
    {
        Utilities.Instance.ANIM_Move(questionText.transform.parent, questionText.transform.position + (Vector3.up * 2f));
        Utilities.Instance.ANIM_ShrinkObject(questionDisplayIMG.transform, callback: () => {
            THI_normal();
            ShowQuestion();
        });
    }
#region QA

    int GetQuestionID(string selectedQues)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i].text.Contains(selectedQues))
            {
                return questions[i].id;
            }
        }
        return -1;
    }

    int GetOptionID(string selectedOption)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].text.Contains(selectedOption))
            {
                return options[i].id;
            }
        }
        return -1;
    }

    void GetData(int questionIndex)
    {
        // question = QAManager.instance.GetQuestionAt(0, questionIndex);
        questions = QAManager.instance.GetAllQuestions(0);
        options = QAManager.instance.GetOption(0, questionIndex);
        answers = QAManager.instance.GetAnswer(0, questionIndex);
    }

    void GetAdditionalData()
    {
        additionalFields = QAManager.instance.GetAdditionalField(0);
    }
#endregion
}
