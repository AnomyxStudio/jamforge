using JamForge;
using UnityEngine;

/// <summary>
/// Demonstrates logging functionality using JamForge's logging system.
/// This example shows how to:
/// - Obtain a logger instance
/// - Log messages at various severity levels
/// </summary>
public class LogExample : MonoBehaviour
{
    private void Start()
    {
        // Obtain a logger instance for this class
        var logger = Jam.LogManager.GetLogger<LogExample>();
        
        // Log messages at different levels
        logger.Trace("Trace");   // Detailed information, typically of interest only when diagnosing problems
        logger.Debug("Debug");   // Information useful to developers for debugging the application
        logger.Info("Info");     // Informational messages that highlight the progress of the application
        logger.Warn("Warn");     // Potentially harmful situations
        logger.Error("Error");   // Error events that might still allow the application to continue running
        logger.Fatal("Fatal");   // Very severe error events that will presumably lead the application to abort
    }
}
