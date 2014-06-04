using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopifyHelper.Model;
using ShopifyHelper;
using System.IO;

namespace ShopifyUsageSample
{



    class Program
    {
        static void Main(string[] args)
        {
            //developer key. just for development purpose. will not return actual image
            string krakenApiKey = "acb247d883957e761db365dbe7c08e3e";
            string krakenApiScreet = "211bf5ae2cb59d99c231c6ebf2eaffd811bac53f";

            KrakenAppHelper kraken = new KrakenAppHelper(krakenApiKey, krakenApiScreet);
            KrakenResponse response;

            response = kraken.UploadFile(KrakenAppHelper.FileType.URL, "http://gambaronline.com/images/2010/09/pemandangan-alam.jpg", true);
            Console.WriteLine(string.Format("File Name: {0}", response.fileName));
            Console.WriteLine(string.Format("Original Size: {0}", response.originalSize));
            Console.WriteLine(string.Format("Kraked Size: {0}", response.krakedSize));
            Console.WriteLine(string.Format("Kraked URL: {0}", response.krakedURL));
            Console.WriteLine("===============================");

            response = kraken.UploadFile(KrakenAppHelper.FileType.LocalFile, @"C:\Users\IEUser\Pictures\test1.jpg", true);
            Console.WriteLine(string.Format("File Name: {0}", response.fileName));
            Console.WriteLine(string.Format("Original Size: {0}", response.originalSize));
            Console.WriteLine(string.Format("Kraked Size: {0}", response.krakedSize));
            Console.WriteLine(string.Format("Kraked URL: {0}", response.krakedURL));
            Console.WriteLine("===============================");

            response = kraken.UploadFile(KrakenAppHelper.FileType.LocalFile, @"C:\Users\IEUser\Downloads\orange-06.jpg", false, 100, 150, KrakenAppHelper.ImageResizingType.portrait);
            Console.WriteLine(string.Format("File Name: {0}", response.fileName));
            Console.WriteLine(string.Format("Original Size: {0}", response.originalSize));
            Console.WriteLine(string.Format("Kraked Size: {0}", response.krakedSize));
            Console.WriteLine(string.Format("Kraked URL: {0}", response.krakedURL));
            Console.WriteLine("===============================");

            response = kraken.UploadFile(KrakenAppHelper.FileType.URL, "http://www.beckershoes.com/shop/2619-thickbox_default/jambu-tokay-brown-orange.jpg", false, 100, 150, KrakenAppHelper.ImageResizingType.crop);
            Console.WriteLine(string.Format("File Name: {0}", response.fileName));
            Console.WriteLine(string.Format("Original Size: {0}", response.originalSize));
            Console.WriteLine(string.Format("Kraked Size: {0}", response.krakedSize));
            Console.WriteLine(string.Format("Kraked URL: {0}", response.krakedURL));
            Console.WriteLine("===============================");


            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }

    }
}
