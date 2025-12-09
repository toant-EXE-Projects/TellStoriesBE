using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Tests
{
    public class ProgramTests
    {
        private readonly HttpClient _httpClient;
        private readonly WebApplicationFactory<Program> _application;
        public ProgramTests()
        {
            _application = new WebApplicationFactory<Program>();
           
            _httpClient = _application.CreateClient();
            
        }
    }
}
