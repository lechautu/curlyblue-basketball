using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a rolling buffer of ball positions and rotations for the last X seconds.
/// </summary>
public class BallTracker : MonoBehaviour
{
    [SerializeField] private float _bufferDuration = 1.0f;
    
    private struct BallFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Timestamp;

        public BallFrame(Vector3 pos, Quaternion rot, float time)
        {
            Position = pos;
            Rotation = rot;
            Timestamp = time;
        }
    }

    private LinkedList<BallFrame> _frameBuffer = new LinkedList<BallFrame>();

    private void Update()
    {
        // Record current frame
        _frameBuffer.AddLast(new BallFrame(transform.position, transform.rotation, Time.time));

        // Trim old frames
        while (_frameBuffer.Count > 0 && (Time.time - _frameBuffer.First.Value.Timestamp) > _bufferDuration)
        {
            _frameBuffer.RemoveFirst();
        }
    }

    /// <summary>
    /// Returns a copy of the recorded frames for replay.
    /// </summary>
    public List<Vector3> GetPositionFrames()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var frame in _frameBuffer) positions.Add(frame.Position);
        return positions;
    }

    public List<(Vector3 pos, Quaternion rot)> GetTotalFrames()
    {
        var frames = new List<(Vector3, Quaternion)>();
        foreach (var frame in _frameBuffer)
        {
            frames.Add((frame.Position, frame.Rotation));
        }
        return frames;
    }
}
