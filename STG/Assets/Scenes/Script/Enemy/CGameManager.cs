using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameManager : MonoBehaviour
{
    public static CResourcesLoader<GameObject> ResourcesLoader = new CResourcesLoader<GameObject>();
    public static GameObject GetObjectHandle(string obj_name)
    {
        return ResourcesLoader.GetObjectHandle(obj_name);
    }
    public class CBulletFactory
    {
        public GameObject Bullet;
        public float Radius;
        public string[] SpriteName;
        public Sprite[] BulletSprite;
        public bool ColliderType;
        float SizeX, SizeY;
        public CBulletFactory(string[] sprite_name, bool collider_type, float radius = 0.5f, float size_x = 0.5f, float size_y = 0.5f)
        {
            Radius = radius;
            SpriteName = sprite_name;
            BulletSprite = new Sprite[SpriteName.Length];
            ColliderType = collider_type;
            SizeX = size_x;
            SizeY = size_y;
        }
        public void Load()
        {
            for (int i = 0; i < SpriteName.Length - 1; ++i)
            {
                BulletSprite[i] = CUtility.GetSprite(SpriteName[0], SpriteName[i + 1]);
            }
        }
        public void CreateBullet(Vector3 pos, int color, int script_type, float speed, float angle)
        {
            GameObject newParent = new GameObject("Empty");
            Bullet = Instantiate(newParent, pos, Quaternion.identity);
            Bullet.tag = "Bullet";
            SpriteRenderer sr = Bullet.AddComponent<SpriteRenderer>();
            sr.sprite = BulletSprite[color];
            sr.sortingLayerName = "Bullet";
            Bullet.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            switch (script_type)
            {
                case 0:
                    Bullet.AddComponent<CBullet>().SetParam(speed, angle);
                    break;
            }
            if (ColliderType)
            {
                CircleCollider2D cc = Bullet.AddComponent<CircleCollider2D>();//.radius = SizeX;
                cc.radius = Radius;
                cc.isTrigger = true;
            }
            else
            {
                BoxCollider2D bc = Bullet.AddComponent<BoxCollider2D>();//.size = new Vector2(SizeX, SizeY);
                bc.size = new Vector2(SizeX, SizeY);
                bc.isTrigger = true;
            }
            Destroy(newParent);
        }
    }
    public CBulletFactory[] BulletFactory = new CBulletFactory[]
    {
            new CBulletFactory(new string[]{ "img/bullet/b0","b0_0","b0_1","b0_2","b0_3","b0_4" }, true),
            new CBulletFactory(new string[]{ "img/bullet/b1","b1_0","b1_1","b1_2","b1_3","b1_4","b1_5" },true),
            new CBulletFactory(new string[]{ "img/bullet/b2","b2_0","b2_1" ,"b2_2" ,"b2_3" ,"b2_4" ,"b2_5" ,"b2_6" ,"b2_7" ,"b2_8" ,"b2_9"},false),
            new CBulletFactory(new string[]{ "img/bullet/b3","b3_0","b3_1" ,"b3_2" ,"b3_3" ,"b3_4" },false),
            new CBulletFactory(new string[]{ "img/bullet/b4","b4_0","b4_1" ,"b4_2" ,"b4_3" ,"b4_4" ,"b4_5" ,"b4_6" ,"b4_7" ,"b4_8" ,"b4_9"},true),
            new CBulletFactory(new string[]{ "img/bullet/b5","b5_0","b5_1" ,"b5_2" },false),
            new CBulletFactory(new string[]{ "img/bullet/b6","b6_0","b6_1" ,"b6_2"},false),
            new CBulletFactory(new string[]{ "img/bullet/b7","b7_0","b7_1" ,"b7_2" ,"b7_3" ,"b7_4" ,"b7_5" ,"b7_6" ,"b7_7" ,"b7_8" ,"b7_9" },true),
            new CBulletFactory(new string[]{ "img/bullet/b8","b8_0","b8_1" ,"b8_2" ,"b8_3" ,"b8_4" ,"b8_5" ,"b8_6" ,"b8_7" ,"b8_8" },false),
            new CBulletFactory(new string[]{ "img/bullet/b9","b9_0","b9_1" ,"b9_2" },false),
            new CBulletFactory(new string[]{ "img/bullet/b10","b10_0","b10_1" ,"b10_2" ,"b10_3" ,"b10_4" ,"b10_5" ,"b10_6" ,"10_7" },true),
            new CBulletFactory(new string[]{ "img/bullet/b11","b11_0","b11_1" ,"b11_2" ,"b11_3" ,"b11_4" ,"b11_5" ,"b11_6" ,"11_7" },true),
            new CBulletFactory(new string[]{ "img/bullet/b12","b12_0","b12_1" ,"b12_2" ,"b12_3" ,"b12_4" ,"b12_5" ,"b12_6" ,"b12_7" ,"b12_8" ,"b12_9" },true),
            new CBulletFactory(new string[]{ "img/bullet/b13","b13_0","b13_1" ,"b13_2" ,"b13_3" ,"b13_4" ,"b13_5" ,"b13_6" ,"b13_7" ,"b13_8" ,"b13_9" },true),
            new CBulletFactory(new string[]{ "img/bullet/b14","b14_0","b14_1" ,"b14_2" ,"b14_3" },true),
            new CBulletFactory(new string[]{ "img/bullet/b15","b15_0","b15_1" ,"b15_2" },true),
            new CBulletFactory(new string[]{ "img/bullet/_b6","_b6_0","_b6_1","_b6_2","_b6_3","_b6_4","_b6_5","_b6_6","_b6_7","_b6_8","_b6_9" },false),
            new CBulletFactory(new string[]{ "img/bullet/l0", "l0_0", "l0_1" },false),
            new CBulletFactory(new string[]{ "img/bullet/l0_moto", "l0_moto_0", "l0_moto_1" },true),
    };
    CCommandManager CommandManager = new CCommandManager();
    void Start()
    {
        for (int i = 0; i < BulletFactory.Length; ++i)
        {
            BulletFactory[i].Load();
        }
        // Prefabsフォルダから、すべてのGameObjectを読み込む
        if (!ResourcesLoader.LoadAllObjects("Prefabs"))
        {
            print("Prefabsファイル読み込みに失敗しました");
        }
        // Soundフォルダからすべての音声ファイルを読み込む
        if (!CSoundPlayer.LoadAllSounds("se"))
        {
            print("seファイル読み込みに失敗しました");
        }
        if (!CSoundPlayer.LoadAllSounds("music"))
        {
            print("seファイル読み込みに失敗しました");
        }
        string file_name = Application.dataPath + @"\Scenes\Stage\stage1.txt";
        CommandManager.Initalize();
        CommandManager.LoadScript(file_name);
    }
    void Update()
    {
        CommandManager.Run();
    }
}
