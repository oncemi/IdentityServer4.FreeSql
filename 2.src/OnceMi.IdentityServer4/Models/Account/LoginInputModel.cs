// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnceMi.IdentityServer4.Models
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Description("用户名")]
        public string Username { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [Description("密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "验证码不能为空")]
        [Description("验证码")]
        public string Code { get; set; }

        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; set; }
    }
}