using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Ipte.Machine.Config
{
    public class SegmentSettings
    {
        public int InitDelay { get; set; } = 3000;
        public int LoadingDelay { get; set; } = 500;
        public int UnloadingDelay { get; set; } = 1000;
        public string ActiveRecipe { get; set; }
        public bool SimulationMode { get; set; } = true;
    }

    public class MachineSettings : ICloneable
    {
        public MachineSettings()
        {
            LoadingSegment = new SegmentSettings();
            MiddleSegment = new SegmentSettings();
            UnloadingSegment = new SegmentSettings();
        }

        public string AdsAmsAddress { get; set; }

        public SegmentSettings LoadingSegment { get; set; }
        public SegmentSettings MiddleSegment { get; set; }
        public SegmentSettings UnloadingSegment { get; set; }

        [Description("Log in after given amount of setting")]
        public double AutoLogOffTimeS { get; set; } = 3600;

        #region Serialization

        /// <summary>
        /// Loads settings from file. Throws exception when fails;
        /// </summary>
        /// <remarks>Relies on xml serialization</remarks>
        /// <returns>Loaded instance, may also be null or throw exception if fails.</returns>
        public static MachineSettings Load(string path)
        {
            if (!File.Exists(path)) return new MachineSettings();
            using (FileStream stream = File.OpenRead(path))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Loads current configuration from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MachineSettings Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MachineSettings));
            var result = serializer.Deserialize(stream) as MachineSettings;
            return result;
        }

        /// <summary>
        /// Saves current configuration to an xml file.
        /// </summary>
        /// <remarks>Relies on xml serialization</remarks>
        public void Save(string path)
        {
            using (FileStream file = File.Open(path, FileMode.Create))
            {
                Save(file);
            }

            ////commit changes to svn
            //try
            //{
            //    SvnCommitArgs args = new SvnCommitArgs();
            //    SvnCommitResult result;
            //    args.LogMessage = "Saving changes to settings file.\r\nUser: " + GuiControlLibrary.AccessManager.UserName;

            //    using (SvnClient client = new SvnClient())
            //    {

            //        SvnUI.Bind(client, new SvnUIBindArgs());
            //        client.Commit(Paths.SettingsDirectory, args, out result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Trace.TraceError("Failed to commit changes to svn reprository: {0}", ex);
            //}
        }

        /// <summary>
        /// Saves the current configuration into stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            serializer.Serialize(stream, this);
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones current object
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cloning is important because a device task settings must not change when the task is started.
        /// The settings may only change while machine is stopped. So when machine is started we create another
        /// clone of settings and let the device tasks to use the cloned settings not the primary version. User
        /// changes while machine is started are applied to primary version not to clone. When machine is stopped
        /// then the clone is discarded.
        /// </para>
        /// <para>
        /// The cloning is done by saving the object into memory stream (by using XML serialization) and
        /// then loading a copy from the stream. Might not be the fastest way to clone but probably the simplest.
        /// </para>
        /// <para>
        /// Attention! can't use <seealso cref="MemberwiseClone"/> because some settings are reference types and they would stay shared.
        /// </para>
        /// </remarks>
        /// <returns>Clone of this object</returns>
        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Save(stream);
                stream.Position = 0;
                return Load(stream);
            }
        }

        #endregion
    }
}