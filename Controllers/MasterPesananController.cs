using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Data;
using System.Linq.Dynamic.Core;
using My_API.Additional;
using My_API.Connection;

namespace My_Portfolio.Controllers
{
    public class MasterPesananController : Controller
    {
        #region Property DataTables
        [BindProperty]
        public DataTables.DataTablesRequest? DataTablesRequest { get; set; }
        #endregion

        #region Connection Database
        private readonly ApplicationDBContext DBContext;
        private readonly ILogger<MasterPesananController> _logger;
        static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
        //public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
        #endregion

        public MasterPesananController(ILogger<MasterPesananController> logger, ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MasterPesanan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AutonumberKodePesanan(string data)
        {
            try
            {
                //var DBContext = new ApplicationDBContext(GetConnection);

                int Auto = 1;
                int TemporaryNumber;
                var Kode = string.Empty;
                var Result = string.Empty;
                var NamaMenu = string.Empty;
                if (data == "Food" || data.Contains("Food"))
                {
                    var QueryMasterPesanan = await (from MasterMenu in DBContext.MasterMenus
                                                    where MasterMenu.JenisMenu == data || MasterMenu.JenisMenu.Contains(data)
                                                    orderby (int?)MasterMenu.KodeMenu.Length descending, MasterMenu.KodeMenu descending
                                                    select new
                                                    {
                                                        MasterMenu.Id,
                                                        MasterMenu.JenisMenu,
                                                        MasterMenu.KodeMenu,
                                                        MasterMenu.NamaMenu
                                                    }).Take(1).ToListAsync();

                    foreach (var Obj in QueryMasterPesanan)
                    {
                        TemporaryNumber = Convert.ToInt32(Obj.KodeMenu.Substring(Obj.KodeMenu.Length - 2, 2)) + Auto;
                        Kode = Obj.KodeMenu.Substring(0, 7);
                        Result = string.Concat(Kode, TemporaryNumber);
                        NamaMenu = Obj.NamaMenu;
                    }
                }
                else if (data == "Drink" || data.Contains("Drink"))
                {
                    var QueryMasterPesanan = await (from MasterMenu in DBContext.MasterMenus
                                                    where MasterMenu.JenisMenu == data || MasterMenu.JenisMenu.Contains(data)
                                                    orderby (int?)MasterMenu.KodeMenu.Length descending, MasterMenu.KodeMenu descending
                                                    select new
                                                    {
                                                        MasterMenu.Id,
                                                        MasterMenu.JenisMenu,
                                                        MasterMenu.KodeMenu,
                                                        MasterMenu.NamaMenu
                                                    }).Take(1).ToListAsync();

                    foreach (var Obj in QueryMasterPesanan)
                    {
                        TemporaryNumber = Convert.ToInt32(Obj.KodeMenu.Substring(Obj.KodeMenu.Length - 1, 1)) + Auto;
                        Kode = Obj.KodeMenu.Substring(0, 7);
                        Result = string.Concat(Kode, TemporaryNumber);
                        NamaMenu = Obj.NamaMenu;
                    }
                }

                var JSONData = new
                {
                    Result
                };
                //return Ok(JSONData);
                return View(JSONData);
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
        //        //var AppDBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(@"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;").Options;
        //        //var DBContext = new ApplicationDBContext(AppDBContext);
        //        var DBContext = new ApplicationDBContext(GetConnection);
        //        var Draw = Request.Form["Draw"].FirstOrDefault(); ///Request.Query
        //        var Start = Request.Form["Start"].FirstOrDefault(); ///Request.Query   
        //        var Length = Request.Form["Length"].FirstOrDefault(); ///Request.Query
        //        var SortColumn = Request.Form["Columns[" + Request.Form["Order[0][Column]"].FirstOrDefault() + "][Name]"].FirstOrDefault(); ///Request.Query
        //        var SortColumnDirection = Request.Form["Order[0][Dir]"].FirstOrDefault(); ///Request.Query
        //        var SearchValue = Request.Form["Search[Value]"].FirstOrDefault(); ///Request.Query
        //        int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
        //        int Skip = Start != null ? Convert.ToInt32(Start) : 0;
        //        int RecordsTotal = 0;
        //        var MasterPesananQuery = DBContext.MasterPesanans.AsNoTracking().AsQueryable();
        //        var RecordsFiltered = MasterPesananQuery.Count();
        //        RecordsTotal = MasterPesananQuery.Count();
        //        if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDirection)))
        //        {
        //            MasterPesananQuery = MasterPesananQuery.OrderBy(SortColumn + " " + SortColumnDirection);
        //        }
        //        if (!string.IsNullOrEmpty(SearchValue))
        //        {
        //            MasterPesananQuery = MasterPesananQuery.Where
        //            (
        //                Object =>
        //                Object.Id.ToString().ToUpper().Contains(SearchValue) ||
        //                Object.KodePemesanan.ToString().ToUpper().Contains(SearchValue) ||
        //                Object.NamaPemesan.ToUpper().Contains(SearchValue) ||
        //                Object.KodeMenu.ToString().ToUpper().Contains(SearchValue) ||
        //                Object.NamaMenu.ToUpper().Contains(SearchValue) ||
        //                Object.JumlahPesanan.ToString().ToUpper().Contains(SearchValue) ||
        //              Object.HargaSatuan.ToString().Contains(SearchValue)
        //            );
        //        }
        //        var Data = MasterPesananQuery.Skip(Skip).Take(PageSize).ToList();
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
                //var DBContext = new ApplicationDBContext(GetConnection);
                var MasterPesananQuery = DBContext.MasterPesanans.AsNoTracking().AsQueryable();
                var RecordsTotal = MasterPesananQuery.Count();
                var RecordsFiltered = MasterPesananQuery.Count();
                var SearchValue = DataTablesRequest?.Search.Value?.ToUpper();
                if (!string.IsNullOrWhiteSpace(SearchValue))
                {
                    MasterPesananQuery = MasterPesananQuery.Where
                    (
                        Object =>
                        Object.Id.ToString().ToUpper().Contains(SearchValue) ||
                        Object.KodePemesanan.ToString().ToUpper().Contains(SearchValue) ||
                        Object.NamaPemesan.ToUpper().Contains(SearchValue) ||
                        Object.KodeMenu.ToString().ToUpper().Contains(SearchValue) ||
                        Object.NamaMenu.ToUpper().Contains(SearchValue) ||
                        Object.JumlahPesanan.ToString().ToUpper().Contains(SearchValue) ||
                        Object.HargaSatuan.ToString().ToUpper().Contains(SearchValue)
                    );
                }
                var SortColumnName = DataTablesRequest?.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
                var SortDirection = DataTablesRequest?.Order.ElementAt(0).Dir.ToLower();
                MasterPesananQuery = MasterPesananQuery.OrderBy($"{SortColumnName} {SortDirection}");
                var Skip = DataTablesRequest.Start;
                var Take = DataTablesRequest.Length;
                var Result = await MasterPesananQuery.Select(x => new
                {
                    Id = x.Id,
                    KodePemesanan = x.KodePemesanan,
                    NamaPemesan = x.NamaPemesan,
                    NamaMenu = x.NamaMenu,
                    JumlahPesanan = x.JumlahPesanan,
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