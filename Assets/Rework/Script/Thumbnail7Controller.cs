using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thumbnail7Controller : MonoBehaviour
{
    public Transform maleOptionSpawnPoint,
                    femaleOptionSpawnPoint;
    public GameObject optionDisplayObj;
    public GenderDataStructure[] maleGenders,
                        femaleGenders;

    public GameObject seesawObj;
    public Image maledisplayObj, femaledisplayObj;
    public List<MaleFemalePair> matchAnswers;
    public AudioClip[] audioClips;
    public TextMeshProUGUI counterTextObj;
    public GameObject gameOverObj;

    Transform[] _maleoptionSpawnPoints,
                _femaleoptionSpawnPoints;
    bool _maleSelected = false,
            _femaleSelected = false;
    Transform _selectedMaleObj,
                _selectedFemaleObj;
    Vector3 _selectedMaleOrgPos,
                _selectedFemaleOrgPos;
    int answeredQuesCount = 0;
    bool B_Canclick;
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
        DisableClicking();
        MoveCounterUp();
        UpdateCounter();
        GetChildObjs(maleOptionSpawnPoint, ref _maleoptionSpawnPoints);
        GetChildObjs(femaleOptionSpawnPoint, ref _femaleoptionSpawnPoints);
        SpawnOptions(maleOptionSpawnPoint, maleGenders, _maleoptionSpawnPoints);
        SpawnOptions(femaleOptionSpawnPoint, femaleGenders, _femaleoptionSpawnPoints);
#region DataSetter
        Main_Blended.OBJ_main_blended.levelno = 6;
        QAManager.instance.UpdateActivityQuestion();
        qIndex = 0;
        GetData(qIndex);
        // GetAdditionalData();
#endregion
    }

    void GetChildObjs(Transform parentObj, ref Transform[] traformArr)
    {
        int childCount = parentObj.childCount;
        traformArr = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            traformArr[i] = parentObj.GetChild(i);
        }
    }

    void SpawnOptions(Transform parentObj, GenderDataStructure[] spawnedGenders, Transform[] spawnPositions, int index = 0)
    {
        if(index == spawnedGenders.Length) { EnableClicking(); MoveCounterDown(); return; }

        int _childCount = parentObj.childCount;

        var spawnedObj = Instantiate(optionDisplayObj, parentObj);
        spawnedObj.transform.position = Vector3.zero;
        spawnedObj.transform.GetChild(0).GetComponent<Image>().sprite = spawnedGenders[index].genderSprite;
        // spawnedObj.AddComponent<HoverAudio>().clip = GetAuidoClip(spawnedGenders[index].genderCl);
        spawnedObj.AddComponent<HoverAudio>().clip = spawnedGenders[index].genderNameClip;
        spawnedObj.GetComponent<Button>().onClick.AddListener(OnOptionClicked);
        Utilities.Instance.ANIM_Move(spawnedObj.transform, spawnPositions[index].position, callBack: () => {
            spawnedObj.GetComponent<FloatingObject>().enabled = true; 
            SpawnOptions(parentObj, spawnedGenders, spawnPositions, ++index);
        });
    }

    void MoveCounterUp() => Utilities.Instance.ANIM_Move(counterTextObj.transform.parent, counterTextObj.transform.position + (Vector3.up * 2f), 0f);

    void MoveCounterDown() => Utilities.Instance.ANIM_Move(counterTextObj.transform.parent, counterTextObj.transform.position + (Vector3.down * 1.25f));

    void UpdateCounter() => counterTextObj.text = $"{answeredQuesCount}/{maleGenders.Length}";

    void EnableClicking() => B_Canclick = true;

    void DisableClicking() => B_Canclick = false;

    void AssignImageDisplay(Image sourceObj, Image destinationObj)
    {
        destinationObj.sprite = sourceObj.sprite;
        destinationObj.preserveAspect = true;
    }

    MaleFemalePair GetPair(string malePairName)
    {
        int _count = matchAnswers.Count;
        for (int i = 0; i < _count; i++)
        {
            if(malePairName.Contains(matchAnswers[i].maleAnimal))
            {
                return matchAnswers[i];
            }
        }
        return null;
    }

    void MakeChildSad()
    {
        seesawObj.transform.GetChild(0).gameObject.SetActive(false);
        seesawObj.transform.GetChild(1).gameObject.SetActive(false);
        seesawObj.transform.GetChild(2).gameObject.SetActive(true);
        seesawObj.transform.GetChild(3).gameObject.SetActive(true);
    }

    void MakeChildSmile()
    {
        seesawObj.transform.GetChild(0).gameObject.SetActive(true);
        seesawObj.transform.GetChild(1).gameObject.SetActive(true);
        seesawObj.transform.GetChild(2).gameObject.SetActive(false);
        seesawObj.transform.GetChild(3).gameObject.SetActive(false);
    }

    void EvaluateAnswer()
    {
        if(!_maleSelected || !_femaleSelected) return;

        var malePairName = maledisplayObj.sprite.name;
        var feMalePairName = femaledisplayObj.sprite.name;

        var pairObj = GetPair(malePairName);

        PlaySeeSaw(SeesawState.PlaySeeSaw);

        if(feMalePairName.Contains(pairObj.femalePairAnimal))
        {
            Utilities.Instance.ANIM_CorrectScaleEffect(maledisplayObj.transform.parent, callback: () => { ResetSelectedObjs(); EnableClicking(); });
            Utilities.Instance.ANIM_CorrectScaleEffect(femaledisplayObj.transform.parent, callback: MakeChildSmile);

            ScoreManager.instance.RightAnswer(answeredQuesCount, questionID: GetQuestionID(maleGenders.GetGenderName(malePairName)), answerID: GetOptionID(femaleGenders.GetGenderName(feMalePairName)));

            answeredQuesCount++;
            UpdateCounter();
        }else{
            ScoreManager.instance.WrongAnswer(answeredQuesCount, questionID: GetQuestionID(maleGenders.GetGenderName(malePairName)), answerID: GetOptionID(femaleGenders.GetGenderName(feMalePairName)));

            Utilities.Instance.ANIM_WrongEffect(maledisplayObj.transform.parent.GetComponent<Image>(), callback: () => { ReleaseSelectedObjs(); EnableClicking(); });
            Utilities.Instance.ANIM_WrongEffect(femaledisplayObj.transform.parent.GetComponent<Image>(), callback: MakeChildSad);
        }
    }

    void ReleaseSelectedObjs()
    {
        if(_selectedMaleObj != null)
        {
            _selectedMaleObj.gameObject.SetActive(true);
            Utilities.Instance.ANIM_Move(_selectedMaleObj, _selectedMaleOrgPos);
        }

        if(_selectedFemaleObj != null)
        {
            _selectedFemaleObj.gameObject.SetActive(true);
            Utilities.Instance.ANIM_Move(_selectedFemaleObj, _selectedFemaleOrgPos);
        }

        ResetSelectedObjs();
    }

    void ResetSelectedObjs()
    {
        _maleSelected = false;
        _femaleSelected = false;
        _selectedMaleObj = null;
        _selectedFemaleObj = null;
        _selectedMaleOrgPos = Vector3.zero;
        _selectedFemaleOrgPos = Vector3.zero;
        maledisplayObj.gameObject.SetActive(false);
        femaledisplayObj.gameObject.SetActive(false);
    }

    void PlaySeeSaw(SeesawState seesawState)
    {
        switch (seesawState)
        {
            case SeesawState.LowerBoy:
                Utilities.Instance.ANIM_RotateObj(seesawObj.transform, new Vector3(0, 0, 10));
                break;
            case SeesawState.LowerGirl:
                Utilities.Instance.ANIM_RotateObj(seesawObj.transform, new Vector3(0, 0, -10));
                break;
            case SeesawState.PlaySeeSaw:
                Vector3 rotateDirection;
                float rotatedDir = Mathf.Round(seesawObj.transform.eulerAngles.z);

                if(rotatedDir == 10)
                {
                    rotateDirection = new Vector3(0, 0, -10);
                }else{
                    rotateDirection = new Vector3(0, 0, 10);
                }

                Utilities.Instance.ANIM_PlaySeeSaw(seesawObj.transform, rotateDirection, callback: () => {
                    Utilities.Instance.ANIM_RotateObj(seesawObj.transform, Vector3.zero, callback: () => {
                        if(answeredQuesCount == maleGenders.Length) {
                            BlendedOperations.instance.NotifyActivityCompleted();
                            gameOverObj.SetActive(true);
                        }
                    });
                });
                break;
        }
    }

    bool IsMaleObject(string matchSTR)
    {
        string pattern = @"\bMale\w";
        Regex rg = new Regex(pattern);
        return rg.Match(matchSTR).Success;
    }

#region OnButtonClickListener
    public void OnOptionClicked()
    {
        if(!B_Canclick) return;

        var clickedObj = EventSystem.current.currentSelectedGameObject.transform;
        clickedObj.GetComponent<FloatingObject>().enabled = false;
        Image destinationObj = null;

        var parentName = clickedObj.transform.parent.name;

        if(IsMaleObject(parentName))
        {
            if(_selectedMaleObj != null)
                ReleaseSelectedObjs();
            _selectedMaleOrgPos = clickedObj.position;
            _selectedMaleObj = clickedObj;
            _maleSelected = true;
            destinationObj = maledisplayObj;

            PlaySeeSaw(SeesawState.LowerBoy);
        }else{
            if(_selectedFemaleObj != null)
                ReleaseSelectedObjs();
            _selectedFemaleOrgPos = clickedObj.position;
            _selectedFemaleObj = clickedObj;
            _femaleSelected = true;
            destinationObj = femaledisplayObj;

            PlaySeeSaw(SeesawState.LowerGirl);
        }

        if(_maleSelected && _femaleSelected) DisableClicking();

        Utilities.Instance.ANIM_Move(clickedObj, destinationObj.transform.position, callBack: () => {
            AssignImageDisplay(clickedObj.transform.GetChild(0).GetComponent<Image>(), destinationObj);
            destinationObj.gameObject.SetActive(true);
            clickedObj.gameObject.SetActive(false);
            Invoke(nameof(EvaluateAnswer), 0.5f);
            // EvaluateAnswer();
        });
    }
#endregion

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

    enum SeesawState
    {
        LowerBoy,
        LowerGirl,
        PlaySeeSaw
    }

    [Serializable]
    public class MaleFemalePair
    {
        public string maleAnimal;
        public string femalePairAnimal;
    }
}
