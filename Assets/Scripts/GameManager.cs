using System;
using System.Collections;
using System.Net;
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
    public int score;
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

    // PAtch call che va parametrizzato
    public async Task PatchApiCall2()
    {
        int randomScore = UnityEngine.Random.Range(0, 1000);

        // 1. Prepare the data
        ScoreData scoreData = new ScoreData
        {
            score = randomScore,
            feedback = "Simone Pelosi!"
        };
        string jsonConverted = JsonUtility.ToJson(scoreData); // Convert to json
        string jsonConstructed = $"{{\"results\": {jsonConverted} }}"; // Wrap in a json object since Unity deosn't support inner seria
        byte[] requestBody = new UTF8Encoding().GetBytes(jsonConstructed);

        // 2. Prepare the URL
        string USER_ID = "cm0atxv6u0000me9grotb3u7h";
        string EXPERIENCE_ID = "1";
        string urlPath = $"https://futura-frontend-pi.vercel.app/api/users/{USER_ID}/experiences/{EXPERIENCE_ID}";

        // 3. Send the request
        UnityWebRequest www = new UnityWebRequest(urlPath, "PATCH");
        www.uploadHandler = new UploadHandlerRaw(requestBody);
        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();
        try
        {
            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Connection error: " + www.error.ToString());
            }

            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Protocol error:" + www.error.ToString());
            }

            if (www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Data Processing Error: " + www.error.ToString());
            }

            if (www.result != UnityWebRequest.Result.Success)
                throw new NotImplementedException(
                    $"Failed to patch with error: {www.result}"
                );

            Debug.Log("Patch done! " + www.result);
            isPressed = false;
        }
        catch (Exception ex)
        {
            isPressed = false;
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
