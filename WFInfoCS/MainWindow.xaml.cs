﻿using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WFInfoCS {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		readonly Main main = new Main(); //subscriber
		public static MainWindow INSTANCE;

		public MainWindow() {
			INSTANCE = this;
			LowLevelListener listener = new LowLevelListener(); //publisher
			try {
				if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WFInfoCS\settings.json")) {
					Settings.settingsObj = JObject.Parse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WFInfoCS\settings.json"));
				} else {
					Settings.settingsObj = JObject.Parse("{\"Display\":\"Overlay\"," +
						"\"ActivationKey\":\"Snapshot\"," +
						"\"Scaling\":100.0," +
						"\"Auto\":false," +
						"\"Debug\":false}");
				}
				Settings.activationKey = (Key)Enum.Parse(typeof(Key), Settings.settingsObj.GetValue("ActivationKey").ToString());
				Settings.debug = (bool)Settings.settingsObj.GetValue("Debug");
				Settings.auto = (bool)Settings.settingsObj.GetValue("Auto");
				Settings.scaling = Convert.ToInt32(Settings.settingsObj.GetValue("Scaling"));

				String thisprocessname = Process.GetCurrentProcess().ProcessName;
				if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1) {
					Main.AddLog("Duplicate process found");
					Close();
				}

				LowLevelListener.KeyAction += main.OnKeyAction;
				listener.Hook();
				InitializeComponent();
				Version.Content = Main.BuildVersion;

				Main.AddLog("Sucsesfully launched");
			}
			catch (Exception e) {
				Main.AddLog("An error occured while loading the main window: " + e.Message);
			}
		}

		public void ChangeStatus(string status, int serverity) {
			Console.WriteLine(status);
			Status.Content = "Status: " + status;
			switch (serverity) {
				case 0: //default, no problem
				Status.Foreground = new SolidColorBrush(Color.FromRgb(177, 208, 217));
				break;
				case 1: //severe, red text
				Status.Foreground = Brushes.Red;
				break;
				case 2: //warning, orange text
				Status.Foreground = Brushes.Orange;
				break;
				default: //Uncaught, big problem
				Status.Foreground = Brushes.Yellow;
				break;
			}
		}

		private void Exit(object sender, RoutedEventArgs e) {
			App.Current.Shutdown();
		}

		private void Minimise(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}

		private void Website_click(object sender, RoutedEventArgs e) {
			ChangeStatus("Go go website", 0);
			Process.Start("https://wfinfo.warframestat.us/");
		}

		private void Relics_click(object sender, RoutedEventArgs e) {
			//todo, open new window, showing all relics
			ChangeStatus("Relics not implemented", 2);
		}

		private void Gear_click(object sender, RoutedEventArgs e) {
			//todo, opens new window, shows all prime items
			ChangeStatus("Equipment not implemented", 2);
			Overlay test = new Overlay();
			test.Show();

		}
		private void Settings_click(object sender, RoutedEventArgs e) {
			Settings settingsWindow = new Settings();
			settingsWindow.Show();
		}

		private void ReloadWikiClick(object sender, RoutedEventArgs e) {
			
			//todo reloads wiki data
		}

		private void ReloadDropClick(object sender, RoutedEventArgs e) {
			//todo reloads de's data
		}

		private void ReloadMarketClick(object sender, RoutedEventArgs e) {
			//todo reloads warframe.market data
		}

		// Allows the draging of the window
		private new void MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}
	}
}