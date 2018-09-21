using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XrmCommandBox
{
    public class CommandOptionsSerializer
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(CommandOptionsSerializer));

		private void DeserializeOptions(object options, XmlNode parentNode)
		{
			if (parentNode != null && options != null)
			{
				foreach (XmlNode configNode in parentNode.ChildNodes)
				{
					var configOptionName = configNode.Name;
					var optionProperty = options.GetType().GetProperty(configOptionName);
					if (optionProperty != null)
					{
						if (optionProperty.PropertyType == typeof(string))
						{
							optionProperty.SetValue(options, configNode.InnerXml);
						}
						else if (optionProperty.PropertyType.IsGenericType && optionProperty.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
						{
							var enumerableType = optionProperty.PropertyType.GetGenericArguments()?[0];
							if (enumerableType == typeof(string))
							{
								var values = new List<string>();
								// find child nodes
								foreach (XmlNode valueNode in configNode.ChildNodes)
								{
									values.Add(valueNode.InnerXml);
								}
								optionProperty.SetValue(options, values);
							}
							else if (typeof(CommonOptions).IsAssignableFrom(enumerableType))
							{
								// Create a generyc list
								//var values = new ArrayList();
								var values = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { enumerableType }));
								foreach (XmlNode valueNode in configNode.ChildNodes)
								{
									object itemValue = enumerableType.GetConstructor(new Type[] { })?.Invoke(new object[] { });
									if(itemValue != null)
									{
										DeserializeOptions(itemValue, valueNode);
										//values.Add(itemValue);

										values.GetType().GetMethod("Add")?.Invoke(values, new object[] { itemValue });
									}
								}
								optionProperty.SetValue(options, values);
							}
						}
					}
				}
			}
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
					DeserializeOptions(options, xml.DocumentElement);
				}
            }
        }
    }
}