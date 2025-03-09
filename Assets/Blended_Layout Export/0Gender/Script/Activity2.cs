using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Activity2 : MonoBehaviour
{
    public GenderDataStructure[] genderData;
    public GameObject G_final;
    public Button nextBTN;
    public Image questionDisplayIMG;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI counterText;
    public AudioClip rightSFX, wrongSFX;
    GameObject dummy;
    int I_Qcount;
    bool B_Canclick;
    Vector3 questionTextPanelPosition;

    void Start()
    {
        I_Qcount = -1;
        questionTextPanelPosition = questionText.transform.parent.position;
        ShowQuestion();
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
            G_final.SetActive(true);
            return;
        }

        questionDisplayIMG.sprite = genderData[I_Qcount].genderSprite;
        questionText.text = genderData[I_Qcount].genderName;

        Utilities.Instance.ANIM_Move(questionText.transform.parent, questionTextPanelPosition, callBack: () => {
            AudioManager.PlayAudio(genderData[I_Qcount].genderNameClip);
        });
        Utilities.Instance.ANIM_ShowNormal(questionDisplayIMG.transform);
    }

    bool EvaluateAnswer(Transform clickedObj, string answerSTR) {
        Debug.Log(genderData[I_Qcount].gender);
        switch(clickedObj.GetComponent<TextMeshProUGUI>().text)
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

        dummy = EventSystem.current.currentSelectedGameObject;
        int childCount = dummy.transform.childCount;

        DisableClicking();

        bool answerEvaluated = EvaluateAnswer(dummy.transform.GetChild(childCount - 1), genderData[I_Qcount].gender.ToString());

        if (answerEvaluated)
        {
            dummy.transform.GetChild(0).gameObject.SetActive(true);
            dummy.transform.GetChild(1).gameObject.SetActive(true);

            dummy.transform.GetChild(0).GetComponent<Image>().color = Color.green;
            dummy.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            AudioManager.PlayAudio(rightSFX);

            nextBTN.interactable = true;
        } else {
            dummy.transform.GetChild(0).gameObject.SetActive(true);
            dummy.transform.GetChild(1).gameObject.SetActive(true);

            dummy.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            dummy.transform.GetChild(1).GetComponent<Image>().color = Color.red;
            AudioManager.PlayAudio(wrongSFX);

            Invoke(nameof(THI_normal), 1f);
        }
    }

    public void THI_normal()
    {
        dummy.transform.GetChild(0).gameObject.SetActive(false);
        dummy.transform.GetChild(1).gameObject.SetActive(false);

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
}
