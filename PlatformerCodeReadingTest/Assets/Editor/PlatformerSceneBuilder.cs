#if UNITY_EDITOR
using System.IO;
using SchoolEscape.CameraSystem;
using SchoolEscape.Core;
using SchoolEscape.Data;
using SchoolEscape.Enemy;
using SchoolEscape.Player;
using SchoolEscape.UI;
using SchoolEscape.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SchoolEscape.Editor
{
public static class PlatformerSceneBuilder
{
    private const string Root = "Assets/SchoolEscape";
    private const string ScenePath = Root + "/Scenes/Main.unity";
    private const int GroundLayer = 8;
    private static Sprite _square;

    [MenuItem("Tools/School Escape/Rebuild Sample Level")]
    public static void BuildFromMenu()
    {
        BuildFromCommandLine();
        EditorUtility.DisplayDialog("School Escape", "The sample level was rebuilt.", "OK");
    }

    public static void BuildFromCommandLine()
    {
        EnsureFoldersAndLayer();
        _square = CreateSquareSprite();
        MovementSettings movementSettings = CreateMovementSettings();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        ScoreManager score = new GameObject("ScoreManager").AddComponent<ScoreManager>();
        LevelTimer timer = new GameObject("LevelTimer").AddComponent<LevelTimer>();
        Transform spawnPoint = CreateMarker("InitialSpawn", new Vector2(-8f, -2.45f));
        RespawnManager respawn = new GameObject("RespawnManager").AddComponent<RespawnManager>();
        SetObject(respawn, "_initialSpawnPoint", spawnPoint);

        PlayerLife playerLife = CreatePlayer(spawnPoint.position, movementSettings, out PlayerMotor playerMotor);
        GoalFlag goal = CreateWorld(score, respawn, playerMotor);
        CameraFollow cameraFollow = CreateCamera(playerLife.transform);

        LevelController controller = new GameObject("LevelController").AddComponent<LevelController>();
        SetObject(controller, "_playerLife", playerLife);
        SetObject(controller, "_respawnManager", respawn);
        SetObject(controller, "_scoreManager", score);
        SetObject(controller, "_levelTimer", timer);
        SetObject(controller, "_goalFlag", goal);
        CreateHud(score, timer, controller, playerLife);

        Selection.activeObject = controller.gameObject;
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"School Escape level built. Camera: {cameraFollow.name}");
    }

    private static void EnsureFoldersAndLayer()
    {
        CreateFolder("Assets", "SchoolEscape");
        CreateFolder(Root, "Art");
        CreateFolder(Root, "Data");
        CreateFolder(Root, "Scenes");
        SerializedObject tags = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        tags.FindProperty("layers").GetArrayElementAtIndex(GroundLayer).stringValue = "Ground";
        tags.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateFolder(string parent, string child)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + child))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }

    private static Sprite CreateSquareSprite()
    {
        string path = Root + "/Art/Square.png";
        if (!File.Exists(path))
        {
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
        }
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static MovementSettings CreateMovementSettings()
    {
        string path = Root + "/Data/PlayerMovement.asset";
        MovementSettings settings = AssetDatabase.LoadAssetAtPath<MovementSettings>(path);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<MovementSettings>();
            AssetDatabase.CreateAsset(settings, path);
        }
        SetFloat(settings, "_maxSpeed", 7f);
        SetFloat(settings, "_groundAcceleration", 42f);
        SetFloat(settings, "_groundDeceleration", 20f);
        SetFloat(settings, "_turnaroundAcceleration", 95f);
        SetFloat(settings, "_airAcceleration", 26f);
        SetFloat(settings, "_airDeceleration", 8f);
        SetFloat(settings, "_jumpVelocity", 13f);
        SetFloat(settings, "_jumpReleaseMultiplier", 0.45f);
        SetFloat(settings, "_coyoteTime", 0.12f);
        SetFloat(settings, "_jumpBufferTime", 0.12f);
        SetFloat(settings, "_baseGravityScale", 3.2f);
        SetFloat(settings, "_lowJumpGravityMultiplier", 2.2f);
        SetFloat(settings, "_fallGravityMultiplier", 1.65f);
        SetFloat(settings, "_maxFallSpeed", 20f);
        return settings;
    }

    private static PlayerLife CreatePlayer(Vector3 position, MovementSettings settings, out PlayerMotor motor)
    {
        GameObject player = SpriteObject("Player", position, new Vector2(0.75f, 1f), new Color(0.97f, 0.78f, 0.16f), 10);
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 3.2f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        CapsuleCollider2D collider = player.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.92f, 0.95f);
        PlayerInputReader input = player.AddComponent<PlayerInputReader>();
        player.AddComponent<PlayerCollector>();

        Transform sensorObject = CreateMarker("GroundSensor", new Vector2(0f, -0.54f));
        sensorObject.SetParent(player.transform, false);
        GroundSensor sensor = sensorObject.gameObject.AddComponent<GroundSensor>();
        SetLayerMask(sensor, "_groundMask", 1 << GroundLayer);

        motor = player.AddComponent<PlayerMotor>();
        SetObject(motor, "_input", input);
        SetObject(motor, "_groundSensor", sensor);
        SetObject(motor, "_settings", settings);

        PlayerLife life = player.AddComponent<PlayerLife>();
        SetObject(life, "_input", input);
        SetObject(life, "_motor", motor);
        SetObject(life, "_bodyCollider", collider);
        SetObject(life, "_bodyRenderer", player.GetComponent<SpriteRenderer>());
        return life;
    }

    private static GoalFlag CreateWorld(ScoreManager score, RespawnManager respawn, PlayerMotor playerMotor)
    {
        SpriteObject("Sky", new Vector2(19f, 0f), new Vector2(58f, 12f), new Color(0.32f, 0.68f, 0.92f), -20);
        CreatePlatform("Ground_A", new Vector2(-4f, -3.7f), new Vector2(12f, 1f));
        CreatePlatform("Ground_B", new Vector2(5f, -3.7f), new Vector2(4f, 1f));
        CreatePlatform("Ground_C", new Vector2(12f, -3.7f), new Vector2(8f, 1f));
        CreatePlatform("Ground_D", new Vector2(21f, -3.7f), new Vector2(8f, 1f));
        CreatePlatform("Ground_E", new Vector2(33f, -3.7f), new Vector2(12f, 1f));
        CreatePlatform("Ground_F", new Vector2(43f, -3.7f), new Vector2(7f, 1f));

        CreatePlatform("Floating_1", new Vector2(2f, -1.4f), new Vector2(3f, 0.45f));
        CreatePlatform("Floating_2", new Vector2(10f, -0.4f), new Vector2(4f, 0.45f));
        CreatePlatform("Floating_3", new Vector2(18f, -1.2f), new Vector2(3f, 0.45f));
        CreatePlatform("Floating_4", new Vector2(34f, -0.5f), new Vector2(4f, 0.45f));

        CreatePipe(new Vector2(14.2f, -2.45f));
        CreatePipe(new Vector2(36f, -2.25f));
        CreateBreakableBrick(new Vector2(6f, -0.4f));
        CreateRewardBrick(new Vector2(7.2f, -0.4f), score);
        CreateBreakableBrick(new Vector2(8.4f, -0.4f));
        CreateRewardBrick(new Vector2(23f, -0.5f), score);

        Vector2[] coins = {
            new Vector2(-1f,-2.3f), new Vector2(2f,-0.5f), new Vector2(5f,-2.2f),
            new Vector2(9f,0.4f), new Vector2(10.5f,0.7f), new Vector2(12f,0.4f),
            new Vector2(18f,-0.3f), new Vector2(24f,-2.1f), new Vector2(31f,-2.1f),
            new Vector2(34f,0.4f), new Vector2(39f,-1.5f), new Vector2(41f,-0.7f)
        };
        foreach (Vector2 position in coins) CreateCoin(position, score);

        CreateEnemy(new Vector2(4.8f, -2.7f), 1.2f);
        CreateEnemy(new Vector2(19.5f, -2.7f), 2f);
        CreateEnemy(new Vector2(32f, -2.7f), 2.3f);
        CreateMovingPlatform(new Vector2(27f, -2.2f));
        CreateCheckpoint(new Vector2(22f, -2.45f), respawn);
        CreateKillZone();
        CreateStairs();
        return CreateGoal(new Vector2(45f, -1.55f));
    }

    private static void CreatePlatform(string name, Vector2 position, Vector2 size)
    {
        GameObject platform = SpriteObject(name, position, size, new Color(0.52f, 0.29f, 0.14f), 0);
        platform.layer = GroundLayer;
        platform.AddComponent<BoxCollider2D>();
    }

    private static void CreatePipe(Vector2 position)
    {
        GameObject pipe = SpriteObject("Pipe", position, new Vector2(1.7f, 2.1f), new Color(0.12f, 0.65f, 0.23f), 2);
        pipe.layer = GroundLayer;
        pipe.AddComponent<BoxCollider2D>();
    }

    private static void CreateBreakableBrick(Vector2 position)
    {
        GameObject brick = SpriteObject("BreakableBrick", position, Vector2.one, new Color(0.68f, 0.34f, 0.14f), 2);
        brick.layer = GroundLayer;
        brick.AddComponent<BoxCollider2D>();
        BreakableBrick breakable = brick.AddComponent<BreakableBrick>();
        SetObject(breakable, "_brickRenderer", brick.GetComponent<SpriteRenderer>());
    }

    private static void CreateRewardBrick(Vector2 position, ScoreManager score)
    {
        GameObject brick = SpriteObject("RewardBrick", position, Vector2.one, new Color(1f, 0.58f, 0.08f), 2);
        brick.layer = GroundLayer;
        brick.AddComponent<BoxCollider2D>();
        RewardBrick reward = brick.AddComponent<RewardBrick>();
        SetObject(reward, "_scoreManager", score);
        SetObject(reward, "_brickRenderer", brick.GetComponent<SpriteRenderer>());
    }

    private static void CreateCoin(Vector2 position, ScoreManager score)
    {
        GameObject coin = SpriteObject("Coin", position, new Vector2(0.35f, 0.65f), new Color(1f, 0.84f, 0.05f), 4);
        CircleCollider2D collider = coin.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        CoinCollectible collectible = coin.AddComponent<CoinCollectible>();
        SetObject(collectible, "_scoreManager", score);
    }

    private static void CreateEnemy(Vector2 position, float distance)
    {
        GameObject enemy = SpriteObject("PatrolEnemy", position, new Vector2(0.85f, 0.75f), new Color(0.48f, 0.18f, 0.12f), 5);
        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.gravityScale = 3.2f;
        body.freezeRotation = true;
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        PatrolEnemy patrol = enemy.AddComponent<PatrolEnemy>();
        SetFloat(patrol, "_patrolDistance", distance);
        StompableEnemy stompable = enemy.AddComponent<StompableEnemy>();
        SetObject(stompable, "_patrol", patrol);
        SetObject(stompable, "_bodyCollider", collider);
    }

    private static void CreateMovingPlatform(Vector2 position)
    {
        GameObject platform = SpriteObject("MovingPlatform", position, new Vector2(2.2f, 0.4f), new Color(0.55f, 0.55f, 0.62f), 1);
        platform.layer = GroundLayer;
        platform.AddComponent<BoxCollider2D>();
        Rigidbody2D body = platform.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        platform.AddComponent<MovingPlatform>();
    }

    private static void CreateCheckpoint(Vector2 position, RespawnManager respawn)
    {
        GameObject checkpoint = SpriteObject("Checkpoint", position, new Vector2(0.25f, 1.5f), new Color(0.95f, 0.95f, 0.95f), 3);
        BoxCollider2D collider = checkpoint.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        Checkpoint component = checkpoint.AddComponent<Checkpoint>();
        SetObject(component, "_respawnManager", respawn);
        SetObject(component, "_indicator", checkpoint.GetComponent<SpriteRenderer>());
    }

    private static void CreateKillZone()
    {
        GameObject zone = new GameObject("FallKillZone", typeof(BoxCollider2D), typeof(KillZone));
        zone.transform.position = new Vector2(20f, -6.2f);
        BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(70f, 2f);
        collider.isTrigger = true;
    }

    private static void CreateStairs()
    {
        for (int i = 0; i < 5; i++)
        {
            CreatePlatform($"Stair_{i + 1}", new Vector2(38.5f + i * 0.8f, -3f + i * 0.65f), new Vector2(0.8f, 1.3f + i * 1.3f));
        }
    }

    private static GoalFlag CreateGoal(Vector2 position)
    {
        GameObject pole = SpriteObject("GoalFlag", position, new Vector2(0.18f, 4.2f), Color.white, 3);
        BoxCollider2D collider = pole.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        GoalFlag goal = pole.AddComponent<GoalFlag>();
        SpriteObject("GoalBanner", position + new Vector2(0.65f, 1.5f), new Vector2(1.1f, 0.6f), new Color(0.95f, 0.2f, 0.28f), 4);
        return goal;
    }

    private static CameraFollow CreateCamera(Transform target)
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(CameraFollow));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(-3f, 0f, -10f);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.2f;
        CameraFollow follow = cameraObject.GetComponent<CameraFollow>();
        SetObject(follow, "_target", target);
        return follow;
    }

    private static void CreateHud(ScoreManager score, LevelTimer timer, LevelController controller, PlayerLife life)
    {
        HudView hud = new GameObject("HUD").AddComponent<HudView>();
        SetObject(hud, "_scoreManager", score);
        SetObject(hud, "_levelTimer", timer);
        SetObject(hud, "_levelController", controller);
        SetObject(hud, "_playerLife", life);
    }

    private static Transform CreateMarker(string name, Vector2 position)
    {
        GameObject marker = new GameObject(name);
        marker.transform.position = position;
        return marker.transform;
    }

    private static GameObject SpriteObject(string name, Vector2 position, Vector2 scale, Color color, int order)
    {
        GameObject go = new GameObject(name, typeof(SpriteRenderer));
        go.transform.position = position;
        go.transform.localScale = scale;
        SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = _square;
        renderer.color = color;
        renderer.sortingOrder = order;
        return go;
    }

    private static void SetObject(Object target, string name, Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(name).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string name, float value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(name).floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetLayerMask(Object target, string name, int value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(name).intValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
}
#endif
