using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_API.Additional;
using My_API.Connection;
using System.Net.Http.Headers;

namespace My_Portfolio.Services
{
	public class Services_Home
	{
		#region Connection Database
		private readonly ApplicationDBContext DBContext;
		static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
		//public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
		public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
		public static string RoleHakAkses = AppSettingJSON["Role:HakAkses"].ToString();
		//public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
		#endregion

		public Services_Home(ApplicationDBContext DBContext)
		{
			this.DBContext = DBContext;
		}

		public async Task<(bool GetStatus, string Message, ReturnLogin Result, int TotalData)> Login(Login ObjectPayload) 
		{
			try
			{
				#region Object Property And Get Connection
				var getStatus = new ReturnLogin();
				//var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
				//var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
				#endregion

				#region Check And Get Data, Then Insert Data Last Login
				var GetData = await DBContext.MasterEmployees.Select(obj => new
				{
					Nik = obj.Nik,
					UsernameLogin = obj.UsernameLogin,
					PasswordLogin = obj.PasswordLogin,
					HakAkses = obj.HakAkses,
					IsActive = obj.IsActive
				}).Where
				(
					x => x.UsernameLogin == ObjectPayload.Username && x.PasswordLogin == ObjectPayload.Password && x.HakAkses == RoleHakAkses && x.IsActive == true
				).Distinct().ToListAsync();

				foreach (var Obj in GetData)
				{
					if (Obj.IsActive == true)
					{
						#region Insert Data Last Login Using API
						var PayloadPOST = new Login
						{
							Username = Obj.UsernameLogin,
							Password = Obj.PasswordLogin,
							HakAkses = Obj.HakAkses
						};
						var BASE_API_URL = "https://localhost:7259/api/HistorySignIn/AddData";
						var client = new HttpClient();
						client.BaseAddress = new Uri(BASE_API_URL);
						client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						HttpResponseMessage response = new HttpResponseMessage();
						response = await client.PostAsJsonAsync(BASE_API_URL, PayloadPOST).ConfigureAwait(false);
						if (response.IsSuccessStatusCode)
						{
							string result = response.Content.ReadAsStringAsync().Result;
						}
						#endregion

						getStatus.getStatus = 1;
						getStatus.Username = ObjectPayload.Username;
						continue;
					}
				}

				return (true, "Successful Hit API : " + "api/HistorySignIn/AddData", getStatus, GetData.Count);
				#endregion
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
