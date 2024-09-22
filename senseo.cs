using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Threading;
using Java.Util;

/**
 * Code de l'app permettant de controler la cafetière Senseo en bluetooth
 * Fait avec Xamarin pour tester
 */
namespace SenseoBT
{
	[Activity (Label = "SenseoBT", MainLauncher = true, Icon = "@drawable/icon", 
		ScreenOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape,
		Theme = "@android:style/Theme.Holo.Light")]
	public class MainActivity : Activity
	{
		private const int REQUEST_CONNECT_DEVICE = 1;
		private const int REQUEST_ENABLE_BT = 2;
		private static readonly UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

		Boolean btnONOFFState = false;
		bool connected = false;
		BluetoothDevice senseo = null;
		BluetoothSocket btSocket = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			// Bluetooth activation
			SetupBluetooth();

			// Bouton ON / OFF
			Button buttonONOFF = FindViewById<Button> (Resource.Id.btnONOFF);
			
			buttonONOFF.Click += delegate {
				buttonONOFF.Text = btnONOFFState ? "OFF" : "ON";
				buttonONOFF.SetBackgroundResource(btnONOFFState ? Resource.Drawable.btnoff : Resource.Drawable.btnon);
				btnONOFFState = !btnONOFFState;

				if (connected && btSocket.OutputStream.CanWrite) {
					btSocket.OutputStream.WriteByte(btnONOFFState ? (byte)'o' : (byte)'f');
				}
			};
		}

		private void SetupBluetooth() 
		{
			var pg = new ProgressDialog (this);
			pg.SetIcon (Resource.Drawable.bt);
			pg.SetTitle ("Connexion Senso");
			pg.SetMessage ("Recherche de la Senseo");
			pg.Show ();


			BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

			if (!mBluetoothAdapter.IsEnabled) {
				Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
				StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
			}

			var pairedDevices = mBluetoothAdapter.BondedDevices;

			if (pairedDevices.Count > 0) {
				foreach (var device in pairedDevices) {
					if (device.Name == "HC-06") {
						senseo = device;
					}
				}
			}

			if (senseo == null) {
				new AlertDialog.Builder(this)
					.SetPositiveButton("Yes", (sender, args) =>
						{
							SetupBluetooth();
						})
					.SetNegativeButton("No", (sender, args) =>
						{

						})
					.SetMessage("Impossible trouver la Senseo, réessayer ?")
					.SetTitle("Connexion impossible")
					.SetIcon(Resource.Drawable.bt)
					.Show();
			} 
			else {
				btSocket = senseo.CreateInsecureRfcommSocketToServiceRecord (MY_UUID);

				if (btSocket != null) {
					try{
						pg.SetMessage ("Tentative de connexion");
						btSocket.Connect ();
						connected = true;
						pg.Hide();
					}
					catch (Exception e)
					{
						pg.Hide ();
						new AlertDialog.Builder(this)
							.SetPositiveButton("Yes", (sender, args) =>
								{
									senseo = null;
									SetupBluetooth();
								})
							.SetNegativeButton("No", (sender, args) =>
								{
									
								})
							.SetMessage("Impossible de se connecter, réessayer ?")
							.SetTitle("Connexion impossible")
							.SetIcon(Resource.Drawable.bt)
							.Show();
					}

					//ManageConnectedDevice(btSocket);
				}
			
			}
		}
		/*
		async private void ManageConnectedDevice(BluetoothSocket btSocket) 
		{
			byte[] buffer = new byte[1024];  // buffer store for the stream
			int bytes; // bytes returned from read()

			// Keep listening to the InputStream until an exception occurs
			while (true) {
				if (btSocket.InputStream.CanRead) {
					
				}

				// Send the obtained bytes to the UI activity
				Toast.MakeText (this, buffer.ToString, ToastLength.Short);
			}
		}
		*/
	}
}

