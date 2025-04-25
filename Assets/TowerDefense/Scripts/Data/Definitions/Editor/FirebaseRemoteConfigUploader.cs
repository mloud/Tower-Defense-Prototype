using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor.ShaderGraph.Serialization;

namespace TowerDefense.Scripts.Data.Definitions.Editor
{
    public static class FirebaseRemoteUploader
    {
        public static string ProjectId = "towerdefense-a08da";
        
        public static async void UploadToFirebase(string key, string json)
        {
            string credentialsPath = Application.dataPath + "/TowerDefense/Editor/firebase-key.json";
            string credentialsJson = File.ReadAllText(credentialsPath);

            var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential
                .FromJson(credentialsJson)
                .CreateScoped("https://www.googleapis.com/auth/firebase.remoteconfig");

            var token = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            string endpoint = $"https://firebaseremoteconfig.googleapis.com/v1/projects/{ProjectId}/remoteConfig";

            var remoteConfigPayload = new
            {
                parameters = new Dictionary<string, object>
                {
                    [key] = new
                    {
                        defaultValue = new { value = json }
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(remoteConfigPayload), Encoding.UTF8,
                "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.DefaultRequestHeaders.Add("If-Match", "*");

            var response = await client.PutAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("✅ Remote config uploaded successfully!");
            }
            else
            {
                Debug.LogError($"❌ Upload failed: {response.StatusCode}\n{await response.Content.ReadAsStringAsync()}");
            }
        }
        
        // New method to download data from Firebase Remote Config
        public static async UniTask<string> DownloadFromFirebase(string key)
        {
            string credentialsPath = Application.dataPath + "/TowerDefense/Editor/firebase-key.json";
            string credentialsJson = File.ReadAllText(credentialsPath);

            var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential
                .FromJson(credentialsJson)
                .CreateScoped("https://www.googleapis.com/auth/firebase.remoteconfig");

            var token = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            string endpoint = $"https://firebaseremoteconfig.googleapis.com/v1/projects/{ProjectId}/remoteConfig";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Debug.Log("✅ Remote config downloaded successfully!");
                var remoteConfig = JsonConvert.DeserializeObject<JObject>(content);
                // Check if the parameters contain the specified key
                if (remoteConfig.ContainsKey("parameters"))
                {
                    var parameters = remoteConfig["parameters"] as JObject;;

                    // If the key exists in the parameters, deserialize the nested JSON value
                    if (parameters != null && parameters.ContainsKey(key))
                    {
                        var json = parameters[key]["defaultValue"]["value"].ToString();
                        return json;
                    }
                }

                // Handle the downloaded config data as needed
                return null;
            }
            else
            {
                Debug.LogError($"❌ Download failed: {response.StatusCode}\n{await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
    }
}