﻿ALTER TABLE [dbo].[ClientAuthorization]
    ADD CONSTRAINT [PK_IssuedToken] PRIMARY KEY CLUSTERED ([AuthorizationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

