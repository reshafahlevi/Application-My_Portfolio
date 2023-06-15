using Microsoft.AspNetCore.Mvc;
using My_API.Additional;
using My_API.Connection;
using System.Diagnostics;

namespace My_Portfolio.Controllers
{
    public class MasterTransaksiController : Controller
    {
        #region Property DataTables
        [BindProperty]
        public DataTables.DataTablesRequest? DataTablesRequest { get; set; }
        #endregion

        #region Connection Database
        private readonly ApplicationDBContext DBContext;
        private readonly ILogger<MasterTransaksiController> _logger;
        static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
        //public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
        #endregion

        public MasterTransaksiController(ILogger<MasterTransaksiController> logger, ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MasterTransaksi()
        {
            return View();
        }
    }
}
