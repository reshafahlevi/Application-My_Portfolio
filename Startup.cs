namespace My_Portfolio
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseSession();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
			services.AddSession();
			services.AddDistributedMemoryCache();
			services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60);});
        }

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSession();
        //    services.AddDistributedMemoryCache();
        //    services.AddSession(options => {
        //        options.IdleTimeout = TimeSpan.FromMinutes(1);// here you can mention the timings
        //    });
        //    services.AddMvc();
        //}
    }
}
