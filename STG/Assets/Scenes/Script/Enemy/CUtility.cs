using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 便利な関数を管理する静的クラス
public static class CUtility
{
	// 移動可能な範囲
	static Vector2 MoveLimitTopLeft = new Vector2(-5.7f, 4.2f);
	static Vector2 MoveLimitButtomRight = new Vector2(1.7f, -4.2f);

	static Vector2 EnemyLimitTopLeft = new Vector2(-7.0f, 6.2f);
	static Vector2 EnemyLimitButtomRight = new Vector2(3.0f, -5.2f);
	// 指定された位置を移動可能な範囲に収めた値を返す
	public static Vector3 ClampPosition(Vector3 position)
	{
		return new Vector3
		(
			Mathf.Clamp(position.x, MoveLimitTopLeft.x, MoveLimitButtomRight.x),
			Mathf.Clamp(position.y, MoveLimitButtomRight.y, MoveLimitTopLeft.y),
			0
		);
	}
	public static bool IsOut(Vector3 position)
	{
		return (position.x < EnemyLimitTopLeft.x ||
			   EnemyLimitTopLeft.y < position.y ||
				EnemyLimitButtomRight.x < position.x ||
				position.y < EnemyLimitButtomRight.y);
	}
	// 引数に角度（ 0 ～ 360 ）を渡すとベクトルに変換して返す
	public static Vector3 GetDirection360(float angle360)
	{
		float angle = angle360 * Mathf.Deg2Rad;
		return new Vector3
		(
			Mathf.Cos(angle),
			Mathf.Sin(angle),
			0
		);
	}
	// 引数に角度（ 0 ～ 1.0f ）を渡すとベクトルに変換して返す
	public static Vector3 GetDirection1(float angle1)
	{
		float angle = angle1 / (3.14f * 2);
		return new Vector3
		(
			Mathf.Cos(angle),
			Mathf.Sin(angle),
			0
		);
	}
	// 引数に角度（ 0 ～ PI2 ）を渡すベクトルに変換して返す
	public static Vector3 GetDirectionPI2(float angle_pi2)
	{
		return new Vector3
		(
			Mathf.Cos(angle_pi2),
			Mathf.Sin(angle_pi2),
			0
		);
	}
	//【機能】マルチプルスプライトからスライスしたスプライトを取得する
	//【第1引数】スプライトファイル名（正確にはResources フォルダからのスプライトファイルまでのパス）
	//【第2引数】取得したいスライスされたスプライト名
	//【戻り値】取得したスプライト
	public static Sprite GetSprite(string fileName, string spriteName)
	{
		Sprite[] sprites = Resources.LoadAll<Sprite>(fileName);
		return System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals(spriteName));
	}
	//渡された-ang～angまでのランダムな角度を返す
	public static float Rang(float ang)
	{
		return (-ang + ang * 2.0f * (Random.value * 10000.0f) / 10000.0f);
	}
	// 初期値から最終値の倍率を変更する
	// num	現在の数値(sa～eaの値を入れる)
	// sa	変化前の初期値
	// ea	変化前の最終値
	// sb	変化後の初期値
	// eb	変化後の最終値
	// 使用例 10～100までの変化を0～1までの変化に倍率を変更する
	// cout << ChangeRate( 10, 10, 100, 0, 1 ) << endl;
	static public float ChangeRate(float num, float sa, float ea, float sb, float eb)
	{
		num = Mathf.Clamp(num, sa, ea);
		float a = (sb - eb) / (sa - ea);
		float b = sb - sa * a;
		return a * num + b;
	}
}
public class CResourcesLoader<T> where T : UnityEngine.Object
{
	Dictionary<string, T> ResourcesHandles = new Dictionary<string, T>();

	public bool LoadObject(string file_name)
	{
		string fn = Path.GetFileName(file_name);

		if (ResourcesHandles.ContainsKey(fn))
		{
			Debug.Log("LoadObject()で同じ名前のキーがありました");
			Debug.Log(file_name + "にある" + fn + "の名前を変更してください");
			return false;
		}
		else
		{
			T ob = Resources.Load<T>(file_name);
			if (ob)
			{
				ResourcesHandles.Add(fn, ob);
				return true;
			}
			else
			{
				Debug.Log("LoadObject()" + file_name + "の読み込みが失敗しました");
				return false;
			}
		}
	}

	public bool LoadAllObjects(string file_name)
	{
		T[] obs = Resources.LoadAll<T>(file_name);

		if (obs.Length <= 0)
		{
			return false;
		}

		foreach (T ob in obs)
		{
			if (ResourcesHandles.ContainsKey(ob.name))
			{
				Debug.Log("LoadAllObject()で同じ名前のキーがありました");
				Debug.Log(file_name + "にある" + ob.name + "の名前を変更してください");
			}
			else
			{
				ResourcesHandles.Add(ob.name, ob);
			}
		}
		return true;
	}

	public T GetObjectHandle(string name)
	{
		if (ResourcesHandles.ContainsKey(name))
		{
			return ResourcesHandles[name];
		}
		else
		{

			return null;
		}
	}

	public bool ContainsKey(string key_name)
	{
		return ResourcesHandles.ContainsKey(key_name);
	}
}

public class CSoundPlayer : MonoBehaviour
{
	static GameObject SoundPlayerObj, BGMPlayerObj;
	static CResourcesLoader<AudioClip> ResourcesLoader = new CResourcesLoader<AudioClip>();
	static AudioSource SoundAudioSource;
	static AudioSource BGMAudioSource;
	static CFadeTimer FadeTimer;

	public static void CallSetFadeTimer(float start_val, float end_val, float end_time)
	{
		//AddComponentでオブジェクトを生成
		CSoundPlayer sound_player = (new GameObject("sound_player_obj")).AddComponent<CSoundPlayer>();
		//コルーチン呼び出し（この場合は正常に実行できる）
		sound_player.StartCoroutine(SetFadeTimer(start_val, end_val, end_time));
	}
	public static IEnumerator SetFadeTimer(float start_val, float end_val, float end_time)
	{
		FadeTimer = new CFadeTimer(start_val, end_val, end_time);

		while (true)
		{
			float t = FadeTimer.CalcTime();
			BGMAudioSource.volume = t;

			if (t <= 0.0f)
			{
				GameObject sound_player_obj = GameObject.Find("sound_player_obj");
				if (sound_player_obj != null)
				{
					Destroy(sound_player_obj);
				}
				BGMAudioSource.Stop();
				yield break;
			}
			else
			{
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
	}

	public static bool LoadAudioClip(string audio_path)
	{
		return ResourcesLoader.LoadObject(audio_path);
	}

	public static bool LoadAllSounds(string audio_path)
	{
		return ResourcesLoader.LoadAllObjects(audio_path);
	}

	public static bool StopSound(string se_name)
	{
		GameObject sound_obj = GameObject.Find(se_name);
		if (sound_obj)
		{
			AudioSource aus = sound_obj.GetComponent<AudioSource>();
			if (aus)
			{
				aus.Stop();
				return true;
			}
		}
		return false;
	}

	public static AudioClip GetAudioClip(string se_name)
	{
		if (ResourcesLoader.ContainsKey(se_name) == false)
		{
			return null;
		}
		else
		{
			return ResourcesLoader.GetObjectHandle(se_name);
		}
	}

	public static bool PlaySound(string se_name, bool se_flag = true)
	{
		if (ResourcesLoader.ContainsKey(se_name) == false)
		{
			return false;
		}

		if (se_flag)
		{
			if (SoundPlayerObj == null)
			{
				SoundPlayerObj = new GameObject("SoundPlayer");
				SoundAudioSource = SoundPlayerObj.AddComponent<AudioSource>();
			}
			SoundAudioSource.PlayOneShot(ResourcesLoader.GetObjectHandle(se_name));
		}
		else
		{
			if (BGMPlayerObj == null)
			{
				BGMPlayerObj = new GameObject("BGMPlayer");
				BGMAudioSource = BGMPlayerObj.AddComponent<AudioSource>();
				BGMAudioSource.clip = ResourcesLoader.GetObjectHandle(se_name);
				BGMAudioSource.volume = 1.0f;
				BGMAudioSource.loop = true;
				BGMAudioSource.Play();
			}
			else
			{
				if (BGMAudioSource)
				{
					if (BGMAudioSource.isPlaying)
					{
						BGMAudioSource.Stop();
					}
					else
					{
						BGMAudioSource.Play();
					}
				}
				else
				{
					BGMAudioSource = BGMPlayerObj.AddComponent<AudioSource>();
					if (BGMAudioSource)
					{
						BGMAudioSource.clip = ResourcesLoader.GetObjectHandle(se_name);
						BGMAudioSource.volume = 1.0f;
						BGMAudioSource.loop = true;
						BGMAudioSource.Play();
					}
				}
			}
		}

		return true;
	}
}

// 指定した時間である値からある値まで数を変化させる
// start_val 変化させたい数の初期値
// end_val	 変化させたい数の終了値
// end_time  終了時間(end_timeかけて終了させる)
// 使用手順
// 1 宣言する
// CFadeTimer FadeTimer;
// 2 初期化する
// FadeTimer = new CFadeTimer( 100, 1000, 3 );
// 3 毎ループ行う処理を追加する
// print( FadeTimer.CalcTime () );
public class CFadeTimer
{
	private bool Flag = true;
	private float StartVal, EndVal, StartTime, EndTime, Delta, Result = 0.0f;

	public CFadeTimer(float start_val, float end_val, float end_time)
	{
		StartVal = start_val;
		EndVal = end_val;
		EndTime = end_time;
		StartTime = Time.realtimeSinceStartup;
		Delta = (EndVal - StartVal) / EndTime;
	}

	public float CalcTime()
	{
		if (Flag)
		{
			float t = Time.realtimeSinceStartup - StartTime;
			if (EndTime <= t)
			{
				Flag = false;
				Result = EndVal;
				return Result;
			}
			Result = Delta * t + StartVal;
			return Result;
		}
		else
		{
			return -1.0f;
		}
	}
}
