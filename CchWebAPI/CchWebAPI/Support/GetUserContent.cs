using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CchWebAPI.Areas.Animation.Models;

namespace CchWebAPI.Support
{
    public class GetUserContent : DataBase
    {
        public int CchId
        {
            set { Parameters["CCHID"].Value = value; }
        }
        public int PagesSize
        {
            set { Parameters["PageSize"].Value = value; }
        }
        public int CampaignId
        {
            set { Parameters["BaseCampaignID"].Value = value; }
        }
        public int BaseContentId
        {
            set { Parameters["BaseContentID"].Value = value; }
        }
        public string LocaleCode
        {
            set { Parameters["LocaleCode"].Value = value; }
        }

        private List<UserContentRecord> _results = new List<UserContentRecord>();
        public List<UserContentRecord> Results
        {
            get { return _results; }
        }

        public int TotalCount
        {
            get { return Results.Count; } 
        }

        public GetUserContent()
            : base("p_GetUserContent")
        {
            Parameters.New("CCHID", SqlDbType.Int);
            Parameters.New("PageSize", SqlDbType.Int);
            Parameters.New("BaseCampaignID", SqlDbType.Int);
            Parameters.New("BaseContentID", SqlDbType.Int);
            Parameters.New("LocaleCode", SqlDbType.VarChar, Size: 10);
        }

        public override void GetData(string connectionString)
        {
            base.GetData(connectionString);

            if (Tables.Count > 0 && Tables[0].Rows.Count > 0)
            {
                _results = (from result in Tables[0].AsEnumerable()
                    select new UserContentRecord()
                    {
                        ContentCaption = result.GetData("ContentCaptionText"),
                        ContentDescription = result.GetData("ContentDesc"), 
                        ContentName = result.GetData("ContentName"),
                        Duration = result.GetData("Duration"),
                        ContentFileLocation = result.GetData("ContentFileLocationDesc"), 
                        ContentId = string.Format("{0}.{1}", result.GetData("CampaignID"), result.GetData("ContentID")), 
                        ContentImageFileName= result.GetData("ContentImageFileName"), 
                        Points = result.GetData("Points"), 
                        ContentSavingsAmt = result.GetData("ContentSavingsAmt"),
                        ContentStatus = result.GetData("ContentStatusDesc"), 
                        ContentTitle = result.GetData("ContentTitle"), 
                        ContentType = result.GetData("ContentTypeDesc"), 
                        MemberContentData = result.GetData("MemberContentDataText"), 
                        NumberOfQuestions = result.GetData("NumQuestions"),
                        ParentContentId = string.Format("{0}.{1}", result.GetData("CampaignID"), result.GetData("ParentContentID")),
                        ContentUrl = result.GetData("ContentUrl"),
                        ContentPhoneNum = result.GetData("ContentPhoneNum"),
                        CreateDate = string.Format("{0:M/dd/yyyy}", result.GetData<DateTime>("CreateDate"))
                    }).ToList<UserContentRecord>();
            }
        }
    }
}
