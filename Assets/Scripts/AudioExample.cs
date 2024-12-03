using JamForge;
using JamForge.Audio;
using UnityEngine;

/// <summary>
/// Demonstrates basic audio playback functionality using JamForge's audio system.
/// This example shows how to:
/// - Play background music
/// - Play sound effects on user input
/// - Control audio playback (pause, resume, stop)
/// </summary>
public class AudioExample : MonoBehaviour
{
    [SerializeField] private AudioDefine audioDefine; // Background music definition
    [SerializeField] private AudioDefine audioDefineEffect; // Sound effect definition

    private IAudioHandle _audioHandle; // Handle for controlling audio playback
    
    private void Start()
    {
        // Play background music on start
        _audioHandle = Jam.Audio.Play(audioDefine);
    }

    private void Update()
    {
        // Play sound effect when spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jam.Audio.PlayOneShot(audioDefineEffect);
        }
        
        // Toggle pause/resume when 'P' key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_audioHandle.IsPlaying)
            {
                _audioHandle.Pause();   
            }
            else
            {
                _audioHandle.Resume();
            }
        }

        // Stop audio playback when 'S' key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            _audioHandle.Stop();
        }
    }
}
