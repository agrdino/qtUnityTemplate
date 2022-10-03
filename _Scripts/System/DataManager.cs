using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using _Prefab.Popup;
using _Scripts.RrpcRequest;
using GFramework;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using MasterData;
using UnityEngine;
using Newtonsoft.Json;
using UnityCipher;
namespace _Scripts.System
{
	public partial class DataManager : qtSingleton<DataManager>
	{
		#region ----- KEY PATH -----

		#endregion

		#region ----- VARIABLE -----


		#endregion

		#region ----- INITIALIZE -----

		protected override void Init()
		{
			base.Init();
			Initialize();
			DontDestroyOnLoad(this);
		}

		public void Initialize()
		{
		}

		#endregion

		#region ----- PUBLIC FUNCTION -----


		#endregion

		#region ----- PRIVATE FUNCTION -----

		
		#endregion

		#region ----- PUBLIC FUNCTION -----

		
		#endregion
	}
}