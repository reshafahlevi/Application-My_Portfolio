using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_API.Additional;
using My_API.Connection;
using My_API.Models;
using System.Data;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using static My_API.Additional.Payload;

namespace My_Portfolio.Controllers
{
	public class MasterMenuController : Controller
    {
        #region Property DataTables
        [BindProperty]
        public DataTables.DataTablesRequest? DataTablesRequest { get; set; }
        #endregion

        #region Connection Database
        private readonly ApplicationDBContext DBContext;
        private readonly ILogger<MasterMenuController> _logger;
        static IConfiguration AppSettingJSON = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        //public static string GetConnection = AppSettingJSON.GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
        public static string GetConnection = AppSettingJSON["ConnectionStrings:DefaultConnection"].ToString();
        //public string ConnectionStringDataBase = @"Server=RESHAPROGRAMMER; Database=DB_Inspirotechs; User Id=sa; Password=P@ssw0rd; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true; PersistSecurityInfo=True;";
        #endregion

        public MasterMenuController(ILogger<MasterMenuController> logger, ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MasterMenu()
        {
            return View();
        }

        #region Autonumber KodeMenu
        [HttpPost]
        public async Task<IActionResult> Autonumber_KodeMenu([FromBody] string JenisMenu)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                int Auto = 1;
                int CheckDigitNumber;
                var CheckCharachter = string.Empty;
                var kodeMenu = string.Empty;
                if (JenisMenu == "Food" || JenisMenu.Contains("Food"))
                {
                    var GetData = await (from MasterMenu in DBContext.MasterMenus.AsNoTracking()
                                         where MasterMenu.JenisMenu == JenisMenu || MasterMenu.JenisMenu.Contains(JenisMenu)
                                         orderby (int?)MasterMenu.KodeMenu.Length descending, MasterMenu.KodeMenu descending
                                         select new
                                         {
                                             MasterMenu.KodeMenu
                                         }).FirstOrDefaultAsync();   //.ToListAsync();

                    var CodeUniqueFood = GetData?.KodeMenu == null || GetData?.KodeMenu == "" || GetData?.KodeMenu == string.Empty ? null : GetData?.KodeMenu.Substring(0, 4);
                    var CodeNumber = GetData?.KodeMenu == null || GetData?.KodeMenu == "" || GetData?.KodeMenu == string.Empty ? null : GetData?.KodeMenu.Substring(4);
                    CheckDigitNumber = Convert.ToInt32(CodeNumber) + Auto;
                    kodeMenu = string.Concat(CodeUniqueFood, CheckDigitNumber);
                }
                else if (JenisMenu == "Drink" || JenisMenu.Contains("Drink"))
                {
                    var GetData = await (from MasterMenu in DBContext.MasterMenus.AsNoTracking()
                                         where MasterMenu.JenisMenu == JenisMenu || MasterMenu.JenisMenu.Contains(JenisMenu)
                                         orderby (int?)MasterMenu.KodeMenu.Length descending, MasterMenu.KodeMenu descending
                                         select new
                                         {
                                             MasterMenu.KodeMenu
                                         }).FirstOrDefaultAsync();   //.ToListAsync();

                    var CodeUniqueDrink = GetData?.KodeMenu == null || GetData?.KodeMenu == "" || GetData?.KodeMenu == string.Empty ? null : GetData?.KodeMenu.Substring(0, 4);
                    var CodeNumber = GetData?.KodeMenu == null || GetData?.KodeMenu == "" || GetData?.KodeMenu == string.Empty ? null : GetData?.KodeMenu.Substring(4);
                    CheckDigitNumber = Convert.ToInt32(CodeNumber) + Auto;
                    kodeMenu = string.Concat(CodeUniqueDrink, CheckDigitNumber);
                }

                var JSONData = new
                {
                    kodeMenu
                };

                return new JsonResult(JSONData);    //return Ok(JSONData);
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

        #region Add Data
        [HttpPost]
        public async Task<JsonResult> AddData([FromBody] PayloadAddMasterMenu PayloadAdd)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var Data = await (from data in DBContext.MasterMenus
                                  orderby data.Id descending
                                  select new
                                  {
                                      Id = data.Id
                                  }).FirstOrDefaultAsync();
                DBContext.Database.BeginTransaction();
                var JSONData = new MasterMenu
                {
                    Id = Data.Id + 1,
                    KodeMenu = PayloadAdd.KodeMenu,
                    NamaMenu = PayloadAdd.NamaMenu,
                    JenisMenu = PayloadAdd.JenisMenu,
                    HargaSatuan = PayloadAdd.HargaSatuan
                };
                DBContext.Add(JSONData);
                DBContext.Database.CommitTransaction();
                DBContext.SaveChanges();
                return new JsonResult(JSONData);
                //return Ok(JSONData);
            }
            catch (Exception Error)
            {
                DBContext.Database.RollbackTransaction();
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }
        #endregion

        #region Update Data
        [HttpPut]
        public async Task<JsonResult> UpdateData([FromBody] PayloadUpdateMasterMenu PayloadUpdate)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var Data = await DBContext.MasterMenus.Where(w => w.KodeMenu == PayloadUpdate.KodeMenu).Select(s => new
                {
                    Id = s.Id
                }).Distinct().FirstOrDefaultAsync();
                DBContext.Database.BeginTransaction();
                var TempData = new MasterMenu
                {
                    Id = Data.Id,
                    KodeMenu = PayloadUpdate.KodeMenu,
                    NamaMenu = PayloadUpdate.NamaMenu,
                    JenisMenu = PayloadUpdate.JenisMenu,
                    HargaSatuan = PayloadUpdate.HargaSatuan
                };
                DBContext.Update(TempData);
                DBContext.Database.CommitTransaction();
                DBContext.SaveChanges();
                return new JsonResult(TempData);
            }
            catch (Exception Error)
            {
                DBContext.Database.RollbackTransaction();
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }
        #endregion

        #region Delete Data
        [HttpPost]
        public async Task<JsonResult> DeleteDataById([FromBody] string kodeMenu)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var Data = await (from x in DBContext.MasterMenus.AsNoTracking().Include(x => x.MasterPesanans).ThenInclude(x => x.MasterTransaksis) where x.KodeMenu == kodeMenu select x).Distinct().FirstOrDefaultAsync();
                DBContext.Database.BeginTransaction();
                DBContext.MasterMenus.RemoveRange(Data);
                DBContext.Database.CommitTransaction();
                DBContext.SaveChanges();
                return new JsonResult(1);
            }
            catch (Exception Error)
            {
                DBContext.Database.RollbackTransaction();
                Console.WriteLine(Error.Message.ToString());
                throw;
            }
            finally
            {
            }
        }
        #endregion

        #region Get Data By Id
        [HttpPost]
        public async Task<IActionResult> GetDataById([FromBody] long Id)
        {
            try
            {
                //var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                //var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
                var GetData = await (from x in DBContext.MasterMenus.AsNoTracking() where x.Id == Id select x).Distinct().FirstOrDefaultAsync();
                var JSONData = new
                {
                    JenisMenu = GetData?.JenisMenu == null || GetData?.JenisMenu == "" || GetData?.JenisMenu == string.Empty ? null : GetData?.JenisMenu,
                    KodeMenu = GetData?.KodeMenu == null || GetData?.KodeMenu == "" || GetData?.KodeMenu == string.Empty ? null : GetData?.KodeMenu,
                    NamaMenu = GetData?.NamaMenu == null || GetData?.NamaMenu == "" || GetData?.NamaMenu == string.Empty ? null : GetData?.NamaMenu,
                    HargaSatuan = GetData?.HargaSatuan == null || GetData?.HargaSatuan == 0 ? null : GetData?.HargaSatuan
                };
                return new JsonResult(JSONData);
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

        #region Ver 1.0 : LoadData
        //[HttpPost]
        //public async Task<IActionResult> LoadData()
        //{
        //    try
        //    {
        //        var ConnectionString = AppSettingJSON.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        //        var DBContext = new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlServer(ConnectionString).Options;
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
        //        var MasterMenuQuery = DBContext.MasterMenus.AsNoTracking().AsQueryable();
        //        var RecordsFiltered = MasterMenuQuery.Count();
        //        RecordsTotal = MasterMenuQuery.Count();
        //        if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDirection)))
        //        {
        //            MasterMenuQuery = MasterMenuQuery.OrderBy(SortColumn + " " + SortColumnDirection);
        //        }
        //        if (!string.IsNullOrEmpty(SearchValue))
        //        {
        //            MasterMenuQuery = MasterMenuQuery.Where
        //            (
        //                Object =>
        //                Object.Id.ToString().ToUpper().Contains(SearchValue) ||
        //                Object.KodeMenu.ToUpper().Contains(SearchValue) ||
        //                Object.NamaMenu.ToUpper().Contains(SearchValue) ||
        //                Object.JenisMenu.ToUpper().Contains(SearchValue) ||
        //                Object.HargaSatuan.ToString().ToUpper().Contains(SearchValue)
        //            );
        //        }
        //        var Data = MasterMenuQuery.Skip(Skip).Take(PageSize).ToList();
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
                var MasterMenuQuery = DBContext.MasterMenus.AsNoTracking().AsQueryable();
                var RecordsTotal = MasterMenuQuery.Count();
                var RecordsFiltered = MasterMenuQuery.Count();
                var SearchValue = DataTablesRequest?.Search.Value?.ToUpper();
                if (!string.IsNullOrWhiteSpace(SearchValue))
                {
                    MasterMenuQuery = MasterMenuQuery.Where
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
                MasterMenuQuery = MasterMenuQuery.OrderBy($"{SortColumnName} {SortDirection}");
                var Skip = DataTablesRequest.Start;
                var Take = DataTablesRequest.Length;
                var Result = await MasterMenuQuery.Select(x => new
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

        #region Example Include And ThenInclude
        //public async Task<IActionResult> Example_Include_ThenInclude()
        //{
        //    ///Penggunaan "AsSingleQuery()" akan melakukan 1 kali hit select table, tetapi akan terjadi double data pada table tersebut.
        //    ///Penggunaan "AsNoTracking()" hanya untuk select data <--- jangan dipakai untuk update, delete, insert data.
        //    ///Penggunaan "AsSplitQuery()" data tidak akan double, tetapi akan terjadi 3 kali select table A, B, C atau lebih dari jumlah banyaknya table.
        //    ///
        //    var Test1 = await applicationDBContext.MasterMenus.ToListAsync(); ///<--- default'nya menggunakan AsSingleQuery()
        //    var Test2 = await applicationDBContext.MasterMenus.Include(x => x.MasterPesanans).AsNoTracking().ToListAsync();
        //    var Test3 = await applicationDBContext.MasterMenus.Include(x => x.MasterPesanans).ThenInclude(x => x.MasterTransaksis).AsNoTracking().AsSplitQuery().Select(a => new 
        //    { 
        //        KodeMenu = a.KodeMenu,
        //        NamaMenu = a.NamaMenu,
        //        MasterPesanans = a.MasterPesanans.Select(b => new 
        //        {
        //            KodePemesanan = b.KodePemesanan,
        //            NamaPemesan = b.NamaPemesan,
        //            MasterTransaksis = b.MasterTransaksis.Select(c => new 
        //            {
        //                KodeMenu = c.KodeMenu,
        //                EmployeeId = c.EmployeeId,
        //                PesananId = c.PesananId
        //            })
        //        })
        //    }).ToListAsync();
        //    return Json(Test3);
        //}
        #endregion
    }
}
