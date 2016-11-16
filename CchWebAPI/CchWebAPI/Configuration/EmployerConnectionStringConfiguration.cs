using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CchWebAPI.Configuration {
    public class EmployerConnectionStringConfiguration : ConfigurationSection {
        private static EmployerConnectionStringConfiguration connectionStrings
            = ConfigurationManager.GetSection("employerConnectionStrings") as EmployerConnectionStringConfiguration;

        public static EmployerConnectionStringConfiguration ConnectionStrings {
            get {
                return connectionStrings;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public EmployersCollection Employers {
            get {
                return (EmployersCollection)base[""];
            }
        }
    }
    public class EmployersCollection : ConfigurationElementCollection {
        public EmployersCollection() {
            EmployerConfigElement details = (EmployerConfigElement)CreateNewElement();
            if (details.Id != 0) {
                Add(details);
            }
        }

        public override ConfigurationElementCollectionType CollectionType {
            get {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new EmployerConfigElement();
        }

        protected override Object GetElementKey(ConfigurationElement element) {
            return ((EmployerConfigElement)element).Id;
        }

        public EmployerConfigElement this[int index] {
            get {
                return (EmployerConfigElement)BaseGet(index);
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public EmployerConfigElement this[string name] {
            get {
                return (EmployerConfigElement)BaseGet(name);
            }
        }

        public int IndexOf(EmployerConfigElement details) {
            return BaseIndexOf(details);
        }

        public void Add(EmployerConfigElement details) {
            BaseAdd(details);
        }

        protected override void BaseAdd(ConfigurationElement element) {
            BaseAdd(element, false);
        }

        public void Remove(EmployerConfigElement details) {
            if (BaseIndexOf(details) >= 0)
                BaseRemove(details.Id);
        }

        public void RemoveAt(int index) {
            BaseRemoveAt(index);
        }

        public void Remove(string name) {
            BaseRemove(name);
        }

        public void Clear() {
            BaseClear();
        }

        protected override string ElementName {
            get { return "employer"; }
        }
    }
    public class EmployerConfigElement : ConfigurationElement {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public int Id {
            get { return (int)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("dataWarehouse", IsRequired = true)]
        public string DataWarehouse {
            get { return (string)this["dataWarehouse"]; }
            set { this["dataWarehouse"] = value; }
        }
        [ConfigurationProperty("priceTransparency", IsRequired = false)]
        public string PriceTransparency {
            get { return (string)this["priceTransparency"]; }
            set { this["priceTransparency"] = value; }
        }
        [ConfigurationProperty("meltingPointMobile", IsRequired = false)]
        public string MeltingPointMobile {
            get { return (string)this["meltingPointMobile"]; }
            set { this["meltingPointMobile"] = value; }
        }
    }
}