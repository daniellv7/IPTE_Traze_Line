using System;
using System.IO;
using System.Windows.Xps.Packaging;
using Ipte.Machine.Configuration;

namespace Ipte.UI.Dialogs
{
    public partial class DocumentViewerDialog
    {
        public DocumentViewerDialog()
        {
            InitializeComponent();

            string docPath = Path.Combine(Paths.RootDirectory, "documentation.xps");

            //Copy the default documentation to application data directory if it's not there
            if (!File.Exists(docPath))
            {
                string defaultFile = AppDomain.CurrentDomain.BaseDirectory + "Resources\\documentation.xps";
                if (File.Exists(defaultFile)) File.Copy(defaultFile, docPath, false);
            }

            if (File.Exists(docPath))
            {
                XpsDocument document = new XpsDocument(docPath, FileAccess.Read);
                try
                {
                    viewer.Document = document.GetFixedDocumentSequence();
                    viewer.FitToWidth();
                }
                finally
                {
                    document.Close();
                }
            }
        }
    }
}
