// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace OnceMi.IdentityServer4.Models
{
    public class ConsentOptions
    {
        public static bool EnableOfflineAccess = true;
        public static string OfflineAccessDisplayName = "脱机访问";
        public static string OfflineAccessDescription = "访问应用程序和资源，即使您处于脱机状态。";

        public static readonly string MustChooseOneErrorMessage = "至少需要选择一项";
        public static readonly string InvalidSelectionErrorMessage = "无效的选择";
    }
}
