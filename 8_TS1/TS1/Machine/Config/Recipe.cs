using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Xml.Serialization;

namespace Ipte.Machine.Configuration
{

    /// <summary>
    /// Generic settings object for recipe.
    /// Attention! because the recipes can be shared between machines, the object contains redundant fields.
    /// Let the fields remain even if they are not relevant in current machine; it is safe to ignore them.
    /// </summary>
    public class RecipeSettings
    {
        public RecipeSettings()
        {          
        }

        public int ModuleCount { get; set; } = 4;
    }

    /// <summary>
    /// A class that contains product specific manufacturing information
    /// </summary>
    public class Recipe : IDisposable
    {
        #region Static methods

        public static string[] EnumerateRecipes()
        {
            return Directory.GetFiles(Paths.ProductDirectory, "*.zprd");
        }

        public static string PathToShortName(string path)
        {
            if(string.IsNullOrEmpty(path) )
                return string.Empty;
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ShortNameToPath(string name)
        {
            if(string.IsNullOrEmpty(name) )
                return string.Empty;
            return Paths.ProductDirectory + "\\" + name + ".zprd";
        }

        public static string GetDescription(string path)
        {
            Package package = ZipPackage.Open(path, FileMode.Open, FileAccess.Read);
            try
            {
                return package.PackageProperties.Description;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                package.Close();
            }
        }

        #endregion

        #region private fields

        private static object _lock = new object();
        private Uri _drawingUri = new Uri("/drawing.xml", UriKind.Relative);
        private Uri _settingsUri = new Uri("/settings.xml", UriKind.Relative);

        #endregion

        #region constructors

        public static Recipe FromFile(string fileName, bool SkipDrawing = false)
        {
            lock (_lock)
            {
                Recipe result = new Recipe();
                result.Load(fileName, SkipDrawing);
                return result;
            }
        }

        public Recipe()
        {
        }

        ~Recipe()
        {
            Dispose();
        }

        public void Dispose()
        {
            //if (Drawing != null)
            //{
            //    Drawing.Dispose();
            //    Drawing = null;
            //}
            Settings = null;
        }

        #endregion

        #region public properties

        //public TeachSpace Drawing { get; set; }
        public RecipeSettings Settings { get; set; }
        public string Description { get; set; }
        public DateTime ModifiedDate { get; set; }

        #endregion

        #region public functions

        /// <summary>
        /// Load recipe from file
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName, bool SkipDrawing)
        {
            Package package = ZipPackage.Open(fileName, FileMode.Open, FileAccess.Read);

            //load metadata
            try
            {
                Description = package.PackageProperties.Description;
            }
            catch { }

            try
            {
                ModifiedDate = package.PackageProperties.Modified ?? DateTime.Now;
            }
            catch { }


            //Load settings
            try
            {
                Settings = null;
                if (package.PartExists(_settingsUri))
                {
                    PackagePart settingsPart = package.GetPart(_settingsUri);
                    XmlSerializer serializer = new XmlSerializer(typeof(RecipeSettings));
                    Settings = (RecipeSettings)serializer.Deserialize(settingsPart.GetStream());
                }
            }
            catch { }

            ////Load panel drawing if present. 
            //try
            //{
            //    Drawing = null;
            //    if (!SkipDrawing && package.PartExists(_drawingUri))
            //    {
            //        PackagePart drawingPart = package.GetPart(_drawingUri);
            //        Drawing = new TeachSpace();
            //        Drawing.Load(drawingPart.GetStream());
            //    }
            //}
            //catch { Drawing = null; }

            package.Close();
        }

        /// <summary>
        /// Save recipe to file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            Package package = ZipPackage.Open(fileName, FileMode.Create, FileAccess.ReadWrite);

            try
            {
                package.PackageProperties.Modified = DateTime.Now;
            }
            catch { }

            //save metadata
            try
            {
                package.PackageProperties.Description = Description;
            }
            catch { }

            try
            {
                PackagePart part;
                XmlSerializer serializer;

                //Save the settings
                if (package.PartExists(_settingsUri))
                    part = package.GetPart(_settingsUri);
                else
                    part = package.CreatePart(_settingsUri, System.Net.Mime.MediaTypeNames.Text.Xml);

                serializer = new XmlSerializer(typeof(RecipeSettings));
                serializer.Serialize(part.GetStream(), Settings);
                package.Flush();

                //Save the drawing
                if (package.PartExists(_drawingUri))
                    part = package.GetPart(_drawingUri);
                else
                    part = package.CreatePart(_drawingUri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

                //Drawing.Save(part.GetStream());
                package.Flush();
            }
            finally
            {
                package.Close();
            }

            ////commit changes to svn
            //try
            //{
            //    SvnCommitArgs args = new SvnCommitArgs();
            //    SvnCommitResult result;

            //    using (SvnClient client = new SvnClient())
            //    {
            //        try
            //        {
            //            Collection<SvnStatusEventArgs> statuses;

            //            client.GetStatus(fileName, out statuses);
            //            if (statuses[0].LocalContentStatus == SvnStatus.NotVersioned)
            //            {
            //                client.Add(fileName);
            //                args.LogMessage = "Adding new recipe to repository: " + fileName;
            //            }
            //        }
            //        catch { }

            //        args.LogMessage += "Saving changes to product file.\r\nUser: " + AccessManager.UserName;

            //        SvnUI.Bind(client, new SvnUIBindArgs());
            //        client.Commit(Paths.ProductDirectory, args, out result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Trace.TraceError("Failed to commit changes to svn repository: {0}", ex);
            //}
        }

        /// <summary>
        /// Deletes recipe from folder and removes it from SVN
        /// </summary>
        /// <param name="fileName"></param>
        public static void Delete(string fileName)
        {
            ////commit changes to svn
            //try
            //{
            //    SvnCommitArgs args = new SvnCommitArgs();
            //    SvnCommitResult result;

            //    using (SvnClient client = new SvnClient())
            //    {
            //        client.Delete(fileName);
            //        args.LogMessage += "Deleting the recipe.\r\nUser: " + AccessManager.UserName;
            //        SvnUI.Bind(client, new SvnUIBindArgs());
            //        client.Commit(Paths.ProductDirectory, args, out result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Trace.TraceError("Failed to commit changes to svn repository: {0}", ex);
            //}

            try
            {
                File.Delete(fileName);
            }
            catch { }
        }

        #endregion
    }

    /// <summary>
    /// A singelton class that represents recipe buffer, the class optimizes recipe loading.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In real world scenario most of the devices in machine need instrucitions about what to do 
    /// with product at hand. The information is stored in Recipe class, but to avoid loading duplicate
    /// instances of recipes a buffered version is used. Firs device that need's the recipe will load it
    /// and other devices get the same loaded module.
    /// </para>
    /// <para>
    /// When active recipe is modified while machine is running the modifications should not take effect
    /// immediately but only after the machine is restarted. To achive this the recipe buffer will be
    /// cleared by controller each time the machine is stopped (State=CamxState.Off).
    /// </para>
    /// </remarks>
    public static class RecipeBuffer
    {
        private static Dictionary<string, Recipe> _recipeBuffer = new Dictionary<string, Recipe>();

        /// <summary>
        /// If the recipe is already loaded then returns the instance from buffer otherwise loads from file
        /// </summary>
        /// <param name="fileName">The recipe container file.</param>
        /// <returns>Specified recipe</returns>
        public static Recipe GetRecipe(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            lock (_recipeBuffer)
            {
                if (!_recipeBuffer.ContainsKey(fileName))
                {
                    _recipeBuffer[fileName] = Recipe.FromFile(fileName);
                }

                return _recipeBuffer[fileName];
            }
        }

        /// <summary>
        /// Clear recipe buffer.
        /// </summary>
        public static void Clear()
        {
            lock (_recipeBuffer)
            {
                foreach (var recipe in _recipeBuffer.Values)
                {
                    recipe.Dispose();
                }
                _recipeBuffer.Clear();
            }
        }
    }
}
