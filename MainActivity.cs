using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Java.Util;
using System.Collections.Generic;

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

		Button buttonONOFF = null;
		Button buttonCUP1 = null;
		Button buttonCUP2 = null;
		Boolean btnONOFFState = false;
		Boolean buttonCUP1State = false;
		Boolean buttonCUP2State = false;
		bool connected = false;
		BluetoothDevice senseo = null;
		BluetoothSocket btSocket = null;
		LogItemAdapter logs = null;
		ProgressDialog currentPd = null;
		bool led = false;
		bool water = false;
		bool working = false;
		int timerSeconds = 25;
		System.Timers.Timer timer = null;


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			// ListView des logs
			ListView lvLog = FindViewById<ListView> (Resource.Id.lvLog);
			logs = new LogItemAdapter (this, lvLog, new List<Log> ());
			lvLog.Adapter = logs;

			logs.Add (new Log ("Démarrage de l'application", Resource.Drawable.Icon));

			// Boutons
			buttonONOFF = FindViewById<Button> (Resource.Id.btnONOFF);
			buttonCUP1 = FindViewById<Button> (Resource.Id.btnCUP1);
			buttonCUP2 = FindViewById<Button> (Resource.Id.btnCUP2);

			buttonONOFF.Click += delegate {
				buttonONOFF.Text = btnONOFFState ? "OFF" : "ON";
				int btnBackground = btnONOFFState ? Resource.Drawable.btnoff : Resource.Drawable.btnon;
				buttonONOFF.SetBackgroundResource(btnBackground);
				btnONOFFState = !btnONOFFState;

				logs.Add (new Log ("Power "+buttonONOFF.Text, btnBackground));

				if (connected && btSocket.OutputStream.CanWrite) {
					btSocket.OutputStream.WriteByte(btnONOFFState ? (byte)'o' : (byte)'f');
				
					if (btnONOFFState) {
						logs.Add (new Log (buttonONOFF.Text, Resource.Drawable.wateron));
					}
				}
			};

			buttonCUP1.Click += delegate {
				buttonCUP1.SetBackgroundResource(buttonCUP1State ? Resource.Drawable.cup1off : Resource.Drawable.cup1on);
				buttonCUP1State = !buttonCUP1State;

				if (connected && btSocket.OutputStream.CanWrite) {
					if (timer != null) {
						timer.Stop();
					}

					if (!working) {
						btSocket.OutputStream.WriteByte((byte)'1');
						logs.Add (new Log ("1 Tasse en préparation", Resource.Drawable.cup1off));

						timerSeconds = 25;
						timer = new System.Timers.Timer();
						timer.Interval = 1000;
						timer.Elapsed += OnOneCupEvent;
						timer.Start();
					}
					else {
						buttonCUP1.Text = "";
						buttonCUP1State = !buttonCUP1State;
						buttonCUP1.SetBackgroundResource(buttonCUP1State ? Resource.Drawable.cup1off : Resource.Drawable.cup1on);

						btSocket.OutputStream.WriteByte((byte)'1');
						logs.Add (new Log ("Préparation terminée", Resource.Drawable.cup1on));
					}
				}
			};

			buttonCUP2.Click += delegate {
				buttonCUP2.SetBackgroundResource(buttonCUP2State ? Resource.Drawable.cup2off : Resource.Drawable.cup2on);
				buttonCUP2State = !buttonCUP2State;

				if (connected && btSocket.OutputStream.CanWrite) {
					if (timer != null) {
						timer.Stop();
					}

					if (!working) {
						btSocket.OutputStream.WriteByte((byte)'2');
						logs.Add (new Log ("2 Tasses en préparation", Resource.Drawable.cup2off));

						timerSeconds = 25;
						timer = new System.Timers.Timer();
						timer.Interval = 1000;
						timer.Elapsed += OnTwoCupEvent;
						timer.Start();
					}
					else {
						buttonCUP2.Text = "";
						buttonCUP2State = !buttonCUP2State;
						buttonCUP2.SetBackgroundResource(buttonCUP2State ? Resource.Drawable.cup2off : Resource.Drawable.cup2on);

						btSocket.OutputStream.WriteByte((byte)'2');
						logs.Add (new Log ("Préparation terminée", Resource.Drawable.cup2on));
					}
				}
			};

			// Bluetooth activation
			SetupBluetooth();

			// Lecture des messages Bluetooth
			Task.Run (() => listenBT ());
		}

		#region handlers
		private void OnOneCupEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			RunOnUiThread (() => {
				buttonCUP1.Text = timerSeconds.ToString();
			});

			timerSeconds--;

			if (timerSeconds == 0) {
				timer.Stop();

				RunOnUiThread (() => {
					buttonCUP1.Text = "";
					buttonCUP1.SetBackgroundResource(buttonCUP1State ? Resource.Drawable.cup1off : Resource.Drawable.cup1on);
					buttonCUP1State = !buttonCUP1State;
					logs.Add (new Log ("Préparation terminée", Resource.Drawable.cup1on));
				});
			}
		}

		private void OnTwoCupEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			RunOnUiThread (() => {
				buttonCUP2.Text = timerSeconds.ToString();
			});

			timerSeconds--;

			if (timerSeconds == 0) {
				timer.Stop();

				RunOnUiThread (() => {
					buttonCUP2.Text = "";
					buttonCUP2.SetBackgroundResource(buttonCUP2State ? Resource.Drawable.cup2off : Resource.Drawable.cup2on);
					buttonCUP2State = !buttonCUP2State;
					logs.Add (new Log ("Préparation terminée", Resource.Drawable.cup2on));
				});
			}
		}
		#endregion 

		#region connexion bluetooth
		private void SetupBluetooth() 
		{
			ProgressDialog pg = new ProgressDialog (this);
			pg.SetIcon (Resource.Drawable.bt);
			pg.SetTitle ("Connexion Senso");
			pg.SetMessage ("Recherche de la Senseo");
			pg.Show ();

			logs.Add (new Log ("Connexion bluetooth", Resource.Drawable.bt));

			Task.Run (() => {
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
					RunOnUiThread(() =>  new AlertDialog.Builder(this)
						.SetPositiveButton("Yes", (sender, args) =>
							{
								pg.Dismiss();
								SetupBluetooth();
							})
						.SetNegativeButton("No", (sender, args) =>
							{
								logs.Add (new Log ("Connexion échouée", Resource.Drawable.btred));
							})
						.SetMessage("Impossible trouver la Senseo, réessayer ?")
						.SetTitle("Connexion impossible")
						.SetIcon(Resource.Drawable.bt)
						.Show()
					);
				} 
				else {
					btSocket = senseo.CreateInsecureRfcommSocketToServiceRecord (MY_UUID);

					if (btSocket != null) {
						try{
							pg.SetMessage ("Tentative de connexion");
							btSocket.Connect ();
							connected = true;
							RunOnUiThread(() => logs.Add (new Log ("Bluetooth connecté", Resource.Drawable.bt)));
							pg.Dismiss();
						}
						catch (Exception e)
						{
							pg.Dismiss ();

							RunOnUiThread(() =>  new AlertDialog.Builder(this)
								.SetPositiveButton("Yes", (sender, args) =>
									{
										senseo = null;
										SetupBluetooth();
									})
								.SetNegativeButton("No", (sender, args) =>
									{
										logs.Add (new Log ("Connexion échouée", Resource.Drawable.btred));
									})
								.SetMessage("Impossible de se connecter, réessayer ?")
								.SetTitle("Connexion impossible")
								.SetIcon(Resource.Drawable.bt)
								.Show()
							);
						}

					}
				
				}
			});
		}

		// Lecture des messages bluetooth
		async void listenBT() 
		{
			while (true) 
			{
				if (connected && btSocket.InputStream.CanRead) 
				{
					var readBuffer = new byte[1024];
					var readCount = 0;
					readCount += await btSocket.InputStream.ReadAsync (readBuffer, readCount, readBuffer.Length);

					var message = System.Text.Encoding.UTF8.GetString (readBuffer, 0, readCount);

					if (message != null && message.Contains (";")) {
						string[] commands = message.Split (';');

						foreach (string command in commands) {
							if (command.Contains ("=")) {
								string key = command.Substring (0, command.IndexOf ('='));
								string val = command.Substring (command.IndexOf ('=')+1);

								if (key != null && val != null) 
								{
									if (key == "water") {
										water = Boolean.Parse(val);
									}
									else if (key == "led") {
										led = Boolean.Parse(val);
									}
									else if (key == "working") {
										working = Boolean.Parse (val);
									}
								}
							}
						}

						// Met à jour les indicateurs de statut de la machine
						RunOnUiThread (() => updateIndicators ());
					}
					Thread.Sleep (1000);
				}
			}
		}
		#endregion


		// Met a jour des indicateurs de statut de la machine
		void updateIndicators() {
			// Active les boutons si on peut faire une action
			enableActions (water && led);

			// Eau
			if (water == true) {
				
			} else {
				
			}

			// Prepa en cours
			if (working == true) {

			} else {

			}

			// Chauffage
			if (led == true) {

			} else {

			}

			// Bluetooth
			if (btSocket.IsConnected) {
				
			}

		}

		// Active/desactive les boutons
		void enableActions(bool enable) {
			buttonCUP1.Enabled = enable;
			buttonCUP2.Enabled = enable;
		}
	}
}


