using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContribution.Models
{
    public class ContributionType
    {
        public int ContributionTypeKey { get; set; }
        public string ContributionTypeCode { get; set; }
        public string ContributionTypeName { get; set; }

        public class ConfigurationTypeConfiguration: EntityTypeConfiguration<ContributionType>
        {
            public ConfigurationTypeConfiguration()
            {
                ToTable("ContributionType_d");
                HasKey(k => k.ContributionTypeKey);
            }
        }
    }
}