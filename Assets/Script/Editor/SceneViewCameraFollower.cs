using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneViewCameraFollower : MonoBehaviour
{
    private static GameObject target; // Il GameObject da seguire
    
    static SceneViewCameraFollower()
    {
        // Esegui l'aggiornamento della camera ogni frame
        EditorApplication.update += UpdateSceneViewCamera;
    }

    [MenuItem("Tools/Set SceneView Follow Target")]
    public static void SetFollowTarget()
    {
        // Imposta il GameObject attivo come target
        if (Selection.activeGameObject != null)
        {
            target = Selection.activeGameObject;
            Debug.Log($"SceneView will now follow: {target.name}");
        }
        else
        {
            Debug.LogWarning("No GameObject selected. Please select a GameObject to follow.");
        }
    }

    private static void UpdateSceneViewCamera()
    {
        if (target == null)
        {
            return;
        }

        // Controlla se esiste una SceneView attiva
        if (SceneView.lastActiveSceneView != null)
        {
            // Ottieni la SceneView attiva
            SceneView sceneView = SceneView.lastActiveSceneView;

            // Imposta la nuova posizione e rotazione per seguire il target
            Vector3 newPosition = target.transform.position;
            //Quaternion newRotation = Quaternion.LookRotation(target.transform.position - newPosition);

            sceneView.pivot = newPosition;
            sceneView.rotation = target.transform.rotation;

            // Rinfresca la SceneView per applicare i cambiamenti
            sceneView.Repaint();
        }
    }
}
#endif

