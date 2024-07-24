using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using test.TestProject.Web.Host.Startup;
using Xunit.Sdk;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using NHibernate.Mapping;

using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using DocumentFormat.OpenXml.EMMA;
using Castle.Core.Internal;
using Xunit;
using System.Data.Common;
using Microsoft.SqlServer.Management.Smo.Mail;


namespace TestProject1
{

    public class TokenRequest
    {
        public string userNameOrEmailAddress { get; set; }
        public string password { get; set; }
    }

    public class VehicleAdd
    {
        public string registrationNumber { get; set; }
        public string make { get; set; }

        public string model { get; set; }

        public int year { get; set; }
    }

    public class TokenResponse
    {
        public TokenContainer result { get; set; }
    }

    public class EntityAddResponse
    {
        public EntityAddContainer result { get; set; }
    }

    public class EntityAddContainer
    {
        public Guid Id { get; set; }
    }

    public class TokenContainer
    {
        public string accessToken { get; set; }
    }
    public class ProductTestServer : TestServer
    {
        public ProductTestServer(IWebHostBuilder builder) : base(builder)
        {
            //ProductContext = Host.Services.GetRequiredService<ProductContext>();


        }

        //public ProductContext ProductContext { get; set; }
    }

    public class SheshTestHelper
    {
        protected string _connectionString;
        protected string _masterString;
        protected string _dbName;
        public SheshTestHelper(string connectionString)
        {
            _connectionString = connectionString;

            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);


            _dbName = builder.InitialCatalog;

            builder.InitialCatalog = "master";

            _masterString = builder.ConnectionString;
        }


        private static bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            SqlConnection tmpConn;
            string sqlCreateDBQuery;
            bool result = false;

            try
            {
                tmpConn = new SqlConnection(connectionString);

                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);

                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();

                        object resultObj = sqlCmd.ExecuteScalar();

                        int databaseID = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }

                        tmpConn.Close();

                        result = (databaseID > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public void CreateAndPopulateDatabase()
        {
            if (CheckDatabaseExists(_masterString, _dbName))
                DropDatabase();
            CreateDatabase();
            PopulateDatabase();
        }

        private void CreateDatabase()
        {
            ExecSQLCommand(_masterString, $"CREATE DATABASE {_dbName}");
        }

        private void DropDatabase()
        {
            ExecSQLCommand(_masterString, $"ALTER DATABASE [{_dbName}] SET SINGLE_USER  WITH ROLLBACK IMMEDIATE");
            ExecSQLCommand(_masterString, $"DROP DATABASE {_dbName}");
        }

        public void CleanDatabase()
        {
            ExecSQLCommand(_connectionString, $"Delete from TP_Vehicles");
        }

        private void PopulateDatabase()
        {
            return;
            using (Microsoft.Data.SqlClient.SqlConnection conn = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                Microsoft.SqlServer.Management.Smo.Server db = new Microsoft.SqlServer.Management.Smo.Server(new Microsoft.SqlServer.Management.Common.ServerConnection(conn));
                string script = File.ReadAllText(Directory.GetCurrentDirectory() + "\\CreatDatabse.sql");
                db.ConnectionContext.ExecuteNonQuery(script);
            }
            return;
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\CreatDatabse.sql");


            string contents = reader.ReadToEnd();
            reader.Close();

            var splitCommands = SplitSqlStatements(contents);
            SqlConnection myConn = new SqlConnection(_connectionString);
            myConn.Open();
            foreach (var sqlCommand in splitCommands)
            {
                SqlCommand myCommand = new SqlCommand(sqlCommand, myConn);
                myCommand.CommandTimeout = 30;

                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception kk)
                {
                    throw kk;
                }



            }

            if (myConn.State == ConnectionState.Open)
            {
                myConn.Close();
            }

        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Make line endings standard to match RegexOptions.Multiline
            sqlScript = Regex.Replace(sqlScript, @"(\r\n|\n\r|\n|\r)", "\n");

            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^[\t ]*GO[\t ]*\d*[\t ]*(?:--.*)?$",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\n'));
        }

        private void ExecSQLCommand(SqlConnection myConn, string sqlCommand)
        {
            SqlCommand myCommand = new SqlCommand(sqlCommand, myConn);

            myConn.Open();
            myCommand.ExecuteNonQuery();


        }

        private void ExecSQLCommand(string connectionString, string sqlCommand)
        {

            SqlConnection myConn = new SqlConnection(connectionString);




            SqlCommand myCommand = new SqlCommand(sqlCommand, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
                //do nothing
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }
    }

    public class CoreTestHelper
    {
        private ProductTestServer _server;
        public ProductTestServer CreateServer()
        {
            if (_server!= null)
                return _server;
            var configurationValues = new Dictionary<string, string>
        {
            { "MyConfigSetting", "Value" }
        };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            //var hostBuilder = WebHost.CreateDefaultBuilder()
            var hostBuilder = WebHost.CreateDefaultBuilder<Startup>(new string[0])
                .UseConfiguration(configuration)
                .CaptureStartupErrors(true)


                 .UseSetting("detailedErrors", "true")
                  .ConfigureAppConfiguration((context, configurationBuilder) =>
                  {

                      //configurationBuilder
                      //    //.SetBasePath(Directory.GetCurrentDirectory())
                      //    //.AddJsonFile("appsettings.Test.json", optional: false)
                      //    //.AddEnvironmentVariables()
                      //    .AddConfiguration(configuration);

                      configurationBuilder.Sources.Add(new JsonConfigurationSource
                      {
                          Path = "appsettings.json",
                          Optional = false,
                          ReloadOnChange = true,
                          FileProvider = new PhysicalFileProvider(Environment.CurrentDirectory)
                      });




                  })
                    .UseEnvironment("Testing")
                  .UseStartup<Startup>();


            //.Build();

            ProductTestServer testServer = new ProductTestServer(hostBuilder);

            //testServer.Host.MigrateDbContext<ProductContext>((_, __) => { });
            _server = testServer;

            return testServer;
        }

        public async Task<string> LoginWithToken(HttpClient client, string username, string password)
        {
            var response = await client.PostAsJsonAsync<TokenRequest>("/api/TokenAuth/Authenticate", new TokenRequest { userNameOrEmailAddress = username, password = password });

            if (response.IsSuccessStatusCode)
            {
                string responseStream = await response.Content.ReadAsStringAsync();
                TokenResponse res = await response.Content.ReadFromJsonAsync<TokenResponse>();
                //set the header as the logged in token
                
                return res.result.accessToken;
            }
            var errorStream = await response.Content.ReadAsStringAsync();

            throw new Exception($"Login failed: {errorStream}");
        }
    }

    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var configurationValues = new Dictionary<string, string>
        {
            { "MyConfigSetting", "Value" }
        };

            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(configurationValues)
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

            //var hostBuilder = WebHost.CreateDefaultBuilder()
            var hostBuilder = WebHost.CreateDefaultBuilder<Startup>(new string[0])
                .UseConfiguration(configuration)
                .CaptureStartupErrors(true)


                 .UseSetting("detailedErrors", "true")
                  .ConfigureAppConfiguration((context, configurationBuilder) =>
                  {

                      //configurationBuilder
                      //    //.SetBasePath(Directory.GetCurrentDirectory())
                      //    //.AddJsonFile("appsettings.Test.json", optional: false)
                      //    //.AddEnvironmentVariables()
                      //    .AddConfiguration(configuration);

                      configurationBuilder.Sources.Add(new JsonConfigurationSource
                      {
                          Path = "appsettings.json",
                          Optional = false,
                          ReloadOnChange = true,
                          FileProvider = new PhysicalFileProvider(Environment.CurrentDirectory)
                      });




                  })
                    .UseEnvironment("Testing")
                  .UseStartup<Startup>();

            return hostBuilder;
        }
       
    }

    public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected string _adminUser = "admin";
        protected string _adminPass = "123qwe";
        protected static CoreTestHelper _coreTestHelper;

        protected readonly CustomWebApplicationFactory<Startup> _factory;

        public IntegrationTestBase(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        public async Task<Guid> AddVehicle(HttpClient client, string reg, string make, string model, int year)
        {
            var response = await client.PostAsJsonAsync<VehicleAdd>("/api/dynamic/test.TestProject/Vehicle/Crud/Create", new VehicleAdd { registrationNumber = reg, make = make, model = model, year = year });

            if (response.IsSuccessStatusCode)
            {
                string responseStream = await response.Content.ReadAsStringAsync();
                EntityAddResponse res = await response.Content.ReadFromJsonAsync<EntityAddResponse>();
                //set the header as the logged in token

                return res.result.Id;
            }
            var errorStream = await response.Content.ReadAsStringAsync();

            throw new Exception($"Create Vehicle failed: {errorStream}");
        }
        public SheshTestHelper GetSheshTestHelper()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();

            return new SheshTestHelper(configuration.GetConnectionString("Default"));
        }

        public CoreTestHelper GetCoreTestHelper()
        {
            if (_coreTestHelper == null)
                _coreTestHelper = new CoreTestHelper();
            return _coreTestHelper;
        }

        public virtual void CleanAll()
        {
            GetSheshTestHelper().CleanDatabase();
        }
    }
    public class BasicTests : IntegrationTestBase

    {
        public BasicTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/dynamic/Shesha/Person/Crud/GetAll")]
        //[InlineData("/Index")]
        //[InlineData("/About")]
        //[InlineData("/Privacy")]
        //[InlineData("/Contact")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            this.CleanAll();
            var coreHelper = GetCoreTestHelper();

            //var server = coreHelper.CreateServer();
            //// Arrange
            ////var client = _factory.CreateClient();

            //var client = server.CreateClient();

            var client = _factory.CreateClient();

            var token = await coreHelper.LoginWithToken(client, "admin", "123qwe");

            client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    public class VehicleTests : IntegrationTestBase
    {
        public VehicleTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task AddVehicleHappyLine()
        {
            this.CleanAll();
            var coreHelper = GetCoreTestHelper();
            var server = coreHelper.CreateServer();

            var client = server.CreateClient();

           var token = await coreHelper.LoginWithToken(client, _adminUser, _adminPass);
            

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //add a test vechile
            var id = await AddVehicle(client, "reg123", "BMW", "320i", 2016);

            client.Dispose();

            Assert.NotNull(id);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task AddedVehicles_SameRegistration_ShouldFail()
        {
            this.CleanAll();
            var coreHelper = GetCoreTestHelper();
            var server = coreHelper.CreateServer();

            var client = server.CreateClient();

            var token = await coreHelper.LoginWithToken(client, _adminUser, _adminPass);
           

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //add a test vechile
            var id = await AddVehicle(client, "reg123", "BMW", "320i", 2016);
            //add another vechile with samne
            bool passed = false;
            try
            {
                var id2 = await AddVehicle(client, "reg123", "Honda", "civic", 2016);
                passed = true;
            }
            catch (Exception kk)
            {
                var expectedMessage = "SQL not available";
                Assert.Contains(expectedMessage, kk.Message);
            }

            Assert.False(passed);
        }

        [Theory]
        //not allow nulls
        [InlineData(null, "BMW", "320i", 2016, "'Registration Number' must not be empty")]
        [InlineData("123", null, "320i", 2016, "'Make' must not be empty")]
        [InlineData("123", "BMW", null, 2016, "'Model' must not be empty")]
        //not allow empty
        [InlineData("", "BMW", "320i", 2016, "'Registration Number' must be between 3")]
        [InlineData("123", "", "320i", 2016, "'Make' must be between 2")]
        [InlineData("123", "BMW", "", 2016, "'Model' must be between 2")]
        //year must be between 1900 and 2100
        [InlineData("123", "BMW", "320i", 1899, "'Year' must be between 1900 and 2100")]
        //year must be between 1900 and 2100
        [InlineData("123", "BMW", "320i", 2101, "'Year' must be between 1900 and 2100")]
        public async Task AddVehicle_DataInvalid_ShouldFail(string reg,string make, string model,int year, string expectedError)
        {
            this.CleanAll();
            var coreHelper = GetCoreTestHelper();
            var server = coreHelper.CreateServer();

            var client = server.CreateClient();

            var token = await coreHelper.LoginWithToken(client, _adminUser, _adminPass);


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

           
    
            bool passed = false;
            try
            {
                var id = await AddVehicle(client, reg, make, model, year);
                passed = true;
            }
            catch (Exception kk)
            {
               
                Assert.Contains(expectedError, kk.Message);
            }

            Assert.False(passed);
        }
    }

    public class ReservationTests : IntegrationTestBase
    {
        public ReservationTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        public async Task<Guid> ReserveVehicle(HttpClient client, Guid vehichileId, DateTime from, DateTime to)
        {
            //var response = await client.PostAsJsonAsync<VehicleAdd>("/api/dynamic/test.TestProject/Vehicle/Crud/Create", new VehicleAdd { registrationNumber = reg, make = make, model = model, year = year });

            //if (response.IsSuccessStatusCode)
            //{
            //    string responseStream = await response.Content.ReadAsStringAsync();
            //    EntityAddResponse res = await response.Content.ReadFromJsonAsync<EntityAddResponse>();
            //    //set the header as the logged in token

            //    return res.result.Id;
            //}
            //var errorStream = await response.Content.ReadAsStringAsync();

            //throw new Exception($"Create Vehicle failed: {errorStream}");
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ReserveVehicleHappyLine()
        {
            this.CleanAll();
            var coreHelper = GetCoreTestHelper();
            var server = coreHelper.CreateServer();

            var client = server.CreateClient();

            var token = await coreHelper.LoginWithToken(client, _adminUser, _adminPass);


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //add a test vechile
            var id = await AddVehicle(client, "reg123", "BMW", "320i", 2016);

            var resrvationId = await ReserveVehicle(client, id, new DateTime(2001, 12, 1), new DateTime(2001, 12, 2));

            client.Dispose();

            Assert.NotNull(resrvationId);
            Assert.NotEqual(Guid.Empty, id);
        }
    }
 }