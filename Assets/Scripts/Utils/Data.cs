using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Data
{
	private static Data instance;

	public static Data GetInstance(){
		if (instance == null)
			LoadData ();
		return instance;
	}

	public static void SetInstance(Data data){
		instance = data;
		SaveData ();
	}

	public const string DATA_PATH = "/data.pltz";
	public const int LOCKED = 0;
	public const int UNLOCKED = 1;
	public const int WIN_IN_PUSSY = 2;
	public const int WIN_IN_NORMAL = 3;
	/**
	 * [world, level completed(0 locked, 1 rien, 2 en mode pussy, 3 en mode normal)]
	 */
	public int[,] completedLevels = new int[3, 20];
	public Powers[] selectedPowers = new Powers[4];
	public PowerAttribut[] powerAttributs = new PowerAttribut[6];

	public static void LoadData(){
		File.Delete (Application.persistentDataPath + DATA_PATH);
		if (instance != null)
			return;
		if (File.Exists (Application.persistentDataPath + DATA_PATH)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + DATA_PATH, FileMode.Open);
			instance = (Data)bf.Deserialize (file);
			file.Close ();
		} else {
			instance = new Data ();
			for (int i = 0; i < 20; i++) {
				instance.completedLevels [0, i] = 1;
			}
			for (int i = 0; i < 4; i++) {
				instance.selectedPowers [i] = Powers.Null;
			}

			instance.selectedPowers [0] = Powers.Boom;
			instance.selectedPowers [1] = Powers.SlowTime;

			instance.powerAttributs [0] = new PowerAttribut (Powers.Boom, 20f, 1f, 50);
			instance.powerAttributs [1] = new PowerAttribut (Powers.SlowTime, 20f, 3f, 50);
			instance.powerAttributs [2] = new PowerAttribut (Powers.United, 20f, 1f, 50);
			instance.powerAttributs [3] = new PowerAttribut (Powers.Laser, 15f, 1f, 50);
			instance.powerAttributs [4] = new PowerAttribut (Powers.Invincible, 30f, 5f, 50);
			instance.powerAttributs [5] = new PowerAttribut (Powers.Ultimate, 15f, 5f, 50);
			instance.powerAttributs [5].level = 3;
			SaveData ();
		}
	}

	public PowerAttribut GetPowerAttribut(Powers pow){
		for (int i = 0; i < powerAttributs.Length; i++) {
			if (powerAttributs [i].type == pow) {
				return powerAttributs [i];
			}
		}
		return null;
	}

	public static void SaveData(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + DATA_PATH);
		bf.Serialize (file, instance);
		file.Close ();
	}

	public static void CheckForWorldAndPowerUnlocking(){
		//TODO check for power unlocking
		//and ower upgrade
		int allCompletedLevel = 0;
		Data data = GetInstance();
		//unlock worlds
		for (int w = 0; w < WorldSelectBanner.maxWorld; w++) {
			int completedLevel = 0;
			for (int l = 0; l < 20; l++) {
				if (data.completedLevels [w, l] >= 2)
					completedLevel++;
			}
			allCompletedLevel += completedLevel;
			if (completedLevel >= 20 && w != WorldSelectBanner.maxWorld - 1 && data.completedLevels [w + 1, 0] == 0)
				UnlockWorld (w + 1);
		}
		//unlock powers
		if (allCompletedLevel >= 10) {
			UnlockPower (Powers.Boom);
		} else if (allCompletedLevel >= 20) {
			UnlockPower (Powers.SlowTime);
		} else if (allCompletedLevel >= 30) {
			UnlockPower (Powers.United);
		} else if (allCompletedLevel >= 40) {
			UnlockPower (Powers.Laser);
		} else if (allCompletedLevel >= 50) {
			UnlockPower (Powers.Invincible);
		} else if (allCompletedLevel >= 60) {
			UnlockPower (Powers.Ultimate);
		}
		//level up Power
		for (int i = 0; i < data.powerAttributs.Length; i++) {
			if (data.powerAttributs [i].useBeforeLevelUp <= 0) {
				data.powerAttributs [i] = UpgradePower (data.powerAttributs [i]);
			}
		}
		SetInstance (data);
	}

	public static void UnlockWorld(int world){
		Data data = GetInstance ();
		for (int i = 0; i < 20; i++) {
			data.completedLevels [world, i] = 1;
		}
		SetInstance (data);
	}

	public static void UnlockPower(Powers p){
		Data data = GetInstance ();
		for (int i = 0; i < data.powerAttributs.Length; i++) {
			if (data.powerAttributs [i].type == p) {
				data.powerAttributs [i].unlocked = true;
				break;
			}
		}
		SetInstance (data);
	}

	public static PowerAttribut UpgradePower(PowerAttribut pow){
		switch (pow.type) {
		case Powers.Boom:
			switch (pow.level) {
			case 1:
				pow.level = 2;
				pow.cooldownDuration = 18f;
				pow.effectDuration = 2f;
				pow.useBeforeLevelUp = 150;
				break;
			case 2:
				pow.level = 3;
				pow.cooldownDuration = 15f;
				pow.effectDuration = 2f;
				pow.useBeforeLevelUp = -1;
				break;
			}
			break;
		case Powers.SlowTime:
			switch (pow.level) {
			case 1:
				pow.level = 2;
				pow.cooldownDuration = 18f;
				pow.effectDuration = 4f;
				pow.useBeforeLevelUp = 150;
				break;
			case 2:
				pow.level = 3;
				pow.cooldownDuration = 16f;
				pow.effectDuration = 5f;
				pow.useBeforeLevelUp = -1;
				break;
			}
			break;
		case Powers.United:
			switch (pow.level) {
			case 1:
				pow.level = 2;
				pow.cooldownDuration = 18f;
				pow.effectDuration = 3f;
				pow.useBeforeLevelUp = 150;
				break;
			case 2:
				pow.level = 3;
				pow.cooldownDuration = 16f;
				pow.effectDuration = 5f;
				pow.useBeforeLevelUp = -1;
				break;
			}
			break;
		case Powers.Laser:
			switch (pow.level) {
			case 1:
				pow.level = 2;
				pow.cooldownDuration = 13f;
				pow.effectDuration = 4f;
				pow.useBeforeLevelUp = 150;
				break;
			case 2:
				pow.level = 3;
				pow.cooldownDuration = 11f;
				pow.effectDuration = 5f;
				pow.useBeforeLevelUp = -1;
				break;
			}
			break;
		case Powers.Invincible:
			switch (pow.level) {
			case 1:
				pow.level = 2;
				pow.cooldownDuration = 25f;
				pow.effectDuration = 5f;
				pow.useBeforeLevelUp = 150;
				break;
			case 2:
				pow.level = 3;
				pow.cooldownDuration = 20f;
				pow.effectDuration = 5f;
				pow.useBeforeLevelUp = -1;
				break;
			}
			break;
		}
		return pow;
	}
}
