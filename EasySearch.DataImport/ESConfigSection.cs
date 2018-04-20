using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace EasySearch.DataImport
{
    public class ESConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Nodes")]
        public NodeCollection Nodes
        {
            get { return ((NodeCollection)(base["Nodes"])); }
            set { base["Nodes"] = value; }
        }

        [ConfigurationProperty("clusterName", IsRequired = true)]
        public string ClusterName
        {
            get
            {
                return this["clusterName"] as string;
            }
        }

        [ConfigurationProperty("indexName", IsRequired = true)]
        public string IndexName
        {
            get
            {
                return this["indexName"] as string;
            }
        }
    }

    public class NodeElement : ConfigurationElement
    {
        [ConfigurationProperty("uri", IsRequired = true, IsKey = true)]
        public string Uri
        {
            get { return (string)base["uri"]; }

        }
    }

    [ConfigurationCollection(typeof(NodeElement))]
    public class NodeCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "NodeElement";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }


        public override bool IsReadOnly()
        {
            return false;
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new NodeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NodeElement)(element)).Uri;
        }

        public NodeElement this[int idx]
        {
            get
            {
                return (NodeElement)BaseGet(idx);
            }
        }
    }

}


