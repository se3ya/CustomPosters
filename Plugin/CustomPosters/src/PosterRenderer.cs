using UnityEngine;
using UnityEngine.Video;

namespace CustomPosters
{
    public class PosterRenderer : MonoBehaviour
    {
        private VideoPlayer? _videoPlayer;
        private AudioSource? _audioSource;
        private RenderTexture? _renderTexture;

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
                Plugin.Log.LogError("Cannot initialize poster, material template is null!");
                Destroy(gameObject);
                return;
            }

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
                    Plugin.Log.LogError($"VideoPlayer error for {videoPath}: {message}");
                };
                _videoPlayer.prepareCompleted += (player) =>
                {
                    Plugin.Log.LogDebug($"Video prepared successfully: {videoPath}");
                    player.Play();
                };

                if (Plugin.ModConfig.EnableVideoAudio.Value)
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