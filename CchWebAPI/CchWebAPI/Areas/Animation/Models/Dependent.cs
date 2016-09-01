using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Areas.Animation.Models
{
    /// <summary>
    /// A person who is a dependent of an employee
    /// </summary>
    [Serializable]
    public class Dependent
    {
        public Dependent()
        {
        }

        private int cchID = -1;
        private string fullName = string.Empty;
        private int age = -1;
        private string email = string.Empty;
        private Boolean isAdult = false;
        private string relationshipText = string.Empty;
        private string subscriberMedicalId = string.Empty;

        public int CCHID
        {
            get { return cchID; }
            set { cchID = value; }
        }

        public string SubscriberMedicalId
        {
            get { return subscriberMedicalId; }
            set { subscriberMedicalId = value; }
        }
        public string FullName
        {
            get
            {
                return fullName;
            }
            set { fullName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
        public Boolean IsAdult { get { return isAdult; } set { isAdult = Boolean.Parse(value.ToString()); } }
        public string RelationshipText { get { return relationshipText; } set { relationshipText = value; } }
    }
}