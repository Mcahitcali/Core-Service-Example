using Topshelf;

namespace Core_Service_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            initialService();
        }

        public static void initialService()
        {
            HostFactory.Run(windowsService =>
            {
                windowsService.Service<TestService>(s =>
                {
                    s.ConstructUsing(service => new TestService()); 
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.SetDescription("RasyoTek Kontrol Servisi");
                windowsService.SetDisplayName("RasyoTek Kontrol");
                windowsService.SetServiceName("RasyoKontrol");
            });
        }
    }
}
