using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public delegate void CREATE_ENEMY_FUNC(TagEnemyStatus enemy_status);

public interface ICommand
{
    void Run();
};

public class CCommandManager
{
    private List<ICommand> Command = new List<ICommand>();
    private CREATE_ENEMY_FUNC[] EnemyFunc =
    {
            CEnemy0.New,
            CEnemy1.New,
            CEnemy2.New,
            CEnemy3.New,
            CEnemy4.New,
            CEnemy5.New,
            CEnemy6.New,
            CEnemy7.New,
            CEnemy8.New,
            CEnemy9.New,
            CEnemy10.New,
    };
    string[] EnemyName =
    {
            "enemy0",
            "enemy1",
            "enemy2",
            "enemy3",
            "enemy4",
            "enemy5",
            "enemy6",
            "enemy7",
            "enemy8",
            "enemy9",
            "enemy10",
     };

    private int CommandIndex { set; get; }
    public float WaitTime { set; get; }
    public void Initalize()
    {
        Command.Clear();
        CommandIndex = 0;
        WaitTime = 0.0f;
    }
    public void Run()
    {
        if (WaitTime > 0)
        {
            WaitTime -= Time.deltaTime;
        }
        else
        {
            WaitTime = 0;
        }

        while (CommandIndex < Command.Count && WaitTime == 0)
        {
            Command[CommandIndex].Run();
            CommandIndex++;
        }
    }
    public bool LoadScript(string file_name)
    {
        CommandIndex = 0;
        WaitTime = 0;

        bool comment = false;
        int cnt = 0; // 行数カウント

        foreach (string line in File.ReadLines(file_name))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.Substring(0, 2) == "//") continue;
            System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
            string[] param = line.Split(new char[] { ',', ' ' }, option);
            if (param.Length <= 0) continue;

            if (param[0] == "*/") comment = false;
            else if (param[0] == "/*") comment = true;
            if (comment) continue;

            List<string> error_string = new List<string>();
            if (param[0] == "enemy")
            {
                if (param.Length < 2)
                {
                    error_string.Add($"{file_name}{cnt}行目のenemyコマンドのパラメーターが足りません");
                    continue;
                }
                for (int j = 0; j < EnemyName.Length; j++)
                {
                    if (param[1] == EnemyName[j])
                    {
                        TagEnemyStatus enemy_status = new TagEnemyStatus();
                        for (int k = 2; k < param.Length - 1; k += 2)
                        {
                            if (param[k] == "obj")
                            {
                                enemy_status.EnemyObj = CGameManager.ResourcesLoader.GetObjectHandle(param[k + 1]);
                            }
                            else if (param[k] == "ex")
                            {
                                // 初期値から最終値の倍率を変更する
                                // num	現在の数値(sa～eaの値を入れる)
                                // sa	変化前の初期値
                                // ea	変化前の最終値
                                // sb	変化後の初期値
                                // eb	変化後の最終値
                                // 使用例 10～100までの変化を0～1までの変化に倍率を変更する
                                // cout << ChangeRate( 10, 10, 100, 0, 1 ) << endl;
                                float sax = 0.0f;
                                float eax = 100.0f;
                                float sbx = -6.5f;
                                float ebx = 2.5f;
                                //print(CUtility.ChangeRate(100, sax, eax, sbx, ebx));
                                enemy_status.X = CUtility.ChangeRate(float.Parse(param[k + 1]), sax, eax, sbx, ebx);
                            }
                            else if (param[k] == "ey")
                            {
                                float say = 0.0f;
                                float eay = 100.0f;
                                float sby = 6.0f;
                                float eby = -5.0f;
                                //print(CUtility.ChangeRate(100, say, eay, sby, eby));
                                enemy_status.Y = CUtility.ChangeRate(float.Parse(param[k + 1]), say, eay, sby, eby);
                            }
                            else if (param[k] == "life")
                            {
                                enemy_status.Life = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "w_time")
                            {
                                enemy_status.WaitTime = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "bl_pat")
                            {
                                enemy_status.BulletPattern = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "bl_type")
                            {
                                enemy_status.BulletType = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "bl_col")
                            {
                                enemy_status.BulletColor = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "bl_int")
                            {
                                enemy_status.BulletInterval = int.Parse(param[k + 1]);
                            }
                            else if (param[k] == "gr_type")
                            {
                                enemy_status.GraphType = int.Parse(param[k + 1]);
                            }

                        }
                        Command.Add(new CEnemyCreateCommand(EnemyFunc[j], enemy_status));
                    }
                }
            }
            else if (param[0] == "wait")
            {
                if (param.Length < 2)
                {
                    error_string.Add($"{file_name}{cnt}行目のwaitコマンドのパラメーターが足りません");
                    continue;
                }
                Command.Add(new CWaitCommand(this, float.Parse(param[1])));
            }
            else if (param[0] == "play")
            {
                if (param.Length < 2)
                {
                    error_string.Add($"{file_name}{cnt}行目のwaitコマンドのパラメーターが足りません");
                    continue;
                }
                Command.Add(new CPlayCommand(param[1]));
            }
            else if (param[0] == "fadeout")
            {
                if (param.Length < 4)
                {
                    error_string.Add($"{file_name}{cnt}行目のwaitコマンドのパラメーターが足りません");
                    continue;
                }
                float start_val = float.Parse(param[1]);
                float end_val = float.Parse(param[2]);
                float end_time = float.Parse(param[3]);

                Command.Add(new CFadeOutCommand(start_val, end_val, end_time));
            }
        }
        return true;
    }
}

public class CEnemyCreateCommand : ICommand
{
    private TagEnemyStatus EnemyStatus;
    CREATE_ENEMY_FUNC CreateEnemyFunc;
    public CEnemyCreateCommand(CREATE_ENEMY_FUNC func, TagEnemyStatus enemy_status)
    {
        CreateEnemyFunc = func;
        EnemyStatus = enemy_status;
    }

    public void Run()
    {
        CreateEnemyFunc(EnemyStatus);
    }
};

public class CWaitCommand : ICommand
{
    private CCommandManager CommandManager;
    private float WaitTime;
    public CWaitCommand(CCommandManager game_control, float wait_time)
    {
        CommandManager = game_control;
        WaitTime = wait_time;
    }

    public void Run()
    {
        CommandManager.WaitTime = WaitTime;
    }
};

public class CPlayCommand : ICommand
{
    string BGMName;
    public CPlayCommand(string bgm_name)
    {
        BGMName = bgm_name;
    }
    public void Run()
    {
        CSoundPlayer.PlaySound(BGMName, false);
    }
};

public class CFadeOutCommand : MonoBehaviour, ICommand
{
    float StartVal, EndVal, EndTime;
    public CFadeOutCommand(float start_val, float end_val, float end_time)
    {
        StartVal = start_val;
        EndVal = end_val;
        EndTime = end_time;
    }
    public void Run()
    {
        //fadeout 1 0 5
        CSoundPlayer.CallSetFadeTimer(StartVal, EndVal, EndTime);
    }
}