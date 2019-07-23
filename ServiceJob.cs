using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.ServiceProcess;
using HtmlAgilityPack;
using Quartz;
public class ServiceJob:IJob
{
    private const string TEST_URL = "XXXX";
    private ServiceController service = new ServiceController("RasyoTek");
    private TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

    private const string LOG_PATH = @"C:\Temp\RasyoTekService.Log";

    public static string[] inputList;
    public async Task Execute(IJobExecutionContext context)
    {
        HtmlWeb document = null;
        HtmlDocument source = null;
        try
        {
            inputList  = new string[]{"kullaniciAdi","isyeriKodu","isyeriSifresi","guvenlikKodu","kaydet"};
    
            HtmlNode.ElementsFlags.Remove("form");
            document = new HtmlWeb();
            source = document.Load(TEST_URL);
    
            if (!checkInputs(source))
            {
                foreach (var item in inputList)
                {
                    Logging(item+" bulunamadi!","Error");
                }
                Logging("Inputlar bulunamadi!","Error");
                StopService(); 
            }
            else if (!checkChaptaImg(source))
            {
                Logging("Captcha.jpg bulunamadi!","Error");
                StopService();
            }
            else
            {
                Logging("Sorun bulunmadi!","Info");
                StartService();
            }

        }
        catch (System.Exception)
        {
            Logging("Siteye ulasilamiyor!","Error");
        }
    }

    public void StartService()
    {
        if (service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running,timeout);
                Logging("Servis baslatiliyor!","Info");
            }   
        else
        {
            Logging("Servis calisiyor!","Ifnfo");
        } 
    }

    public void StopService()
    {
        if (service.Status == ServiceControllerStatus.Running)
        {
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped,timeout);
            Logging("Servis durduruluyor!","Error");
        }
        else
        {
            Logging("Servis calismiyor!","Error");
        }
    }

    public static bool checkInputs(HtmlDocument source)
        {
            bool result = false;
            var inputs = source.DocumentNode.SelectNodes("//input");
            foreach (var item in inputs)
            {
                if(inputList.Any(item.Attributes["name"].Value.Contains))
                {
                    inputList = inputList.Where(x => x != item.Attributes["name"].Value).ToArray();
                    result =  true;
                }
                if (inputList.Count()>0)
                {
                    result = false;
                }
                
            }
            return result;
        }

    public static bool checkChaptaImg(HtmlDocument source)
        {
            var imgs = source.DocumentNode.SelectNodes("//img");
            foreach (var item in imgs)
            {
                if (item.Attributes["src"].Value == "Captcha.jpg")
                {
                    return true;
                }
            }
            return false;
        }

    private void Logging(string message, string level)
    {
        if (!File.Exists(LOG_PATH))
        {
            using (StreamWriter sw = File.CreateText(LOG_PATH))
            {
                sw.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + message);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(LOG_PATH))
            {
                sw.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + message);
            }
        }

    }
}