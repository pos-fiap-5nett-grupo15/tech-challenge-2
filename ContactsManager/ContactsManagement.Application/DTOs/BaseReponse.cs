﻿using System.Text.Json.Serialization;

namespace ContactsManagement.Application.DTOs
{
    public class BaseReponse
    {
        [JsonIgnore]
        public int ErrorCode { get; set; }
        [JsonIgnore]
        public string ErrorDescription { get; set; }
    }
}
