using System;
using log4net;
using System.Xml;

namespace XrmCommandBox
{
    public class CommandOptionsSerializer
    {
        private ILog _log = LogManager.GetLogger(typeof(Program));

        public CommandOptionsSerializer()
        {
            
        }

        public CommandOptionsSerializer(ILog log)
        {
            this._log = log;
        }

        public void Deserialize(CommonOptions options)
        {
            if (!string.IsNullOrEmpty(options?.ConfigFile))
            {
                _log.Debug("Loading options from file...");
                var xml = new XmlDocument();
                xml.Load(options.ConfigFile);

                if (xml.DocumentElement != null)
                {
                    foreach (XmlNode configNode in xml.DocumentElement.ChildNodes)
                    {
                        var configOptionName = configNode.Name;
                        var optionProperty = options.GetType().GetProperty(configOptionName);
                        if (optionProperty != null)
                        {
                            if (optionProperty.PropertyType == typeof(string))
                            {
                                optionProperty.SetValue(options,configNode.InnerXml);
                            }
                        }
                    }                    
                }
            }
        }
    }
}