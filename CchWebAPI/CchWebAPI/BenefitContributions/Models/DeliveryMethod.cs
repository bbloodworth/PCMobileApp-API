using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models
{
    public class DeliveryMethod
    {
        public int DeliveryMethodKey { get; set; }
        public string DeliveryMethodCode { get; set; }
        public string DeliveryMethodName { get; set; }

        public class DeliveryMethodConfiguration: EntityTypeConfiguration<DeliveryMethod>
        {
            public DeliveryMethodConfiguration()
            {
                ToTable("DeliveryMethod_d");
                HasKey(k => k.DeliveryMethodKey);
            }
        }
    }
}