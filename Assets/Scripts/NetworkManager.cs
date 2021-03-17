﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public UIController uiController;

    private Attributes[] assetAttributes;
    private UnityEngine.Object[] assets;
    private GameObject[] instAssets;
    private int position = 0;
    public GameObject objectHolder;
    public float rotationSpeed = 50f;

    private bool isStartDone = false;
    
    public string url = String.Empty;

    public string jsonBaseUrl = String.Empty;
    IEnumerator getRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            callback(request);
        }
    }

    public void nextButtonPushed()
    {
        try
        {
            instAssets[position].SetActive(false);
            position = (position + 1) % instAssets.Length;
            Activate(position);
        }
        catch(Exception e)
        {
            Debug.LogError("Error in nextButtonPushed.");
            Debug.LogError(e);
        }
    }

    public void backButtonPushed()
    {
        try
        {
            instAssets[position].SetActive(false);
            position = (position + instAssets.Length - 1) % instAssets.Length;
            Activate(position);
        }
        catch (Exception e)
        {
            Debug.LogError("Error in backButtonPushed.");
            Debug.LogError(e);
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        int i = 0;
        UnityEngine.Networking.UnityWebRequest request
            = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        assets = bundle.LoadAllAssets();
        int assetCount = 0;
        for(i = 0; i < assets.Length; i++)
        {
            if(assets[i].GetType() == typeof(GameObject))
            {
                assetCount++;
            }
        }
        instAssets = new GameObject[assetCount];
        assetAttributes = new Attributes[assetCount];
        i = 0;
        foreach(UnityEngine.Object obj in assets)
        {
            if(obj.GetType() == typeof(GameObject))
            {
                GameObject g = (GameObject)obj;
                GameObject used = Instantiate(g);
                g.transform.parent = objectHolder.transform;
                used.SetActive(false);
                instAssets[i] = used;
                UnityWebRequest req = UnityWebRequest.Get(jsonBaseUrl + g.name + ".json");
                yield return req.SendWebRequest();
                assetAttributes[i] = JsonConvert.DeserializeObject<Attributes>(req.downloadHandler.text);
                i++;
            }
        }
        Activate(0);

        isStartDone = true;
    }

    void Activate(int i)
    {
        instAssets[i].SetActive(true);
        if(assetAttributes[i] != null)
        {
            // nameText.text = assetAttributes[i].Name;
            uiController.UpdateObjectPropertiesUI(assetAttributes[i]);
        }
        else
        {
            // nameText.text = "Untitled";
            uiController.SetAttributesToEmpty();
        }
    }        
}
