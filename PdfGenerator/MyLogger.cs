using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfGenerator
{
    public class MyLogger
    {
        public static Action<string> Action = (s) => Console.WriteLine(s);


        public static void LogInfo(string text)
        {
            Action(text);
        }

        public static void LogErrror(string text)
        {
            Action(text);
        }

        public static void LogDuration(string tag, Action action)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                action();
            }
            catch(Exception e)
            {
                LogErrror($"{tag}: Error: {e.Message}");
            }
            finally
            {
                watch.Stop();
                LogInfo($"{tag}: Duration: {watch.ElapsedMilliseconds}");
            }
        }

        public static T LogDuration<T>(string tag, Func<T> func)
        {
            T result = default(T);
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                result = func();
            }
            catch (Exception e)
            {
                LogErrror($"{tag}: Error: {e.Message}");
            }
            finally
            {
                watch.Stop();
                LogInfo($"{tag}: Duration: {watch.ElapsedMilliseconds}");
            }
            return result;
        }
    }
}
