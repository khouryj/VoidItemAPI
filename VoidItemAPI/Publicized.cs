﻿using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]