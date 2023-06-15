using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Diagnostics;
using My_API.Additional;
using My_API.Connection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace My_Portfolio.Controllers
{
	public class DashboardController : Controller
    {
        #region Property DataTables
        [BindProperty]
        public DataTables.DataTablesRequest? DataTablesRequest { get; set; }
        #endregion

        #region Connection Database
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDBContext DBContext;
        static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
        //public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
        #endregion

        public DashboardController(ILogger<DashboardController> logger, ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static HomeController home;

        public IActionResult Dashboard()
        {
            //TempData["Username"] = HttpContext.Session.Id;
            return View();
        }

        public IActionResult MenuPesanan()
        {
            return new RedirectResult(url: "/MenuPesanan/MenuPesanan", permanent: true, preserveMethod: true);
            //Response.Redirect("/MenuPesanan/MenuPesanan");
            //return View();
        }

        public IActionResult MenuTransaksi()
        {
            return new RedirectResult(url: "/MenuTransaksi/MenuTransaksi", permanent: true, preserveMethod: true);
            //Response.Redirect("/MenuTransaksi/MenuTransaksi");
            //return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetDataById(long id)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var Data = new List<dynamic>();
                var MasterMenusQuery = await DBContext.MasterMenus.AsSingleQuery().AsNoTracking().Where(x => x.Id == id).ToListAsync();
                foreach (var Obj in MasterMenusQuery)
                {
                    dynamic HargaSatuanRounded = string.Format("{0:#,#.##}", Obj.HargaSatuan);
                    dynamic Temp = new
                    {
                        Id = Obj.Id,
                        KodeMenu = Obj.KodeMenu,
                        NamaMenu = Obj.NamaMenu,
                        JenisMenu = Obj.JenisMenu,
                        HargaSatuan = "Rp. " + HargaSatuanRounded
                    };
                    Data.Add(Temp);
                }
                return await Task.FromResult(Json(Data));
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchCustome(string Search)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var MasterMenusQuery = DBContext.MasterMenus.AsNoTracking().AsQueryable();
                var RecordsFiltered = MasterMenusQuery.Count();
                var RecordsTotal = MasterMenusQuery.Count();
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    MasterMenusQuery = MasterMenusQuery.Where
                    (
                        Object =>
                        Object.Id.ToString().ToUpper().Contains(Search) ||
                        Object.KodeMenu.ToUpper().Contains(Search) ||
                        Object.NamaMenu.ToUpper().Contains(Search) ||
                        Object.JenisMenu.ToUpper().Contains(Search) ||
                        Object.HargaSatuan.ToString().ToUpper().Contains(Search)
                    );
                }
                var SortColumnName = DataTablesRequest?.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
                var SortDirection = DataTablesRequest?.Order.ElementAt(2).Dir.ToLower();
                MasterMenusQuery = MasterMenusQuery.OrderBy($"{SortColumnName} {SortDirection}");
                var Skip = DataTablesRequest.Start;
                var Take = DataTablesRequest.Length;
                var Data = MasterMenusQuery.Skip(Skip).Take(Take).ToListAsync();
                return new JsonResult(new
                {
                    Draw = DataTablesRequest.Draw,
                    RecordsTotal = RecordsTotal,
                    RecordsFiltered = RecordsFiltered,
                    Data = Data
                });

            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }

        #region Ver 1.0 : LoadData
        //[HttpPost]
        //public async Task<IActionResult> LoadData()
        //{
        //    try
        //    {
        //        var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        //        var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
        //        var Draw = Request.Form["Draw"].FirstOrDefault(); ///Request.Query
        //        var Start = Request.Form["Start"].FirstOrDefault(); ///Request.Query   
        //        var Length = Request.Form["Length"].FirstOrDefault(); ///Request.Query
        //        var SortColumn = Request.Form["Columns[" + Request.Form["Order[0][Column]"].FirstOrDefault() + "][Name]"].FirstOrDefault(); ///Request.Query
        //        var SortColumnDirection = Request.Form["Order[0][Dir]"].FirstOrDefault(); ///Request.Query
        //        var SearchValue = Request.Form["Search[Value]"].FirstOrDefault(); ///Request.Query
        //        int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
        //        int Skip = Start != null ? Convert.ToInt32(Start) : 0;
        //        int RecordsTotal = 0;
        //        var GetData = DBContext.MasterMenus.AsNoTracking().Distinct(); //.AsQueryable();
        //        var RecordsFiltered = GetData.Count();
        //        RecordsTotal = GetData.Count();
        //        if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDirection)))
        //        {
        //            GetData = GetData.OrderBy(SortColumn + " " + SortColumnDirection);
        //        }
        //        if (!string.IsNullOrEmpty(SearchValue))
        //        {
        //            GetData = GetData.Where
        //            (
        //                Object =>
        //                Object.Id.ToString().Contains(SearchValue) ||
        //                Object.KodeMenu.ToString().Contains(SearchValue) ||
        //                Object.NamaMenu.ToString().Contains(SearchValue) ||
        //                Object.JenisMenu.ToString().Contains(SearchValue) ||
        //                Object.HargaSatuan.ToString().Contains(SearchValue)
        //            );
        //        }
        //        var Data = GetData.Skip(Skip).Take(PageSize).ToList();
        //        var jsonData = new
        //        {
        //            Draw = Draw,
        //            RecordsTotal = RecordsTotal,
        //            RecordsFiltered = RecordsFiltered,
        //            Data = Data
        //        };
        //        return Ok(jsonData);
        //    }
        //    catch (Exception Error)
        //    {
        //        Console.WriteLine(Error.Message.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //    }
        //}
        #endregion


        #region Ver 1.1 : LoadData
        [HttpPost]
        public async Task<JsonResult> LoadData()
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var MasterMenusQuery = DBContext.MasterMenus.AsNoTracking().AsQueryable();
                var RecordsTotal = MasterMenusQuery.Count();
                var RecordsFiltered = MasterMenusQuery.Count();
                var SearchValue = DataTablesRequest?.Search.Value?.ToUpper();
                if (!string.IsNullOrWhiteSpace(SearchValue))
                {
                    MasterMenusQuery = MasterMenusQuery.Where
                    (
                        Object =>
                        Object.Id.ToString().ToUpper().Contains(SearchValue) ||
                        Object.KodeMenu.ToUpper().Contains(SearchValue) ||
                        Object.NamaMenu.ToUpper().Contains(SearchValue) ||
                        Object.JenisMenu.ToUpper().Contains(SearchValue) ||
                        Object.HargaSatuan.ToString().ToUpper().Contains(SearchValue)
                    );
                }
                var SortColumnName = DataTablesRequest?.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
                var SortDirection = DataTablesRequest?.Order.ElementAt(0).Dir.ToLower();
                //MasterMenuQuery = MasterMenuQuery.OrderBy(SortColumnName + " " + SortDirection);
                MasterMenusQuery = MasterMenusQuery.OrderBy($"{SortColumnName} {SortDirection}");
                var Skip = DataTablesRequest.Start;
                var Take = DataTablesRequest.Length;
                var Result = await MasterMenusQuery.Select(x => new
                {
                    Id = x.Id,
                    KodeMenu = x.KodeMenu,
                    NamaMenu = x.NamaMenu,
                    JenisMenu = x.JenisMenu,
                    HargaSatuan = "Rp. " + x.HargaSatuan
                }).Skip(Skip).Take(Take).ToListAsync();
                return new JsonResult(new
                {
                    Draw = DataTablesRequest.Draw,
                    RecordsTotal = RecordsTotal,
                    RecordsFiltered = RecordsFiltered,
                    Data = Result
                });
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }
        #endregion
    }
}
