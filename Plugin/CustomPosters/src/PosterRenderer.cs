using UnityEngine;
using UnityEngine.Video;
using CustomPosters.Utils;

namespace CustomPosters
{
    public class PosterRenderer : MonoBehaviour
    {
        private VideoPlayer? _videoPlayer;
        private AudioSource? _audioSource;
        private RenderTexture? _renderTexture;
        private float _originalVolume;

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

        public void Initialize(Texture2D? texture, string? videoPath, Material? materialTemplate)
        {
            if (materialTemplate == null)
            {
                Plugin.Log.LogError("Cannot initialize poster, material template is null.");
                Destroy(gameObject);
                return;
            }

            var renderer = GetComponent<MeshRenderer>();
            var material = new Material(materialTemplate);
    
            renderer.material = material;

            if (videoPath != null && System.IO.Path.GetExtension(videoPath).ToLower() == ".mp4")
            {
                _renderTexture = new RenderTexture(512, 512, 16);

                material.SetTexture("_BaseColorMap", _renderTexture);

                _videoPlayer = gameObject.AddComponent<VideoPlayer>();
                _videoPlayer.url = "file://" + videoPath;
                _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                _videoPlayer.targetTexture = _renderTexture;
                var (volume, maxDistance, aspectRatio) = Plugin.ModConfig.GetFileAudioSettings(videoPath);
                _videoPlayer.aspectRatio = ConvertAspectRatio(aspectRatio);
                _videoPlayer.isLooping = true;
                _videoPlayer.playOnAwake = false;

                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1.0f;
                _audioSource.spatialize = false;
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
                    Plugin.Log.LogError($"VideoPlayer error for {PathUtils.GetPrettyPath(videoPath)}: {message}");
                };
                _videoPlayer.prepareCompleted += (player) =>
                {
                    player.Play();

                    if (Plugin.ModConfig.EnableNetworking.Value && Unity.Netcode.NetworkManager.Singleton != null && !Unity.Netcode.NetworkManager.Singleton.IsHost)
                    {
                        Networking.PosterSyncManager.RequestVideoTimeFromServer(gameObject.name);
                    }
                };

                _originalVolume = volume / 100.0f;

                if (Plugin.ModConfig.EnableVideoAudio.Value)
                {
                    _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                    _videoPlayer.SetTargetAudioSource(0, _audioSource);
                    _audioSource.volume = _originalVolume;
                }
                else
                {
                    _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    _audioSource.volume = 0f;
                }

                _videoPlayer.Prepare();
            }
            else if (texture != null)
            {
                material.SetTexture("_BaseColorMap", texture);
            }
            else
            {
                Plugin.Log.LogError($"No valid texture or video for poster: {gameObject.name}");
                Destroy(gameObject);
            }
        }

        public void SetVideoTime(double time)
        {
            if (_videoPlayer != null && _videoPlayer.isPrepared)
            {
                _videoPlayer.time = time;
            }
        }

        public double? GetCurrentVideoTime()
        {
            if (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                return _videoPlayer.time;
            }
            return null;
        }

        private void Update()
        {
            if (_audioSource != null && !Plugin.ModConfig.EnableVideoAudio.Value)
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
}