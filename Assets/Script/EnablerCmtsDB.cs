using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EnablerCmtsDB
{
    public string welcome;
    public string introduction;
    public string brain_gym_1;
    public string gender_name_humans;
    public string gender_name_animals;
    public string commongender;
    public string activity_1;
    public string brain_gym_2;
    public string activity_2;
    public string goodbye;

    public EnablerCmtsDB()
    {
        welcome = Main_Blended.OBJ_main_blended.enablerComments[0];
        introduction = Main_Blended.OBJ_main_blended.enablerComments[1];
        brain_gym_1 = Main_Blended.OBJ_main_blended.enablerComments[2];
        gender_name_humans = Main_Blended.OBJ_main_blended.enablerComments[3];
        gender_name_animals = Main_Blended.OBJ_main_blended.enablerComments[4];
        commongender = Main_Blended.OBJ_main_blended.enablerComments[5];
        activity_1 = Main_Blended.OBJ_main_blended.enablerComments[6];
        brain_gym_2 = Main_Blended.OBJ_main_blended.enablerComments[7];
        activity_2 = Main_Blended.OBJ_main_blended.enablerComments[8];
        goodbye = Main_Blended.OBJ_main_blended.enablerComments[9];

    }
}