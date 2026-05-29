BEGIN TRY
	BEGIN TRANSACTION T1

	DECLARE @clientKey VARCHAR(36) = 'dba1d25a-0062-49e7-b4f0-31224a69f9e4'
	DECLARE @clientSecret VARCHAR(36) = '818fec5e-bff4-4396-85a7-9cc2eccd166f'

	INSERT INTO [dbo].[client]
	SELECT NULL AS [version],
		   'Identity API' AS [name],
		   @clientKey AS [key],
		   @clientSecret AS [secret],
		   0 AS [wrong_login_attempts_counter],
		   1 AS [is_internal]

	DECLARE @clientId INT = (SELECT id FROM [dbo].[client] WITH(NOLOCK) WHERE [KEY] = @clientKey)

	INSERT INTO [dbo].[client_status]
	SELECT NULL AS [version],
		   @clientId AS [client_id],
		   1 /* Active */ AS [status],
		   0 /* None */ AS [reason],
		   NULL AS [note]

	INSERT INTO [dbo].[client_right]
	SELECT NULL AS [version],
		   @clientId AS [client_id],
		   1 AS [can_notify]

	COMMIT TRANSACTION T1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION T1;
	THROW;
END CATCH

BEGIN TRY
	BEGIN TRANSACTION T1

	SET @clientKey = '806279f4-0b27-4398-9de7-9442cac986eb'
	SET @clientSecret = '78f67e88-a792-4848-a5f1-11a57616dd99'

	INSERT INTO [dbo].[client]
	SELECT NULL AS [version],
		   'Health UI' AS [name],
		   @clientKey AS [key],
		   @clientSecret AS [secret],
		   0 AS [wrong_login_attempts_counter],
		   1 AS [is_internal]

	SET @clientId = (SELECT id FROM [dbo].[client] WITH(NOLOCK) WHERE [KEY] = @clientKey)

	INSERT INTO [dbo].[client_status]
	SELECT NULL AS [version],
		   @clientId AS [client_id],
		   1 /* Active */ AS [status],
		   0 /* None */ AS [reason],
		   NULL AS [note]

	INSERT INTO [dbo].[client_right]
	SELECT NULL AS [version],
		   @clientId AS [client_id],
		   0 AS [can_notify_party]

	COMMIT TRANSACTION T1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION T1;
	THROW;
END CATCH