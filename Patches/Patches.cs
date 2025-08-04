using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

namespace CustomPosters
{
    internal class Patches : MonoBehaviour
    {
        private static bool _materialsUpdated = false;
        private static string _selectedPack = null;
        private static Material _copiedMaterial = null;
        private static readonly List<GameObject> CreatedPosters = new List<GameObject>();
        private static int _sessionMapSeed = 0;
        private static bool _isNewLobby = true;
        private static bool _sessionSeedInitialized = false;

        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void GameNetworkManagerStartPatch()
        {
            _sessionSeedInitialized = false;
            _sessionMapSeed = 0;
            Plugin.Log.LogDebug("Reset session seed initialization");
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(StartOfRound __instance)
        {
            _materialsUpdated = false;
            CopyPlane001Material();
            if (_isNewLobby)
            {
                if (!_sessionSeedInitialized)
                {
                    _sessionMapSeed = PosterConfig.PerSession.Value ? StartOfRound.Instance.randomMapSeed : Environment.TickCount;
                    _sessionSeedInitialized = true;
                    Plugin.Log.LogDebug($"Initialized session with map seed: {_sessionMapSeed}");
                }

                int seedToUse;
                if (PosterConfig.PerSession.Value)
                {
                    seedToUse = _sessionMapSeed;
                }
                else
                {
                    seedToUse = Environment.TickCount;
                    _selectedPack = null;
                }

                Plugin.Service.SetRandomSeed(seedToUse);
            }

            if (__instance.inShipPhase)
            {
                __instance.StartCoroutine(DelayedUpdateMaterialsAsync(__instance));
            }
            _isNewLobby = false;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "StartHost")]
        [HarmonyPostfix]
        private static void StartHostPatch()
        {
            _isNewLobby = true;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "JoinLobby")]
        [HarmonyPostfix]
        private static void JoinLobbyPatch()
        {
            _isNewLobby = true;
        }

        private static IEnumerator LoadTextureAsync(string filePath, Action<(Texture2D texture, string filePath)> onComplete)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Plugin.Log.LogError($"File not found: {filePath}");
                    onComplete?.Invoke((null, null));
                    yield break;
                }

                var cachedTexture = Plugin.Service.GetCachedTexture(filePath);
                if (cachedTexture != null)
                {
                    onComplete?.Invoke((cachedTexture, filePath));
                    yield break;
                }

                var fileData = File.ReadAllBytes(filePath);
                var texture = new Texture2D(2, 2);
                if (!texture.LoadImage(fileData))
                {
                    Plugin.Log.LogError($"Failed to load texture from {filePath}");
                    onComplete?.Invoke((null, null));
                    yield break;
                }

                texture.filterMode = FilterMode.Point;
                Plugin.Service.CacheTexture(filePath, texture);
                onComplete?.Invoke((texture, filePath));
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error loading file {filePath}: {ex.Message}");
                onComplete?.Invoke((null, null));
            }
        }

        private static void CopyPlane001Material()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Plugin.Log.LogError("Poster plane Plane.001 not found under HangarShip");
                return;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Plugin.Log.LogError("Poster plane renderer or materials not found");
                return;
            }

            _copiedMaterial = new Material(originalRenderer.material);
        }

        private static void HideVanillaPosterPlane()
        {
            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001 (Old)");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
                return;
            }

            posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
            }
        }

        private static void CleanUpPosters()
        {
            foreach (var poster in CreatedPosters)
            {
                if (poster != null)
                {
                    var renderer = poster.GetComponent<PosterRenderer>();
                    if (renderer != null) UnityEngine.Object.Destroy(renderer);
                    UnityEngine.Object.Destroy(poster);
                }
            }
            CreatedPosters.Clear();
        }

        private static GameObject CreatePoster()
        {
            var newPoster = GameObject.CreatePrimitive(PrimitiveType.Quad);
            if (newPoster == null)
            {
                Plugin.Log.LogError("Failed to create new poster GameObject");
            }
            return newPoster;
        }

        public class PosterRenderer : MonoBehaviour
        {
            private VideoPlayer _videoPlayer;
            private AudioSource _audioSource;
            private RenderTexture _renderTexture;

            private static UnityEngine.Video.VideoAspectRatio ConvertAspectRatio(PosterConfig.VideoAspectRatio aspectRatio)
            {
                return aspectRatio switch
                {
                    PosterConfig.VideoAspectRatio.Stretch => UnityEngine.Video.VideoAspectRatio.Stretch,
                    PosterConfig.VideoAspectRatio.FitInside => UnityEngine.Video.VideoAspectRatio.FitInside,
                    PosterConfig.VideoAspectRatio.FitOutside => UnityEngine.Video.VideoAspectRatio.FitOutside,
                    PosterConfig.VideoAspectRatio.NoScaling => UnityEngine.Video.VideoAspectRatio.NoScaling,
                    _ => UnityEngine.Video.VideoAspectRatio.Stretch
                };
            }

            public void Initialize(Texture2D texture, string videoPath, Material materialTemplate)
            {
                var renderer = GetComponent<MeshRenderer>();
                var material = new Material(materialTemplate);

                if (videoPath != null && System.IO.Path.GetExtension(videoPath).ToLower() == ".mp4")
                {
                    _renderTexture = new RenderTexture(512, 512, 16);
                    material.mainTexture = _renderTexture;

                    _videoPlayer = gameObject.AddComponent<VideoPlayer>();
                    _videoPlayer.url = "file://" + videoPath;
                    _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                    _videoPlayer.targetTexture = _renderTexture;
                    var (volume, maxDistance, aspectRatio) = PosterConfig.GetFileAudioSettings(videoPath);
                    _videoPlayer.aspectRatio = ConvertAspectRatio(aspectRatio);
                    _videoPlayer.isLooping = true;
                    _videoPlayer.playOnAwake = false;

                    _audioSource = gameObject.AddComponent<AudioSource>();
                    _audioSource.spatialBlend = 1.0f;
                    _audioSource.spatialize = true;
                    _audioSource.rolloffMode = AudioRolloffMode.Custom;
                    var curve = new AnimationCurve(
                        new Keyframe(0f, 1.0f),
                        new Keyframe(0.157f, 0.547f),
                        new Keyframe(0.517f, 0.278f),
                        new Keyframe(1.259f, 0.102f),
                        new Keyframe(2.690f, 0.033f),
                        new Keyframe(4.0f, 0.0f)
                    );
                    _audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

                    _audioSource.maxDistance = maxDistance;

                    _videoPlayer.Stop();

                    _videoPlayer.errorReceived += (player, message) =>
                    {
                        Plugin.Log.LogError($"VideoPlayer error for {videoPath}: {message}");
                    };
                    _videoPlayer.prepareCompleted += (player) =>
                    {
                        Plugin.Log.LogDebug($"Video prepared successfully: {videoPath}");
                        player.Play();
                    };

                    if (PosterConfig.EnableVideoAudio.Value)
                    {
                        _videoPlayer.controlledAudioTrackCount = 1;
                        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                        _videoPlayer.SetTargetAudioSource(0, _audioSource);
                        _audioSource.volume = volume / 100.0f;
                        _audioSource.mute = false;
                        for (ushort track = 0; track < _videoPlayer.audioTrackCount; track++)
                        {
                            _videoPlayer.EnableAudioTrack(track, true);
                        }
                    }
                    else
                    {
                        _videoPlayer.controlledAudioTrackCount = 0;
                        _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                        _audioSource.Stop();
                        _audioSource.volume = 0f;
                        _audioSource.mute = true;
                        for (ushort track = 0; track < _videoPlayer.audioTrackCount; track++)
                        {
                            _videoPlayer.EnableAudioTrack(track, false);
                        }
                    }

                    _videoPlayer.source = VideoSource.Url;
                    _videoPlayer.enabled = true;
                    _videoPlayer.Play();

                    renderer.material = material;
                }
                else if (texture != null)
                {
                    material.mainTexture = texture;
                    renderer.material = material;
                }
                else
                {
                    Plugin.Log.LogError($"No valid texture or video for poster: {gameObject.name}");
                    Destroy(gameObject);
                }
            }

            private void Update()
            {
                if (_audioSource != null && !PosterConfig.EnableVideoAudio.Value)
                {
                    if (_audioSource.isPlaying)
                    {
                        _audioSource.Stop();
                    }
                    _audioSource.volume = 0f;
                    _audioSource.mute = true;
                }
            }

            private void OnDestroy()
            {
                if (_renderTexture != null)
                {
                    _renderTexture.Release();
                    UnityEngine.Object.Destroy(_renderTexture);
                }
                if (_videoPlayer != null && _videoPlayer.isPlaying)
                {
                    _videoPlayer.Stop();
                }
            }
        }

        private static IEnumerator CreateCustomPostersAsync()
        {
            CleanUpPosters();

            var environment = GameObject.Find("Environment");
            if (environment == null)
            {
                Plugin.Log.LogError("Environment GameObject not found in the scene hierarchy");
                yield break;
            }

            var hangarShip = environment.transform.Find("HangarShip")?.gameObject;
            if (hangarShip == null)
            {
                Plugin.Log.LogError("HangarShip GameObject not found under Environment");
                yield break;
            }

            var postersParent = new GameObject("CustomPosters");
            postersParent.transform.SetParent(hangarShip.transform);
            postersParent.transform.localPosition = Vector3.zero;

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane == null)
            {
                Plugin.Log.LogError("Poster [Plane.001] not found under HangarShip");
                yield break;
            }

            var originalRenderer = posterPlane.GetComponent<MeshRenderer>();
            if (originalRenderer == null || originalRenderer.materials.Length == 0)
            {
                Plugin.Log.LogError("Poster plane renderer or materials not found");
                yield break;
            }

            var posterData = new (Vector3 position, Vector3 rotation, Vector3 scale, string name)[]
            {
        (new Vector3(4.1886f, 2.9318f, -16.8409f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1"),
        (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2"),
        (new Vector3(9.9186f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3"),
        (new Vector3(5.2187f, 2.5963f, -11.0945f), new Vector3(0, 337.5868f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4"),
        (new Vector3(5.5286f, 2.5882f, -17.3541f), new Vector3(0, 201.1556f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5"),
        (new Vector3(3.0647f, 2.8174f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips")
            };

            // Adjust poster positions based on ShipWindows or ShipWindowsBeta
            if (Plugin.Service.IsShipWindowsInstalled && Plugin.Service.IsWindow2Enabled)
            {
                Plugin.Log.LogInfo("Repositioning posters due to ShipWindows Right Window enabled");
                posterData[1] = (new Vector3(6.4202f, 2.2577f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                posterData[3] = (new Vector3(6.4449f, 3.0961f, -10.8221f), new Vector3(0, 0.026f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
            }

            // Adjust poster positions for ShipWindows/Beta Right Window and WiderShipMod Extended Side Left
            if (Plugin.Service.IsShipWindowsInstalled && Plugin.Service.IsWindow2Enabled && Plugin.Service.IsWiderShipModInstalled && Plugin.Service.WiderShipExtendedSide == "Left")
            {
                Plugin.Log.LogInfo("Repositioning posters due to ShipWindows Left Window, Right Window and WiderShipMod Extended Side Left enabled");
                posterData[1] = (new Vector3(6.4202f, 2.2577f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                posterData[3] = (new Vector3(6.4449f, 3.0961f, -10.8221f), new Vector3(0, 0.026f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                posterData[0] = (new Vector3(4.6777f, 2.9007f, -19.63f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                posterData[2] = (new Vector3(9.7197f, 2.8151f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                posterData[4] = (new Vector3(5.3602f, 2.5482f, -18.3793f), new Vector3(0, 118.0114f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                posterData[5] = (new Vector3(2.8647f, 2.7774f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
            }

            // Adjust poster positions based on WiderShipMod's Extended Side
            if (Plugin.Service.IsWiderShipModInstalled)
            {
                Plugin.Log.LogInfo($"Repositioning posters due to WiderShipMod Extended Side: {Plugin.Service.WiderShipExtendedSide}");

                switch (Plugin.Service.WiderShipExtendedSide)
                {
                    case "Both":
                        posterData[0] = (new Vector3(4.6877f, 2.9407f, -19.62f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[3] = (new Vector3(5.5699f, 2.5963f, -10.3268f), new Vector3(0, 62.0324f, 2.6799f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(5.3602f, 2.5882f, -18.3793f), new Vector3(0, 118.0114f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.7253f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        break;

                    case "Right":
                        posterData[0] = (new Vector3(4.2224f, 2.9318f, -16.8609f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[1] = (new Vector3(6.4202f, 2.4776f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                        posterData[2] = (new Vector3(9.9426f, 2.8591f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                        posterData[3] = (new Vector3(5.5699f, 2.5963f, -10.3268f), new Vector3(0, 62.0324f, 2.6799f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(5.5386f, 2.5882f, -17.3641f), new Vector3(0, 200.9099f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(3.0947f, 2.8174f, -6.733f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        break;

                    case "Left":
                        if (!(Plugin.Service.IsShipWindowsInstalled && Plugin.Service.IsWindow2Enabled))
                        {
                            posterData[0] = (new Vector3(4.6777f, 2.9007f, -19.63f), new Vector3(0, 118.2274f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                            posterData[1] = (new Vector3(6.4202f, 2.2577f, -10.8226f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4882f, 2f), "Poster2");
                            posterData[2] = (new Vector3(9.7197f, 2.8151f, -17.4716f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                            posterData[3] = (new Vector3(5.2187f, 2.5963f, -11.0945f), new Vector3(0, 337.5868f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                            posterData[4] = (new Vector3(5.3602f, 2.5482f, -18.3793f), new Vector3(0, 118.0114f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                            posterData[5] = (new Vector3(2.8647f, 2.7774f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        }
                        break;
                }
            }

            // Reposition posters based on 2 Story Ship Mod compatibility
            if (Plugin.Service.Is2StoryShipModInstalled)
            {
                if (Plugin.Service.IsShipWindowsInstalled)
                {
                    // If ShipWindows and 2 Story Mod are detected, ignore both configs and use specific positions
                    Plugin.Log.LogInfo("Repositioning posters due to ShipWindows and 2 Story Ship Mod detected");
                    posterData[0] = (new Vector3(6.5923f, 2.9318f, -17.4766f), new Vector3(0, 179.2201f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(10.2813f, 2.7482f, -8.8271f), new Vector3(0, 0.9014f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                if (Plugin.Service.IsShipWindowsInstalled && Plugin.Service.IsWiderShipModInstalled)
                {
                    // If ShipWindows and WiderShipMod are detected, ignore 2 Story Mod config
                    Plugin.Log.LogInfo("Repositioning posters due to ShipWindows and WiderShipMod detected with 2 Story Ship Mod");
                    posterData[0] = (new Vector3(6.5923f, 2.9318f, -22.4766f), new Vector3(0, 179.2201f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.0884f, 2.4776f, -5.8265f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[2] = (new Vector3(10.1364f, 2.8591f, -22.4788f), new Vector3(0, 180.3376f, 0), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(7.8577f, 2.7482f, -22.4803f), new Vector3(0, 179.7961f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(-5.8111f, 2.541f, -17.577f), new Vector3(0, 270.0942f, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                else if (Plugin.Service.IsWiderShipModInstalled)
                {
                    // If WiderShipMod is detected with 2 Story Mod, use specific positions
                    Plugin.Log.LogInfo("Repositioning posters due to WiderShipMod detected with 2 Story Ship Mod");
                    posterData[0] = (new Vector3(6.3172f, 2.9407f, -22.4766f), new Vector3(0, 180f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                    posterData[1] = (new Vector3(9.5975f, 2.5063f, -5.8245f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                    posterData[2] = (new Vector3(10.1364f, 2.8591f, -22.4788f), new Vector3(0, 180f, 356.3345f), new Vector3(0.7487f, 1.0539f, 1f), "Poster3");
                    posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                    posterData[4] = (new Vector3(7.5475f, 2.5882f, -22.4803f), new Vector3(0, 180f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                    posterData[5] = (new Vector3(-5.8111f, 2.541f, -17.577f), new Vector3(0, 270.0942f, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                }
                else
                {
                    // If only 2 Story Mod is detected, use its config
                    Plugin.Log.LogInfo("Repositioning posters due to 2 Story Ship Mod detected");

                    // If all windows are enabled (default behavior)
                    if (Plugin.Service.EnableRightWindows && Plugin.Service.EnableLeftWindows)
                    {
                        Plugin.Log.LogInfo("Repositioning posters due to 2 Story Ship Mod Left and Right windows enabled");
                        posterData[0] = (new Vector3(10.1567f, 2.75f, -8.8293f), new Vector3(0, 0, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                        posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                        posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                        posterData[4] = (new Vector3(6.1473f, 2.8195f, -17.4729f), new Vector3(0, 179.7123f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                        posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                    }
                    else
                    {
                        // Reposition posters if right windows are disabled
                        if (!Plugin.Service.EnableRightWindows)
                        {
                            Plugin.Log.LogInfo("Repositioning posters due to 2 Story Ship Mod Right window disabled");
                            posterData[0] = (new Vector3(4.0286f, 2.9318f, -16.7774f), new Vector3(0, 200.9872f, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                            posterData[1] = (new Vector3(9.0884f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                            posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 0), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                            posterData[4] = (new Vector3(5.3282f, 2.7482f, -17.2754f), new Vector3(0, 202.3357f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                            posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        }

                        // Reposition posters if left windows are disabled
                        if (!Plugin.Service.EnableLeftWindows)
                        {
                            Plugin.Log.LogInfo("Repositioning posters due to 2 Story Ship Mod Left window disabled");
                            posterData[0] = (new Vector3(9.8324f, 2.9318f, -8.8257f), new Vector3(0, 0, 0), new Vector3(0.6391f, 0.4882f, 2f), "Poster1");
                            posterData[1] = (new Vector3(7.3648f, 2.4776f, -8.8229f), new Vector3(0, 0, 0), new Vector3(0.7296f, 0.4896f, 1f), "Poster2");
                            posterData[3] = (new Vector3(5.3599f, 2.5963f, -9.455f), new Vector3(0, 307.2657f, 2.68f), new Vector3(0.7289f, 0.9989f, 1f), "Poster4");
                            posterData[4] = (new Vector3(6.1473f, 2.8195f, -17.4729f), new Vector3(0, 179.7123f, 359.8f), new Vector3(0.5516f, 0.769f, 1f), "Poster5");
                            posterData[5] = (new Vector3(2.5679f, 2.6763f, -11.7341f), new Vector3(0, 0, 358.6752f), new Vector3(0.8596f, 1.2194f, 1f), "CustomTips");
                        }
                    }
                }
            }

            var enabledPacks = Plugin.Service.PosterFolders.Where(folder => PosterConfig.IsPackEnabled(folder)).ToList();
            if (enabledPacks.Count == 0)
            {
                Plugin.Log.LogWarning("No enabled packs found");
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                }
                yield break;
            }

            var enabledPackNames = enabledPacks.Select(pack => Path.GetFileName(pack)).ToList();

            List<string> packsToUse;
            if (PosterConfig.RandomizerModeSetting.Value == PosterConfig.RandomizerMode.PerPack)
            {
                if (!PosterConfig.PerSession.Value || _selectedPack == null || !enabledPacks.Contains(_selectedPack))
                {
                    var packChances = enabledPacks.Select(p => PosterConfig.GetPackChance(p)).ToList();
                    if (packChances.Any(c => c > 0))
                    {
                        var totalChance = packChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < enabledPacks.Count; i++)
                        {
                            cumulative += packChances[i];
                            if (randomValue <= cumulative)
                            {
                                _selectedPack = enabledPacks[i];
                                break;
                            }
                        }
                        _selectedPack ??= enabledPacks[0];
                    }
                    else
                    {
                        _selectedPack = enabledPacks[Plugin.Service.Rand.Next(enabledPacks.Count)];
                    }
                    var selectedPackName = Path.GetFileName(_selectedPack);
                    Plugin.Log.LogInfo($"PerPack randomization enabled. Using pack: {selectedPackName} [Chances: {string.Join(", ", packChances)}]");
                }
                packsToUse = new List<string> { _selectedPack };
            }
            else
            {
                if (!PosterConfig.PerSession.Value)
                {
                    _selectedPack = null;
                }
                packsToUse = enabledPacks;
                Plugin.Log.LogInfo("PerPoster - true, combining enabled packs");
            }

            var allTextures = new Dictionary<string, List<(Texture2D texture, string filePath)>>();
            var allVideos = new Dictionary<string, List<string>>();
            foreach (var pack in packsToUse)
            {
                string packName = Path.GetFileName(pack);
                string postersPath = Path.Combine(pack, "posters");
                string tipsPath = Path.Combine(pack, "tips");
                string nestedPostersPath = Path.Combine(pack, "CustomPosters", "posters");
                string nestedTipsPath = Path.Combine(pack, "CustomPosters", "tips");

                var filesToLoad = new List<string>();
                var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".mp4" };

                var pathsToCheck = new[] { postersPath, tipsPath, nestedPostersPath, nestedTipsPath }
                    .Where(p => Directory.Exists(p))
                    .Select(p => Path.GetFullPath(p).Replace('\\', '/'))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var path in pathsToCheck)
                {
                    var files = Directory.GetFiles(path)
                        .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()) && PosterConfig.IsFileEnabled(f))
                        .Select(f => Path.GetFullPath(f).Replace('\\', '/'))
                        .ToList();

                    filesToLoad.AddRange(files);
                }

                filesToLoad = filesToLoad.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                const int batchSize = 5;
                for (int i = 0; i < filesToLoad.Count; i += batchSize)
                {
                    var batch = filesToLoad.Skip(i).Take(batchSize).ToList();
                    foreach (var file in batch)
                    {
                        if (Path.GetExtension(file).ToLower() == ".mp4")
                        {
                            var posterName = Path.GetFileNameWithoutExtension(file).ToLower();
                            if (!allVideos.ContainsKey(posterName))
                            {
                                allVideos[posterName] = new List<string>();
                            }
                            allVideos[posterName].Add(file);
                            Plugin.Service.CacheVideo(file);
                        }
                        else
                        {
                            yield return LoadTextureAsync(file, (result) =>
                            {
                                if (result.texture != null)
                                {
                                    var posterName = Path.GetFileNameWithoutExtension(file).ToLower();
                                    if (!allTextures.ContainsKey(posterName))
                                    {
                                        allTextures[posterName] = new List<(Texture2D, string)>();
                                    }
                                    allTextures[posterName].Add((result.texture, file));
                                }
                                else
                                {
                                    Plugin.Log.LogWarning($"Failed to load texture from {file}");
                                }
                            });
                        }
                    }
                    yield return null;
                }
            }

            var prioritizedContent = new Dictionary<string, (Texture2D texture, string filePath, bool isVideo)>();
            foreach (var kvp in allTextures)
            {
                if (kvp.Value.Count > 1)
                {
                    var fileChances = kvp.Value.Select(t => PosterConfig.GetFileChance(t.filePath)).ToList();
                    if (fileChances.Any(c => c > 0))
                    {
                        var totalChance = fileChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            cumulative += fileChances[i];
                            if (randomValue <= cumulative)
                            {
                                prioritizedContent[kvp.Key] = (kvp.Value[i].texture, kvp.Value[i].filePath, false);
                                break;
                            }
                        }
                        if (!prioritizedContent.ContainsKey(kvp.Key))
                        {
                            prioritizedContent[kvp.Key] = (kvp.Value[0].texture, kvp.Value[0].filePath, false);
                        }
                    }
                    else
                    {
                        var selected = kvp.Value.OrderBy(t => Plugin.Service.GetFilePriority(t.filePath)).First();
                        prioritizedContent[kvp.Key] = (selected.texture, selected.filePath, false);
                    }
                }
                else
                {
                    prioritizedContent[kvp.Key] = (kvp.Value[0].texture, kvp.Value[0].filePath, false);
                }
            }

            foreach (var kvp in allVideos)
            {
                if (kvp.Value.Count > 1)
                {
                    var fileChances = kvp.Value.Select(v => PosterConfig.GetFileChance(v)).ToList();
                    if (fileChances.Any(c => c > 0))
                    {
                        var totalChance = fileChances.Sum();
                        var randomValue = Plugin.Service.Rand.NextDouble() * totalChance;
                        double cumulative = 0;
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            cumulative += fileChances[i];
                            if (randomValue <= cumulative)
                            {
                                prioritizedContent[kvp.Key] = (null, kvp.Value[i], true);
                                break;
                            }
                        }
                        if (!prioritizedContent.ContainsKey(kvp.Key))
                        {
                            prioritizedContent[kvp.Key] = (null, kvp.Value[0], true);
                        }
                    }
                    else
                    {
                        var selected = kvp.Value.OrderBy(v => Plugin.Service.GetFilePriority(v)).First();
                        prioritizedContent[kvp.Key] = (null, selected, true);
                    }
                }
                else
                {
                    prioritizedContent[kvp.Key] = (null, kvp.Value[0], true);
                }
            }

            if (allTextures.Count == 0 && allVideos.Count == 0)
            {
                Plugin.Log.LogWarning("No textures or videos found in enabled packs");
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                }
                yield break;
            }

            bool anyPosterLoaded = false;

            for (int i = 0; i < posterData.Length; i++)
            {
                var poster = CreatePoster();
                if (poster == null)
                {
                    continue;
                }
                poster.name = posterData[i].name;
                poster.transform.SetParent(postersParent.transform);

                poster.transform.position = posterData[i].position;
                poster.transform.rotation = Quaternion.Euler(posterData[i].rotation);
                poster.transform.localScale = posterData[i].scale;

                var posterKey = posterData[i].name.ToLower();
                if (prioritizedContent.ContainsKey(posterKey) && PosterConfig.IsFileEnabled(prioritizedContent[posterKey].filePath))
                {
                    var renderer = poster.AddComponent<PosterRenderer>();
                    var (texture, filePath, isVideo) = prioritizedContent[posterKey];
                    renderer.Initialize(texture, isVideo ? filePath : null, _copiedMaterial);

                    CreatedPosters.Add(poster);
                    anyPosterLoaded = true;

                }
                else
                {
                    Plugin.Log.LogWarning($"No enabled texture or video found for {posterData[i].name}. Destroying the poster");
                    UnityEngine.Object.Destroy(poster);
                }
                yield return null;
            }

            if (anyPosterLoaded)
            {
                if (posterPlane != null)
                {
                    UnityEngine.Object.Destroy(posterPlane);
                }
                var vanillaPlane = hangarShip.transform.Find("Plane")?.gameObject;
                if (vanillaPlane != null)
                {
                    UnityEngine.Object.Destroy(vanillaPlane);
                }
                Plugin.Log.LogInfo("Custom posters created successfully");
            }
            else
            {
                if (posterPlane != null)
                {
                    posterPlane.SetActive(true);
                    Plugin.Log.LogWarning("Re-enabled vanilla Plane.001 poster due to no custom posters loaded");
                }
            }
        }

        private static IEnumerator DelayedUpdateMaterialsAsync(StartOfRound instance)
        {
            if (_materialsUpdated)
                yield break;

            yield return new WaitForEndOfFrame();

            var posterPlane = GameObject.Find("Environment/HangarShip/Plane.001");
            if (posterPlane != null)
            {
                posterPlane.SetActive(false);
            }

            HideVanillaPosterPlane();
            yield return instance.StartCoroutine(CreateCustomPostersAsync());
            _materialsUpdated = true;
        }

        public static void ChangePosterPack(string packName)
        {
            if (string.IsNullOrEmpty(packName))
            {
                var enabledPacks = Plugin.Service.GetEnabledPackNames();
                if (enabledPacks.Count == 0) return;

                int currentIndex = enabledPacks.FindIndex(p => p.Equals(_selectedPack, StringComparison.OrdinalIgnoreCase));
                _selectedPack = enabledPacks[(currentIndex + 1) % enabledPacks.Count];
            }
            else
            {
                if (Plugin.Service.GetEnabledPackNames().Contains(packName, StringComparer.OrdinalIgnoreCase))
                {
                    _selectedPack = packName;
                }
                else
                {
                    Plugin.Log.LogWarning($"Attempted to select invalid pack: {packName}");
                    return;
                }
            }

            Plugin.Service.SetRandomSeed(Environment.TickCount);
            Plugin.Log.LogInfo($"Changed poster pack to - {_selectedPack}");

            _materialsUpdated = false;
            StartOfRound instance = FindObjectOfType<StartOfRound>();
            if (instance != null && instance.inShipPhase)
            {
                instance.StartCoroutine(DelayedUpdateMaterialsAsync(instance));
            }
        }
    }
}