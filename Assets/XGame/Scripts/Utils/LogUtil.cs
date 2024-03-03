using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XGame
{
    public static class LogUtil
    {
        public const int LogLevelInfo = 1;
        public const int LogLevelWarning = 1 << 1;
        public const int LogLevelError = 1 << 2;
        public const int LogLevelBreak = 1 << 3;
        public const int LogLevelAssert = 1 << 4;

        public static int LogLevel = LogLevelInfo | LogLevelWarning | LogLevelError | LogLevelBreak | LogLevelAssert;

        public static void Log(string message)
        {
            if ((LogLevel & LogLevelInfo) > 0)
            {
                Debug.Log(message);
            }
        }
        public static void Warning(string message)
        {
            if ((LogLevel & LogLevelWarning) > 0)
            {
                Debug.LogWarning(message);
            }
        }
        public static void Err(string message)
        {
            if ((LogLevel & LogLevelError) > 0)
            {
                Debug.LogError(message);
            }
        }
        public static void Break()
        {
            if ((LogLevel & LogLevelBreak) > 0)
            {
                Debug.Break();
            }
        }
        public static void Assert(bool condition, string message)
        {
            if ((LogLevel & LogLevelAssert) > 0)
            {
                Debug.Assert(condition, message);
            }
        }

    }
}

