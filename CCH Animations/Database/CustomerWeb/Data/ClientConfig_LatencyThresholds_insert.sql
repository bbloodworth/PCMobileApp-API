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

  UPDATE [dbo].[ClientConfig] SET ConfigValue = '{"ethernet": 5000, "wifi": 5000, "4g": 5000, "3g": 10000, "2g": 15000, "unknown": 15000, "none": 0 }'
  WHERE ConfigKey = 'NetworkLatencyThresholdJson'

  GO
  
    
