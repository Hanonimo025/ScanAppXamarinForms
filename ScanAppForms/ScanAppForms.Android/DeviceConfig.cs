using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Xamarin.Forms;

[assembly: Dependency(typeof(ScanAppForms.Droid.DeviceConfig))]
namespace ScanAppForms.Droid
{
    public class DeviceConfig : Java.Lang.Object, EMDKManager.IEMDKListener, IDeviceConfig
    {
        #region PropiedadesScanner
        private EMDKManager emdkManager = null;
        private BarcodeManager barcodeManager = null;
        private Scanner scanner = null;
        #endregion


        public void EnciendeEscaner()
        {
            EMDKResults results = EMDKManager.GetEMDKManager(Android.App.Application.Context, this);

            if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                // EMDKManager object initialization success
                Console.WriteLine("Main", "Status: EMDKManager object creation failed.");
            }
            else
            {
                // EMDKManager object initialization failed
                Console.WriteLine("Main", "Status: EMDKManager object creation succeeded.");
            }
        }

        public void ApagarEscaner()
        {
            OnClosed();
        }

        public void OnClosed()
        {
            if (emdkManager != null)
            {
                if (barcodeManager != null)
                {
                    // Remove connection listener
                    barcodeManager.Connection -= BarcodeManager_Connection;
                    barcodeManager = null;
                }

                // Release all the resources
                emdkManager.Release();
                emdkManager = null;
            }
        }

        public void OnOpened(EMDKManager emdkManagerInstance)
        {
            emdkManager = emdkManagerInstance;

            try
            {
                barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                if (barcodeManager != null)
                {
                    barcodeManager.Connection += BarcodeManager_Connection;
                }

                var scannerlist = barcodeManager.SupportedDevicesInfo;
                foreach (var item in scannerlist)
                {
                    Console.WriteLine(item.FriendlyName);
                    Console.WriteLine(item.Class);
                    Console.WriteLine(item.DeviceIdentifier);
                }
                scanner = barcodeManager.GetDevice(scannerlist.Where(d => d.FriendlyName.Contains("2D Barcode")).Single());
                scanner.Data += Scanner_Data;
                scanner.Status += Scanner_Status;
                scanner.TriggerType = Scanner.TriggerTypes.Hard;

                try
                {
                    scanner.Enable();
                }
                catch (ScannerException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    //throw;
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine("PedidoActivity -> OnOpened", ex.Message);
            }
        }

        private void Scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            StatusData statusData = e.P0;
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                var statusString = "Status: " + statusData.FriendlyName + " is enabled and idle...";
                //RunOnUiThread(() => textViewStatus.Text = statusString);

                if (true)
                {
                    try
                    {
                        // An attempt to use the scanner continuously and rapidly (with a delay < 100 ms between scans) 
                        // may cause the scanner to pause momentarily before resuming the scanning. 
                        // Hence add some delay (>= 100ms) before submitting the next read.
                        try
                        {
                            Thread.Sleep(1000);
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }

                        // Submit another read to keep the continuation
                        scanner.Read();
                    }
                    catch (ScannerException ex)
                    {
                        statusString = "Status: " + ex.Message;
                        //RunOnUiThread(() => textViewStatus.Text = statusString);
                        Console.WriteLine(ex.StackTrace);
                    }
                    catch (NullReferenceException ex)
                    {
                        statusString = "Status: An error has occurred.";
                        //RunOnUiThread(() => textViewStatus.Text = statusString);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }

            if (state == StatusData.ScannerStates.Waiting)
            {
                Console.WriteLine("Status: Scanner is waiting for trigger press...");

            }

            if (state == StatusData.ScannerStates.Scanning)
            {
                Console.WriteLine("Status: Scanning...");

            }

            if (state == StatusData.ScannerStates.Disabled)
            {
                Console.WriteLine("Status: " + statusData.FriendlyName + " is disabled.");
            }

            if (state == StatusData.ScannerStates.Error)
            {
                Console.WriteLine("Status: An error has occurred.");
            }
        }

        private void Scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                string dataString = scanData.First().Data;
                Console.WriteLine(dataString);

            }
        }

        private void BarcodeManager_Connection(object sender, BarcodeManager.ScannerConnectionEventArgs e)
        {
            ScannerInfo scannerInfo = e.P0;
            BarcodeManager.ConnectionState connectionState = e.P1;

            string statusBT = connectionState.ToString();
            string scannerNameBT = scannerInfo.FriendlyName;
        }
    }
}