using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;

namespace MeterChangeApi.Options
{
    public class DevelopmentOptions
    {
        public bool EnableSwagger { get; set; } = true;
        public string DevelopmentConnectionString { get; set; } = "Server=localhost;Database=DevDb;...";
        public SwaggerOptions Swagger { get; set; } = new SwaggerOptions();
    }

    public class SwaggerOptions
    {
        public bool EnableAuthorization { get; set; } = true;
        public string Title { get; set; } = "MeterChangeApi";
        public string Version { get; set; } = "v1";
    }    
}
