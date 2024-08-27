using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DeserializableData
{
    public string userId;
    public string id;
    public string title;
    public string body;
}

[Serializable]
public class ScoreData
{
    public string score;
    public string feedback;
}

public class GameManager : MonoBehaviour
{
    string backendApiPath = "https://jsonplaceholder.typicode.com/posts/1";

    bool isPressed = false;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isPressed)
        {
            isPressed = true;
            //TestPatch();
            await PatchApiCall2();
        }
    
    }


    [ContextMenu("TestPatchAsyncAwait")]
    public async void TestPatch()
    {
        Debug.Log("TestPatch");
        await PatchApiCall();
        isPressed = false;
    }

    [ContextMenu("TestPatchCoroutine")]
    public void TestPatchCoroutine()
    {
        Debug.Log("TestPatchCoroutine");
        StartCoroutine(GetText());
    }

    public async Task PatchApiCall()
    {
        using var www = UnityWebRequest.Get(backendApiPath);

        //www.SetRequestHeader(
        //    "Authorization",
        //    "Bearer " + cognitoToken.AuthenticationResult.IdToken
        //);
        //www.SetRequestHeader("Content-Type", "application/text");

        var operation = www.SendWebRequest();
        try
        {
            while (!operation.isDone)
                await Task.Yield();

            var jsonResponse = www.downloadHandler.text;

            if (www.result != UnityWebRequest.Result.Success)
                throw new NotImplementedException(
                    $"Failed to get with error: {www.result}"
                );

            DeserializableData result = JsonUtility.FromJson<DeserializableData>(
                jsonResponse
            );

            Debug.Log($"Data correctly downloaded: {jsonResponse} ");
            PrintDesirializableData(result);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get data with error: {ex}");
            throw;
        }
    }

    public async Task PatchApiCall2()
    {
        ScoreData scoreData = new ScoreData
        {
            score = "100",
            feedback = "Good job!"
        };

        string jsonConverted = JsonUtility.ToJson(scoreData); // Convert to json
        string httpPutRequestBody = $"{{\"results\":\"{jsonConverted}\"}}"; // Wrap in a json object since Unity deosn't support inner seria

        string USER_ID = "1";
        string EXPERIENCE_ID = "1";

        string urlPath = $"https://futura-frontend-pi.vercel.app/api/users/{USER_ID}/experiences/{EXPERIENCE_ID}";
        UnityWebRequest www = new UnityWebRequest(urlPath, "PATCH");

        byte[] jsonToSend = new UTF8Encoding().GetBytes(httpPutRequestBody);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);

        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();
        try
        {
            while (!operation.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
                throw new NotImplementedException(
                    $"Failed to patch with error: {www.result}"
                );

            Debug.Log("Patch done!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to PATCH with error:  {ex}");
            throw;
        }
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(backendApiPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    public void PrintDesirializableData(DeserializableData data)
    {
        Debug.Log($"userId: {data.userId}");
        Debug.Log($"id: {data.id}");
        Debug.Log($"title: {data.title}");
        Debug.Log($"body: {data.body}");
    }
}
