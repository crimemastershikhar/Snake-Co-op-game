using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainScene,
        Loading,
    }
    private static Action loaderCallbackAction;
    public static void Load(Scene scene)
    {
        //setup callback action that will be triggered after scene loading
        loaderCallbackAction = () =>
        {
            //Load target scene when loading scene is loaded
            SceneManager.LoadScene(scene.ToString());
        };
        //Load loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    }
    public static void LoaderCallback()
    {
        if(loaderCallbackAction != null)
        {
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}
