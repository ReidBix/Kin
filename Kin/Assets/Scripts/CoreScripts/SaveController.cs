﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SaveController : MonoBehaviour {
    //on enable on disable for autosave

    public static SaveController s_instance;
	public GameObject Player;
	public GameObject Health;
	public GameObject Stamina;

    public long day;
    public float time;

    // Use this for initialization
    void Awake()
    {
        if (s_instance == null)
        {
            DontDestroyOnLoad(gameObject); // save object on scene mvm
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
		if (PreLoader.Instance != null) {
			if (PreLoader.Instance.resume) {
				Load (PreLoader.Instance.fileNumber, PreLoader.Instance.autosave);
			}
		}
    }
	
    void OnGUI()
    {
        if(GUI.Button(new Rect(10, 60, 100, 30), "Save"))
        {
            Save("", false);
        }
        if(GUI.Button(new Rect(10, 90, 100, 30), "Load"))
        {
            Load("", false);
        }
    }

	public void Save(String fileNumber, bool autosave)
    {
		if (autosave) {
			fileNumber = "autosave" + fileNumber;
		}
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveInfo" + fileNumber + ".dat");
        SaveData data = WriteToData();
        bf.Serialize(file, data);
        file.Close();
    }

	public void Load(String fileNumber, bool autosave)
    {
		if (autosave) {
			fileNumber = "autosave" + fileNumber;
		}
		if(File.Exists(Application.persistentDataPath + "/saveInfo" + fileNumber + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saveInfo" + fileNumber + ".dat",
                FileMode.Open);
            SaveData data = (SaveData) bf.Deserialize(file);
            file.Close();

			WriteFromData (data);
        }
    }

	private void WriteFromData(SaveData data)
	{
		Health.GetComponent<Slider>().value = data.health;
		Stamina.GetComponent<Slider>().value = data.stamina;
		Player.GetComponent<StatController>().setHealth(data.healthLvlP);
		Player.GetComponent<StatController>().setHealthOrder(data.healthLvlO);
		Player.GetComponent<StatController>().setStamina(data.stamLvlP);
		Player.GetComponent<StatController>().setStaminaOrder(data.stamLvlO);
		Player.GetComponent<StatController>().setStrength(data.strLvlP);
		Player.GetComponent<StatController>().setStrengthOrder(data.strLvlO);
		Player.GetComponent<StatController>().setWisdom(data.wisLvlP);
		Player.GetComponent<StatController>().setWisdomOrder(data.wisLvlO);
		day = data.day;
		time = data.time;
	}

    private SaveData WriteToData ()
    {
        SaveData data = new SaveData();
		data.health = Health.GetComponent<Slider>().value;
		data.stamina = Stamina.GetComponent<Slider>().value;
		data.healthLvlP = Player.GetComponent<StatController>().getHealth();
		data.healthLvlO = Player.GetComponent<StatController>().getHealthOrder();
		data.stamLvlP = Player.GetComponent<StatController>().getStamina();
		data.stamLvlO = Player.GetComponent<StatController>().getStaminaOrder();
		data.strLvlP = Player.GetComponent<StatController>().getStrength();
		data.strLvlO = Player.GetComponent<StatController>().getStrengthOrder();
		data.wisLvlP = Player.GetComponent<StatController>().getWisdom();
		data.wisLvlO = Player.GetComponent<StatController>().getWisdomOrder();
        data.day = day;
        data.time = time;

        return data;
    }
}

[Serializable]
class SaveData 
{
    public float health;
    public float stamina;

    public int healthLvlP;
    public int healthLvlO;
    public int stamLvlP;    //stamina
    public int stamLvlO;
    public int strLvlP;     //strength
    public int strLvlO;
    public int wisLvlP;     //wisdom
    public int wisLvlO;

    public long day;
    public float time;
}
