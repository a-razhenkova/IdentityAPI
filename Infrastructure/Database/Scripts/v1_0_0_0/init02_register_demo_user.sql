DECLARE @userId INT = 1

BEGIN TRY
	BEGIN TRANSACTION T1

	INSERT INTO [dbo].[user]
	SELECT NULL AS [version],
		   '2a47a4fc-3d90-4ddb-a1ec-a664c0a8a2f3' AS [public_id],
		   'ivan.ivanov' AS [username],
		   0 /* Administrator */ AS [role],
		   'DNo+ZzGdBKpttZHUzuHxSQ==' AS [otp_key],
		   NULL AS [email],
		   0 AS [is_verified],
		   GETDATE() AS [registration_timestamp]

	INSERT INTO [dbo].[user_status]
	SELECT NULL AS [version],
		   @userId AS [user_id],
		   1 /* Active */ AS [status],
		   0 /* None */ AS [reason],
		   NULL AS [note]

	INSERT INTO [dbo].[user_password]
	SELECT NULL AS [version],
		   1 AS [user_id],
		   'ciU/l7HWErkLLPvD7B0L/aQYNNMo6L5Z/cOk8pU9tk/LXxAjtHTGWvIJYnbdhWw4meDOCvDbv6VW5GyTe6r15+/cw/GB8ZF00guBYTc1uZTKttNIQcDG7oNKzzjOE1nX1ScrCfNgw+f0mO5Fxi1j77Y9LegLaSZTDMfbT46SAUc=' AS [password], -- m4A0?Edis66a
		   'LudClePZUDxe5KOXLfYDBQ==' AS [secret],
		   GETDATE() AS [last_changed_timestamp]

	INSERT INTO [dbo].[login]
	SELECT NULL AS [version],
		   @userId AS [user_id],
		   0 AS [wrong_login_attempts_counter],
		   NULL AS [last_login_timestamp],
		   NULL AS [last_login_ip_address]

	COMMIT TRANSACTION T1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION T1;
	THROW;
END CATCH