using System;
using System.Collections;
using System.Data;

namespace CchWebAPI.Areas.Animation.Models
{
    [Serializable]
    public class Dependents : CollectionBase
    {
        public Dependents()
        {
        }


        public Dependent this[int index]
        {
            get { return (Dependent)List[index]; }
        }

        public void Add(Dependent dependent)
        {
            List.Add(dependent);
        }

        public Dependents GetAdultDependents()
        {
            Dependents adults = new Dependents();

            foreach (Dependent dep in List)
            {
                if (dep.Age >= 18)
                {
                    adults.Add(dep);
                }
            }
            return adults;
        }

        public DataTable AsDataTable()
        {
            DataTable dependentData = new DataTable("DependentData");

            dependentData.Columns.Add("FullName");
            dependentData.Columns.Add("FirstName");
            dependentData.Columns.Add("LastName");
            dependentData.Columns.Add("RelationShip");
            dependentData.Columns.Add("ShowQuestions");
            dependentData.Columns.Add("DepToUser");
            dependentData.Columns.Add("UserToDep");
            dependentData.Columns.Add("Adult");
            dependentData.Columns.Add("CCHID");

            DataRow dependentRow = null;
            foreach (Dependent dependent in List)
            {
                dependentRow = dependentData.NewRow();
                dependentRow["FullName"] = dependent.FullName;
                dependentRow["RelationShip"] = dependent.RelationshipText;
                dependentRow["Adult"] = dependent.IsAdult;
                dependentRow["CCHID"] = dependent.CCHID;
                dependentData.Rows.Add(dependentRow);
            }

            return dependentData;
        }
    }
}