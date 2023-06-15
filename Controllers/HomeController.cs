using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using My_API.Connection;
using My_API.Additional;
using My_API.Models;
using System.Text;

namespace My_Portfolio.Controllers
{
	public class HomeController : Controller
    {
        #region Connection Database
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext DBContext;
        static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
        public static string RoleHakAkses = AppSettingJSON["Role:HakAkses"].ToString();
        //public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
        #endregion

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

		[HttpPost, Produces("application/json")]
		//public async Task<JsonResult> Login([FromBody] string Username, string Password, string HakAkses)
		public async Task<JsonResult> Login([FromBody] Login ObjectPayload)
        {
            try
            {
				#region Object Property And Connection
				//var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
				//var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
				var Data = new List<MasterEmployee>();
				var getStatus = new ReturnLogin();
				var PayloadPOST = new Login
				{
					Username = ObjectPayload.Username,
					Password = ObjectPayload.Password,
					HakAkses = RoleHakAkses
				};
				#endregion

				#region Method 1 Find Data Employee Using API : api/MasterEmployee/FindEmployeeForSignIn
				var API_FindEmployeeForSignIn = "https://localhost:7259/api/MasterEmployee/FindEmployeeForSignIn";
				var HttpClient_API_FindEmployee = new HttpClient();
				HttpClient_API_FindEmployee.BaseAddress = new Uri(API_FindEmployeeForSignIn);
				HttpClient_API_FindEmployee.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var Response_ReadAPI_FindEmployee = new HttpResponseMessage();
				Response_ReadAPI_FindEmployee = await HttpClient_API_FindEmployee.PostAsJsonAsync(API_FindEmployeeForSignIn, PayloadPOST);
				if (Response_ReadAPI_FindEmployee.IsSuccessStatusCode)
				{
					var Result_ReadAPI = Response_ReadAPI_FindEmployee.Content.ReadAsStringAsync().Result;
					Data = JsonConvert.DeserializeObject<List<MasterEmployee>>(Result_ReadAPI);
				}
				else
				{
					getStatus.getStatus = 0;
					getStatus.Username = string.Empty;
					RedirectToAction("Index");
				}
				#endregion

				#region Method 2 Find Data Employee Using API : api/MasterEmployee/FindEmployeeForSignIn
				//var API_FindEmployeeForSignIn = "https://localhost:7259/api/MasterEmployee/FindEmployeeForSignIn";
				//var HttpClient_API_FindEmployeeForSignIn = new HttpClient();
				//var Request = new HttpRequestMessage(HttpMethod.Post, API_FindEmployeeForSignIn);
				//Request.Headers.Accept.Clear();
				//Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				//var Result_SerializeObject = JsonConvert.SerializeObject(PayloadPOST, new JsonSerializerSettings { Formatting = Formatting.None });
				//Request.Content = new StringContent(Result_SerializeObject, Encoding.UTF8, "application/json");
				//var Response = await HttpClient_API_FindEmployeeForSignIn.SendAsync(Request, CancellationToken.None);
				//var ReadAPI = await Response.Content.ReadAsStringAsync();
				//var Result_DeserializeObject = JsonConvert.DeserializeObject<List<MasterEmployee>>(ReadAPI);
				//Response.EnsureSuccessStatusCode();
				//if (Response.IsSuccessStatusCode)
				//{
				//	var Result_ReadAPI = Response.Content.ReadAsStringAsync().Result;
				//	Data = JsonConvert.DeserializeObject<List<MasterEmployee>>(Result_ReadAPI);
				//}
				//else
				//{
				//	getStatus.getStatus = 0;
				//	getStatus.Username = string.Empty;
				//	RedirectToAction("Index");
				//}
				#endregion

				#region Check Data Employee And Insert Data Using API : api/HistorySignIn/AddData
				if (Data.Count() == 0 || Data.Count() < 1)
				{
					getStatus.getStatus = 0;
					getStatus.Username = string.Empty;
					RedirectToAction("Index");
				}

				foreach (var Obj in Data)  
				{
					if (Obj.IsActive == true)
					{
						var Base_API_URL_AddData = "https://localhost:7259/api/HistorySignIn/AddData";
						var HttpClient_API_AddHistorySignIn = new HttpClient();
						HttpClient_API_AddHistorySignIn.BaseAddress = new Uri(Base_API_URL_AddData);
						HttpClient_API_AddHistorySignIn.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						var Response_ReadAPI_AddHistorySignIn = new HttpResponseMessage();
						Response_ReadAPI_AddHistorySignIn = await HttpClient_API_AddHistorySignIn.PostAsJsonAsync(Base_API_URL_AddData, PayloadPOST).ConfigureAwait(false);
						if (Response_ReadAPI_AddHistorySignIn.IsSuccessStatusCode)
						{
							var Result_ReadAPI = Response_ReadAPI_AddHistorySignIn.Content.ReadAsStringAsync().Result;
							getStatus.getStatus = 1;
							getStatus.Username = Obj.UsernameLogin; //ObjectPayload.Username;
							TempData["Username"] = Obj.UsernameLogin;
                            continue;
						}
						else
						{
							getStatus.getStatus = 0;
							getStatus.Username = string.Empty;
							RedirectToAction("Index");
							break; 
						}
					}
					else if (Obj.UsernameLogin == null || Obj.UsernameLogin == "" || Obj.UsernameLogin == string.Empty &&
							 Obj.PasswordLogin == null || Obj.PasswordLogin == "" || Obj.PasswordLogin == string.Empty &&
							 Obj.HakAkses == null || Obj.HakAkses == "" || Obj.HakAkses == string.Empty || Obj.HakAkses.Contains(string.Empty) && 
							 Obj.IsActive == false)
					{
						getStatus.getStatus = 0;
						getStatus.Username = string.Empty;
						RedirectToAction("Index");
						break;
					}
					else
					{
						getStatus.getStatus = 0;
						getStatus.Username = string.Empty;
						RedirectToAction("Index");
						break;
					}
				}
				#endregion

				return new JsonResult(getStatus);
			}
			catch (NullReferenceException ErrorNullReference)
			{
				Console.WriteLine(ErrorNullReference.Message);
				throw;
			}
			catch (Exception ErrorException)
			{
				Console.WriteLine(ErrorException.Message);
				throw new ApplicationException(ErrorException.Message);
			}

			finally
			{
			}
		}
    }
}