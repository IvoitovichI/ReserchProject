using System.Collections;
using DungeonTrace.Interaction;
using DungeonTrace.Player;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DungeonTrace.Tests.PlayMode
{
    public sealed class InteractionPlayModeTests
    {
        [UnityTest] public IEnumerator CenterCameraRay_InteractsWithTarget()
        {
            var cameraObject = new GameObject("TestCamera");
            var camera = cameraObject.AddComponent<Camera>();
            var aim = cameraObject.AddComponent<AimProvider>();
            aim.Configure(camera);
            var raycaster = cameraObject.AddComponent<InteractionRaycaster>();
            raycaster.Configure(aim, null, null, 1 << 8);

            var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.layer = 8;
            target.transform.position = new Vector3(0f, 0f, 2f);
            var interactable = target.AddComponent<TestInteractable>();
            Physics.SyncTransforms();
            yield return null;

            Assert.That(raycaster.TryInteract(), Is.True);
            Assert.That(interactable.Count, Is.EqualTo(1));
            Object.Destroy(cameraObject);
            Object.Destroy(target);
        }

        private sealed class TestInteractable : MonoBehaviour, IInteractable
        {
            public int Count { get; private set; }
            public void Interact() => Count++;
        }

        [UnityTest] public IEnumerator PrototypeRoom_BuildsPlaceholderGeometry()
        {
            var builderObject = new GameObject("RoomBuilderTest");
            var builder = builderObject.AddComponent<RoomBuilder>();
            var definition = RoomDefinition.CreatePrototype();
            builder.Configure(definition);
            var room = builder.Build();
            yield return null;
            Assert.That(room, Is.Not.Null);
            Assert.That(room.transform.Find("Floor"), Is.Not.Null);
            Assert.That(room.transform.Find("Exit_exit-prototype-01-north"), Is.Not.Null);
            Object.Destroy(definition);
            Object.Destroy(builderObject);
        }
    }
}
