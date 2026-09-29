using DungeonTrace.Domain;
using DungeonTrace.Combat;
using DungeonTrace.Flow;
using DungeonTrace.Interaction;
using DungeonTrace.Player;
using DungeonTrace.Rooms;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static bool hasBootstrapped;
        private GameFlowController flow;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetBootstrapFlag() => hasBootstrapped = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (hasBootstrapped) return;
            hasBootstrapped = true;
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            flow = new GameFlowController();
            if (SceneManager.GetActiveScene().name == "DungeonWalkthrough")
            {
                flow.StartSession(new SessionConfig(ResearchMode.Standard, 12345, "prototype-02"));
                CreatePlayer(new Vector3(0f, 0f, -4f));
                Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
                return;
            }
            if (SceneManager.GetActiveScene().name == "TenRoomDungeonZone")
            {
                flow.StartSession(new SessionConfig(ResearchMode.Standard, 12345, "prototype-03"));
                CreatePlayer(new Vector3(0f, 0f, -4f));
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }
            flow.StartSession(new SessionConfig(ResearchMode.Standard, 12345, "prototype-01"));
            var room = CreateTestRoom();
            CreatePlayer(room.Definition.PlayerSpawn);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private PlayerHealth CreatePlayer(Vector3 spawnPosition)
        {
            var player = new GameObject("Player");
            player.transform.position = spawnPosition;
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f; controller.radius = .35f; controller.center = new Vector3(0f, .9f, 0f);
            var input = player.AddComponent<PlayerInputReader>();
            var cameraObject = new GameObject("PlayerCamera");
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            var health = player.AddComponent<PlayerHealth>();
            var state = player.AddComponent<PlayerStateController>();
            state.Configure(input, health, flow);
            var motor = player.AddComponent<FirstPersonMotor>(); motor.Configure(controller, input);
            var look = player.AddComponent<PlayerLookController>(); look.Configure(cameraObject.transform, input, state);
            var aim = player.AddComponent<AimProvider>(); aim.Configure(camera);
            var interaction = player.AddComponent<InteractionRaycaster>(); interaction.Configure(aim, input, state);
            var weapon = player.AddComponent<WeaponController>(); weapon.Configure(aim, input, WeaponDefinition.CreatePrototypePulsePistol(), state);
            var combatSandbox = player.AddComponent<CombatSandboxSpawner>();
            combatSandbox.Configure(cameraObject.transform, weapon);
            if (SceneManager.GetActiveScene().name != "TenRoomDungeonZone") combatSandbox.SpawnEnemyEncounter(player.transform);
            motor.Configure(controller, input, state);
            return health;
        }

        private static RoomBuilder CreateTestRoom()
        {
            var roomObject = new GameObject("TestRoomBuilder");
            var builder = roomObject.AddComponent<RoomBuilder>();
            builder.Configure(RoomDefinition.CreatePrototype());
            var room = builder.Build();
            var console = CreateConsole(room.transform, new Vector3(0f, 1f, 3f));
            var traceConsole = console.AddComponent<TraceConsole>();
            traceConsole.Configure(console.GetComponent<Renderer>());
            return builder;
        }

        private static GameObject CreateConsole(Transform parent, Vector3 position)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "TraceConsole_console-prototype-01";
            block.transform.SetParent(parent, false);
            block.transform.localPosition = position;
            block.transform.localScale = new Vector3(1f, 2f, .6f);
            block.GetComponent<Renderer>().material.color = Color.yellow;
            return block;
        }
    }
}
