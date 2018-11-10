using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace Elmanager
{
    internal static class Utils
    {
        internal const int BcmFirst = 0x1600; //Normal button
        internal const int BcmSetshield = (BcmFirst + 0x000C); //Elevated button
        private static bool _scrollInProgress;

        public static string HttpUploadFile(string url, string file, string paramName, string contentType,
            NameValueCollection nvc = null)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest) WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();
            const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            if (nvc != null)
            {
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = String.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            rs.Write(boundarybytes, 0, boundarybytes.Length);
            const string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = String.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();
            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                string response = reader2.ReadToEnd();
                reader2.Close();
                return response;
            }
            catch (Exception)
            {
                wresp?.Close();
                return null;
            }
        }

        internal static void AddShieldToButton(Button b)
        {
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BcmSetshield, 0, 0xFFFFFFFF);
        }

        internal static void BeginArrowScroll(ElmaRenderer renderer)
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
                    renderer.CenterY += timeDelta / 200.0 * renderer.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    renderer.CenterY -= timeDelta / 200.0 * renderer.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    renderer.CenterX += timeDelta / 200.0 * renderer.ZoomLevel;
                }

                if (Keyboard.IsKeyDown(Key.Left))
                {
                    renderer.CenterX -= timeDelta / 200.0 * renderer.ZoomLevel;
                }

                lastTime = timer.ElapsedMilliseconds;
                renderer.RedrawScene();
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
                Process.Start(destFile);
            }
            catch (WebException)
            {
                ShowError("Failed to download file " + uri);
            }
        }

        internal static List<string> GetLevelFiles(bool searchSubDirs)
        {
            if (Directory.Exists(Global.AppSettings.General.LevelDirectory))
            {
                string[] files = Directory.GetFiles(Global.AppSettings.General.LevelDirectory, Constants.AllLevs,
                    searchSubDirs
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly);
                Array.Sort(files);
                List<string> stringList = new List<string>();
                stringList.AddRange(files);
                return stringList;
            }

            return new List<string>();
        }

        internal static List<string> GetLevelFiles()
        {
            return GetLevelFiles(Global.AppSettings.ReplayManager.SearchLevSubDirs);
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

        internal static bool LoadDataBase()
        {
            try
            {
                BinaryFormatter binFormatter = new BinaryFormatter();
                byte[] serializedData = File.ReadAllBytes(Global.AppSettings.ReplayManager.DbFile);
                MemoryStream ms = new MemoryStream(serializedData);
                Global.ReplayDataBase = (List<Replay>) binFormatter.Deserialize(ms);
                ms.Close();
                return true;
            }
            catch (FileNotFoundException)
            {
                ShowError("The specified database file " + Global.AppSettings.ReplayManager.DbFile +
                          " doesn\'t exist!");
                return false;
            }
            catch (Exception)
            {
                ShowError("Failed to load database file " + Global.AppSettings.ReplayManager.DbFile);
                return false;
            }
        }

        internal static void PutEventsToList(Player player, ListBox listBox, bool finished,
            PlayerEvent[] selectedEvents)
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
                    case ReplayEventType.AppleTake:
                        appleCounter++;
                        strToAdd = "Apple " + appleCounter;
                        break;
                    case ReplayEventType.SuperVolt:
                        superVoltCounter++;
                        strToAdd = "Supervolt " + superVoltCounter;
                        break;
                    case ReplayEventType.LeftVolt:
                        leftVoltCounter++;
                        strToAdd = "Left volt " + leftVoltCounter;
                        break;
                    case ReplayEventType.RightVolt:
                        rightVoltCounter++;
                        strToAdd = "Right volt " + rightVoltCounter;
                        break;
                    case ReplayEventType.Turn:
                        turnCounter++;
                        strToAdd = "Turn " + turnCounter;
                        break;
                    case ReplayEventType.GroundTouch:
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

        internal static string SizeToString(object x)
        {
            return ((int) x / 1024.0).ToString("F2");
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

        internal static string ToTimeString(object x)
        {
            return ToTimeString((double) x);
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
    }
}