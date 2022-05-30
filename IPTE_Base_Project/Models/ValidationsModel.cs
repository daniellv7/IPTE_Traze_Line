using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IPTE_Base_Project.Models
{
   public class ValidationsModel : BaseModel
    {
        private SettingsModel settings;
        public ValidationsList validationList;

        //public ValidationsModel(SettingsModel settingsModel)
        //{
        //    settings = settingsModel;

        //    //Load XML and serializer
        //    XmlSerializer serializer = new XmlSerializer(typeof(ValidationsList));
        //    using (FileStream fileStream = new FileStream(settings.DATA_PATH + settings.validationsFilePath, FileMode.Open))
        //    {
        //        validationList = (ValidationsList)serializer.Deserialize(fileStream);
        //    }
        //}
    }

    [XmlRoot("Validations")]
    public class ValidationsList
    {
        [XmlElement("Validation")]
        public List<Validation> Validations { get; set; }
    }

    public class Validation
    {
        [XmlElement("ClassName")]
        public string ClassName { get; set; }

        [XmlElement("PropertyName")]
        public string PropertyName { get; set; }

        [XmlElement("PropertyType")]
        public string PropertyType { get; set; }

        [XmlElement("Min")]
        public double Min { get; set; }

        [XmlElement("Max")]
        public double Max { get; set; }

        [XmlElement("Regex")]
        public string Regex { get; set; }

        [XmlElement("ErrorMessage")]
        public string ErrorMessage { get; set; }
    }
}
