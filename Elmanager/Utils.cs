using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using BrightIdeasSoftware;

namespace Elmanager
{
    internal static class Utils
    {
        private static bool _scrollInProgress;

        internal static void BeginArrowScroll(ElmaRenderer renderer, ZoomController zoomCtrl, SceneSettings sceneSettings)
        {
            if (_scrollInProgress)
                return;
            _scrollInProgress = true;
            var timer = new Stopwatch();
            timer.Start();
            long lastTime = timer.ElapsedMilliseconds;
            while (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Left) ||
                   Keyboard.IsKeyDown(Key.Right))
            {
                long timeDelta = timer.ElapsedMilliseconds - lastTime;
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    zoomCtrl.CenterY += timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    zoomCtrl.CenterY -= timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    zoomCtrl.CenterX += timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Left))
                {
                    zoomCtrl.CenterX -= timeDelta / 200.0 * zoomCtrl.ZoomLevel;
                }

                lastTime = timer.ElapsedMilliseconds;
                renderer.DrawScene(zoomCtrl.Cam, sceneSettings);
                renderer.Swap();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            timer.Stop();
            _scrollInProgress = false;
        }

        internal static string BoolToString(object x)
        {
            return (bool) x ? "Yes" : "No";
        }

        internal static int BooleanToInteger(bool b)
        {
            return b ? 1 : 0;
        }

        internal static bool CompareWith(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        internal static void DownloadAndOpenFile(string uri, string destFile, string username = null,
            string password = null)
        {
            var wc = new WebClient();
            if (username != null && password != null)
                wc.Credentials = new NetworkCredential(username, password);
            try
            {
                wc.DownloadFile(uri, destFile);
                Utils.ShellExecute(destFile);
            }
            catch (WebException)
            {
                ShowError("Failed to download file " + uri);
            }
        }

        internal static List<string> GetLevelFiles(SearchOption searchSubDirs)
        {
            if (Directory.Exists(Global.AppSettings.General.LevelDirectory))
            {
                string[] files = Directory.GetFiles(Global.AppSettings.General.LevelDirectory, Constants.AllLevs,
                    searchSubDirs);
                Array.Sort(files);
                return files.ToList();
            }

            return new List<string>();
        }

        internal static string GetPlayerInfoStr(Player p)
        {
            return "Apples: " + p.Apples + "\r\n" + "Left volts: " + p.LeftVolts + "\r\n" + "Right volts: " +
                   p.RightVolts + "\r\n" + "Supervolts: " + p.SuperVolts + "\r\n" + "Turns: " + p.Turns + "\r\n" +
                   "Groundtouches: " + p.GroundTouches + "\r\n" + "Top speed: " + Math.Round(p.TopSpeed) + "\r\n" +
                   "Trip: " + Math.Round(p.Trip);
        }

        internal static string GetPossiblyInternal(object x)
        {
            return Level.GetPossiblyInternal((string) x);
        }

        internal static bool LevDirectoryExists()
        {
            return Directory.Exists(Global.AppSettings.General.LevelDirectory);
        }

        internal static bool LevRecDirectoriesExist()
        {
            return RecDirectoryExists() && LevDirectoryExists();
        }

        internal static void PutEventsToList(Player player, ListBox listBox, bool finished,
            List<PlayerEvent<LogicalEventType>> selectedEvents)
        {
            int turnCounter = 0;
            int leftVoltCounter = 0;
            int rightVoltCounter = 0;
            int superVoltCounter = 0;
            int gtCounter = 0;
            int appleCounter = 0;
            double lastEventTime = 0;
            listBox.Items.Clear();
            foreach (var e in selectedEvents)
            {
                double eventTime = e.Time;
                string strToAdd;
                switch (e.Type)
                {
                    case LogicalEventType.AppleTake:
                        appleCounter++;
                        strToAdd = "Apple " + appleCounter;
                        break;
                    case LogicalEventType.SuperVolt:
                        superVoltCounter++;
                        strToAdd = "Supervolt " + superVoltCounter;
                        break;
                    case LogicalEventType.LeftVolt:
                        leftVoltCounter++;
                        strToAdd = "Left volt " + leftVoltCounter;
                        break;
                    case LogicalEventType.RightVolt:
                        rightVoltCounter++;
                        strToAdd = "Right volt " + rightVoltCounter;
                        break;
                    case LogicalEventType.Turn:
                        turnCounter++;
                        strToAdd = "Turn " + turnCounter;
                        break;
                    case LogicalEventType.GroundTouch:
                        gtCounter++;
                        strToAdd = "Touch " + gtCounter;
                        break;
                    default:
                        throw new Exception("Unknown ReplayEventType.");
                }

                listBox.Items.Add(strToAdd + ": " + eventTime.ToTimeString() + " + " +
                                  (eventTime - lastEventTime).ToTimeString());
                lastEventTime = eventTime;
            }

            if ((finished && player.FakeFinish) || player.Finished)
                listBox.Items.Add("Flower: " + ToTimeString(player.Time) + " + " +
                                  (player.Time - lastEventTime).ToTimeString());
        }

        internal static string ReadNullTerminatedString(byte[] data, int startIndex, int initialSize)
        {
            byte[] tempBytes = new byte[initialSize];
            for (int j = startIndex; j < startIndex + initialSize; j++)
            {
                if (data[j] != 0)
                    tempBytes[j - startIndex] = data[j];
                else
                {
                    Array.Resize(ref tempBytes, j - startIndex);
                    break;
                }
            }

            return Encoding.ASCII.GetString(tempBytes);
        }

        internal static string ReadNullTerminatedString(this BinaryReader reader, int count)
        {
            var chars = reader.ReadChars(count);
            return new string(chars.TakeWhile(c => c != '\0').ToArray());
        }

        internal static string ReadString(this BinaryReader reader, int count)
        {
            return new string(reader.ReadChars(count));
        }

        internal static bool RecDirectoryExists()
        {
            return Directory.Exists(Global.AppSettings.General.ReplayDirectory);
        }

        [DllImport("user32")]
        internal static extern UInt32 SendMessage
            (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        /// <summary>
        ///   Display a message box to indicate that an error occurred.
        /// </summary>
        /// <param name = "text">The text to display in the message box.</param>
        /// <param name = "caption">The title of the message box.</param>
        /// <param name = "icon">The icon to display in the message box.</param>
        /// <returns></returns>
        internal static void ShowError(string text, string caption = "Error", MessageBoxIcon icon = MessageBoxIcon.Hand)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///   Converts the given string to a double number. The string must be of the form 00:00,000 or 00:00,00.
        /// </summary>
        /// <param name = "timeStr">The string to be converted.</param>
        /// <returns>The time string as a double number.</returns>
        internal static double StringToTime(string timeStr)
        {
            return Double.Parse(timeStr.Substring(0, 2)) * 60 + Double.Parse(timeStr.Substring(3, 2)) +
                   Double.Parse(timeStr.Substring(6)) * 0.001;
        }

        /// <summary>
        ///   Converts the given time (in seconds) to a string representation. The time string is of the form 0:00:00,000.
        /// </summary>
        /// <param name = "time">The time to be converted in seconds.</param>
        /// <param name = "digits">Number of decimal digits to display in the string.</param>
        /// <returns></returns>
        internal static string ToTimeString(this double time, int digits = 3)
        {
            double T = Math.Abs(time);
            StringBuilder timeStr = new StringBuilder(9);
            int minutes = (int) Math.Floor(T / 60);
            int hours = (int) Math.Floor(T / 3600);
            if (hours > 0)
            {
                timeStr.Append(hours);
                timeStr.Append(":");
                minutes -= 60 * hours;
            }

            double timeVar = T - (60 * minutes + 3600 * hours);
            timeStr.Append(minutes.ToString("D2"));
            timeStr.Append(":");
            switch (digits)
            {
                case 3:
                    timeStr.Append(String.Format("{0:00.000}", timeVar));
                    break;
                case 2:
                    timeStr.Append(String.Format("{0:00.00}", timeVar));
                    break;
                default:
                    throw new Exception("Invalid number of digits!");
            }

            if (time < 0)
                timeStr.Insert(0, '-');
            return timeStr.ToString();
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo propInfo;
            do
            {
                propInfo = type.GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            } while (propInfo == null && type != null);

            return propInfo;
        }

        internal static object GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var objType = obj.GetType();
            var propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException(nameof(propertyName),
                    $"Couldn't find property {propertyName} in type {objType.FullName}");
            return propInfo.GetValue(obj, null);
        }

        public static void ConfigureColumns<T>(ObjectListView oList, bool addIndexColumn = false,
            IEnumerable<string> hiddenColumns = null)
        {
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var cols = new List<OLVColumn>();
            if (addIndexColumn)
            {
                cols.Add(new OLVColumn("#", null));
                oList.FormatRow += (sender, args) => { args.Item.Text = (args.RowIndex + 1).ToString(); };
            }

            var hiddens = new HashSet<string>();
            if (hiddenColumns != null)
            {
                foreach (var c in hiddenColumns)
                {
                    hiddens.Add(c);
                }
            }

            foreach (var m in members)
            {
                var descs = (DescriptionAttribute[]) m.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descs.Length == 0)
                {
                    continue;
                }

                var desc = descs[0];
                var col = new OLVColumn(desc.Description, m.Name);
                Type t;
                switch (m)
                {
                    case FieldInfo fieldInfo:
                        t = fieldInfo.FieldType;
                        break;
                    case MethodInfo methodInfo:
                        t = methodInfo.ReturnType;
                        break;
                    case PropertyInfo propertyInfo:
                        t = propertyInfo.PropertyType;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(m));
                }

                if (t == typeof(bool))
                {
                    col.AspectToStringConverter = BoolToString;
                }
                else if (t == typeof(double))
                {
                    col.AspectToStringConverter = d => $"{d:F2}";
                }

                if (hiddens.Contains(col.Text))
                {
                    col.IsVisible = false;
                }

                cols.Add(col);
            }

            var i = cols.FindIndex(c => c.AspectName == nameof(ElmaObject.FileNameNoExt));
            if (i >= 0)
            {
                var colf = cols[i];
                cols.RemoveAt(i);
                cols.Insert(0, colf);
            }

            oList.AllColumns = cols;
            oList.RebuildColumns();
        }

        public static void ConfigureTooltip(ToolTipControl toolTip)
        {
            toolTip.IsBalloon = true;
            toolTip.AutoPopDelay = 30000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 0;
            toolTip.Title = "Details";
        }

        public static IEnumerable<string> FilterByRegex(IEnumerable<string> files, string pattern)
        {
            Regex matcher;
            try
            {
                matcher = new Regex(pattern, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                matcher = new Regex(String.Empty, RegexOptions.IgnoreCase);
            }

            return files.Where(x =>
            {
                var f = Path.GetFileNameWithoutExtension(x);
                return f != null && matcher.IsMatch(f);
            });
        }

        public static Range<double> GetTimeRange(string timeMin, string timeMax)
        {
            return new Range<double>(StringToTime(timeMin),
                StringToTime(timeMax));
        }

        public static bool Confirm(string text)
        {
            return MessageBox.Show(text, "Elmanager", MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) == DialogResult.Yes;
        }

        internal static void ShellExecute(string url)
        {
            Process.Start(
                new ProcessStartInfo(url)
                    {UseShellExecute = true});
        }

        internal static double GetFirstGridLine(double size, double offset, double min)
        {
            var tmp = (Math.Floor(min / size) + 1) * size;
            var left = (tmp - (size + offset));
            return left;
        }
    }
}