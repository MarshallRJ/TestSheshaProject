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


namespace TestProject1
{

    public class TokenRequest
    {
        public string userNameOrEmailAddress { get; set; }
        public string password { get; set; }
    }
    
    public class TokenResponse
    {
        public TokenContainer result { get; set; }
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
            //ExecSQLCommand(_connectionString, $"Delet from ");
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
    public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
    {
        public SheshTestHelper GetSheshTestHelper()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();

            return new SheshTestHelper(configuration.GetConnectionString("Default"));
        }
        public static ProductTestServer CreateServer()
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


                //.Build();

            ProductTestServer testServer = new ProductTestServer(hostBuilder);

            //testServer.Host.MigrateDbContext<ProductContext>((_, __) => { });

            return testServer;
        }
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }


        private async Task<string> LoginWithToken(HttpClient client, string username, string password)
        {
            var response = await client.PostAsJsonAsync<TokenRequest>("/api/TokenAuth/Authenticate", new TokenRequest {userNameOrEmailAddress= username,password=password });

            if (response.IsSuccessStatusCode)
            {
                string responseStream = await response.Content.ReadAsStringAsync();
                TokenResponse res = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return res.result.accessToken;
            }
            throw new Exception("Login failed");
        }
        [Theory]
        [InlineData("/api/dynamic/Shesha/Person/Crud/GetAll")]
        //[InlineData("/Index")]
        //[InlineData("/About")]
        //[InlineData("/Privacy")]
        //[InlineData("/Contact")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var test = GetSheshTestHelper();
            test.CleanDatabase();

            var server = CreateServer();
            // Arrange
            //var client = _factory.CreateClient();

            var client = server.CreateClient();

            var token = await LoginWithToken(client, "admin", "123qwe");

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

}