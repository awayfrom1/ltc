using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]

public class cameraSceneGameMove : MonoBehaviour
{
    public bool ifFollowGame;
    public bool ifFollowScene;

    private SceneView view;

    private void OnEnable()
    {
        UnityEditor.EditorApplication.update += cameraMove;
    }

    private void OnDisable()
    {
        UnityEditor.EditorApplication.update -= cameraMove;
    }

    private void cameraMove()
    {
        if (Application.isPlaying || UnityEditor.SceneView.sceneViews.Count < 0) return;
        if (view == null) view = (UnityEditor.SceneView)UnityEditor.SceneView.sceneViews[0];

        if (ifFollowScene)
        {
            this.transform.position = view.camera.transform.position;
            this.transform.rotation = view.camera.transform.rotation;
        }

        if (ifFollowGame)
        {
            view.AlignViewToObject(this.transform);
            //view.AlignWithView();
            //view.MoveToView();
        }
    }
}
