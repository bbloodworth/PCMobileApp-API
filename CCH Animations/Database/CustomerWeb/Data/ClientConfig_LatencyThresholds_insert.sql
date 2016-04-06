/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-04-06
-- Description:	Adds new client config key for Network Latency Threshold (for use in MP App)
-- =============================================
*/

  IF (NOT EXISTS(SELECT * FROM [dbo].[ClientConfig] WHERE ConfigKey = 'NetworkLatencyThresholdJson') )
	BEGIN 
	  INSERT INTO [dbo].[ClientConfig] (ConfigKey, Description, CreateDate)
	  VALUES ('NetworkLatencyThresholdJson', 
	          'This refers to the array of Threshold values (in milliseconds) that determines ' +
			  'whether a service request is latent or not.  Each element in the array represents a network protocol', 
	          GETDATE())
	END

  GO

  UPDATE [dbo].[ClientConfig] SET ConfigValue = '{ ethernet: 2000, wifi: 2000, 4g: 2000, 3g: 5000, 2g: 7000, unknown: 7000, none: 0 }' 
  WHERE ConfigKey = 'NetworkLatencyThresholdJson'

  GO
  
    
