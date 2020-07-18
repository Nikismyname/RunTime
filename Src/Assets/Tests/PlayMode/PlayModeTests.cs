using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayModeTests
    {
        [UnityTest]
        public IEnumerator TestLevelWorks()
        {
            GameObject camera = new GameObject("Camera");
            camera.AddComponent<Camera>();

            GameObject main = new GameObject("Main");
            WorldSpaceUI worldSpaceUI = main.AddComponent<WorldSpaceUI>();
            worldSpaceUI.inputPanelPrefab = GetInputCanvasPrefab(); 
            worldSpaceUI.resultAndVariablesPanelPrefab = GetResultCanvasPrefab(); 
            worldSpaceUI.transperantMat = GetTransperantMatPrefab();
            worldSpaceUI.worldSpaceTextPrefab = WorldSpaceTextPrefab();
            yield return null;
        }

        private GameObject GetInputCanvasPrefab()
        {
            return Resources.Load("Prefabs/WorldSpaceCanvases/ConstantCanvasPrefab", typeof(GameObject)) as GameObject;
        }

        private GameObject WorldSpaceTextPrefab()
        {
            return Resources.Load("Prefabs/WorldSpaceCanvases/WorldSpaceTextPrefab", typeof(GameObject)) as GameObject;
        }

        private GameObject GetResultCanvasPrefab()
        {
            return Resources.Load("Prefabs/WorldSpaceCanvases/ResultCanvasPrefab", typeof(GameObject)) as GameObject;
        }

        private Material GetTransperantMatPrefab()
        {
            return Resources.Load("Materials/transperantMat", typeof(Material)) as Material;
        }
    }
}
